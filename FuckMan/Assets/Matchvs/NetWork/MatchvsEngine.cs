using Matchvs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BestHTTP;
using LitJson;
using UnityEngine;

namespace Matchvs{
    [SuppressMessage("ReSharper","UseObjectOrCollectionInitializer")]
    public class MatchvsEngine:NW.NWCB{
        private static MatchvsEngine INSTANCE = new MatchvsEngine();
        private Dictionary<String,Delegate> mRsp;
        private Pro _mPro;
        private NW _mGTWNw;
        private NW _mHNw;
        private NW _mWNw;

        private MatchvsEngine(){
            _mPro = new Pro();
            mState = new State();
            mRsp = new Dictionary<string,Delegate>();
        }

        public State mState;
        private MonoBehaviour context;
        private Queue<FrameDataNotify> frameCache = new Queue<FrameDataNotify>();
        private Queue<LiveFrameDataNotify> frameWatchCache = new Queue<LiveFrameDataNotify>();
        private object lastJoinRoomRsp;
        private JoinWatchRoomRsp lastJoinWatchRoomRsp;

        public static MatchvsEngine getInstance(){ return INSTANCE; }

        /**
         * FrameOpt 帧同步发送选项
         */
        public enum FrameOpt{
            ONLY_CLIENT = 0,//(0),// 只发客户端
            ONLY_GS = 1,//(1),// 只发GameServer
            CLIENT_GS = 2//(2),// 同时发送客户端和GameServer
        }

        /**
         * 初始化
         * @UnityContext context #UnityContext.cs必须到任意活动的Object上,依赖它发送心跳.
         * @uint gameID 后台获取 @link matchvs.com
         * @string appKey 后台获取 @link matchvs.com
         */
        public int init(UnityContext context,uint gameID,string appKey,string channel = "Matchvs",
            string platform = "alpha",int gameVersion = 1,int threshold = 0){
            if(null==context){
                Log.e("[ERR] ill pars , UnityContext is null");
                return -1;
            }

            if(string.IsNullOrEmpty(channel)){
                Log.e("[ERR] ill pars ,channel:"+channel);
                return -1;
            }

            if(string.IsNullOrEmpty(platform)){
                Log.e("[ERR] ill pars ,platform:"+platform);
                return -1;
            }

            if(gameID<=0){
                Log.e("[ERR] ill pars ,gameID:"+gameID);
                return -1;
            }

            if(string.IsNullOrEmpty(appKey)){
                Log.e("[ERR] ill pars ,appKey:"+appKey);
                return -1;
            }

            if(MVS.mtaReport!=null){ MVS.mtaReport.Report("init"); }

            MVS.Channel = channel;
            MVS.Platform = platform;
            MVS.Version = gameVersion;
            MVS.Appkey = appKey;
            MVS.GameID = gameID;
            this.mState.SetIniting();

            this.context = context;
            context.heartBeat = heartBeat;
            if(threshold==0){
                this.getHostList();
            }
            else{
                MVS.threshold = threshold;
                this.speed();
            }

            return 0;
        }

        public void unInit(){
            if(mState.HaveLogin()){
                logout("unInit");
            }

            this.mRsp = new Dictionary<string,Delegate>();
            this.context = null;
            mState.clearState(State.INIT);
        }

        private void heartBeat(){
            _mGTWNw?.send(_mPro.en(CmdId.HeartBeatReq,MVS.GameID,MVS.RoomID));

            _mHNw?.send(_mPro.en(SDKHotelCmdID.HeartbeatCmdid,MVS.GameID,MVS.RoomID,MVS.UserID));
            _mWNw?.send(_mPro.en(SDKWatchCmdID.LiveHeartbeatCmdid,MVS.GameID,MVS.RoomID,MVS.UserID));
        }

        private void speed(){ Log.w("speed NotImplementedException"); }

        private void getHostList(){
            string url = MVS.Host.MAIN_URL+MVS.Host.HOSTLIST+"?mac=0"+"&gameid="+MVS.GameID+"&channel="+MVS.Channel+
                         "&platform="+MVS.Platform+(MVS.isNeedWSS() ? "&useWSSProxy=1":"");
            var httpRequest = new HTTPRequest(new Uri(url),(HTTPRequest req,HTTPResponse rsp)=>{
                if(rsp.IsSuccess){
                    Log.i("getHostList:{0}",rsp.DataAsText);
                    JsonData jsonData = JsonMapper.ToObject(rsp.DataAsText);//这里的JsonFile.txt文件即为上面1中的文本文件

                    int status = int.Parse(jsonData["status"].ToString());
                    string engine = jsonData["data"]["engine"].ToString();
                    string vsopen = jsonData["data"]["vsopen"].ToString();

                    MVS.Host.HOST_GATWAY_ADDR = $"ws://{engine}:7001";
                    var http = "https://";
                    MVS.Host.VS_OPEN_URL = http+vsopen;
                    Log.i("engine:{0}",MVS.Host.HOST_GATWAY_ADDR);
                    callback("initResponse",200);
                }
                else{
                    Log.w("getHostList fail: {0}",rsp.Message);
                }
            });
            httpRequest.MethodType = HTTPMethods.Post;
            httpRequest.Send();
        }

        /**
         * 登录
         * @userID {uint32} value 用户ID
         * @token {uint64} value 用户的token值
         * @deviceID { !Array.<string> } deviceID 设备ID,0
         * @nodeID {uint32}
         */
        public int login(uint userID,string token,string deviceID = "0",int nodeID = 0){
            if(userID==0){
                Log.i("[ERR] ill pars ,userID:"+userID);
                return -1;
            }

            if(string.IsNullOrEmpty(token)){
                Log.i("[ERR] ill pars ,token:"+token);
                return -1;
            }

            MVS.UserID = userID;
            MVS.Token = token;
            MVS.DeviceID = deviceID;

            if(mState.HaveLogin()){ return -1; }

            var ak = new MVS.AppKeyCheck();
            if(!ak.isInvailed(MVS.Appkey)){
                return -26;
            }

            loginConnect();

            //if (nodeID==0) {
            //    return assignNodeLogin({ nodeID: nodeID});
            //} else {
            ////上报节点获取最优节点
            //resNo = setSchedule(function(res, err) {
            //    if (err != null) {
            //        MatchvsLog.logE("node schedule error:" + JSON.stringify(err));
            //        return;
            //    }
            //    loginConnect(userID, token, deviceID);
            //}.bind(this),nodeID && nodeID !== 0);
            //if (resNo !== 0) { return resNo; }

            return 0;
        }

        public int logout(string profile = ""){
            if(mState.HaveLogin()){
                _mGTWNw?.send(_mPro.en(CmdId.LogoutReq,profile));
                _mGTWNw?.close();
                _mGTWNw = null;
                mState.clearState(State.LOGIN);
                var rsp = new LogoutRsp();
                rsp.Status = ErrorCode.Ok;
                callback("logoutResponse",rsp);
            }

            if(mState.isHaveState(State.RECONNECT)){
                mState.clearState(State.RECONNECT);
            }

            if(mState.HaveInRoom()){
                _mHNw?.close();
                _mHNw = null;
                mState.clearState(State.ROOM);
            }

            if(mState.HaveInWatch()){
                _mWNw?.close();
                _mWNw = null;
                mState.clearState(State.WATCH);
            }

            return 0;
        }

        public void disconnect(){
            _mGTWNw?.close();
            _mGTWNw = null;

            _mWNw?.close();
            _mWNw = null;

            _mHNw?.close();
            _mHNw = null;
        }

        public int createRoom(MsRoomInfo roomInfo = null,string userProfile = ""){
            if(mState.HaveInRoom()) return -1;
            if(roomInfo==null){
                roomInfo = new MsRoomInfo(JoinRoomType.JoinRandomRoom,MVS.UserID,0,MVS.GameID,2,0,1,userProfile);
            }

            if(roomInfo.maxPlayer>MVS.Config.MAXPLAYER_LIMIT||roomInfo.maxPlayer<MVS.Config.MINPLAYER_LIMIT) return -20;
            if(userProfile.Length>512) return -21;
            mState.SetJoinRooming();
            _mGTWNw?.send(_mPro.en(CmdId.CreateRoomReq,roomInfo));
            return 0;
        }

        public int reconnect(){
            if(!mState.HaveLogin()){
                mState.setState(State.RECONNECT);
                login(MVS.UserID,MVS.Token);
                Log.i("[INFO] reconnecting before login :{0}",", be ignore");
                return 0;
            }

//            if(mState.isHaveState(State.RECONNECT)){
//                Log.w("[INFO] reconnecting:{0}",", be ignore");
//                return -1;
//            }


            if(MVS.TeamID!=0){
                Log.i("[INFO] reconnecting to team :{0}",MVS.TeamID);
                joinTeam(MVS.TeamID,MVS.PassWord,"reconnectJoinTeam",true);
            }

            if(MVS.RoomID!=0){
                Log.i("[INFO] reconnecting to room :{0}",MVS.RoomID);
                joinRoom(MVS.RoomID,"reconnectJoinRoom",true);
            }

            mState.clearState(State.RECONNECT);
            return 0;
        }


        public int joinRandomRoom(uint maxPlayer,string userProfile = ""){
            if(!mState.HaveLogin()) return -1;
            if(mState.HaveInRoom()) return -1;
            if(maxPlayer>MVS.Config.MAXPLAYER_LIMIT||maxPlayer<MVS.Config.MINPLAYER_LIMIT) return -20;
            if(userProfile.Length>512) return -21;
            var roomJoin = new MsRoomInfo(JoinRoomType.JoinRandomRoom,MVS.UserID,0,MVS.GameID,maxPlayer,0,1,
                userProfile);
            mState.SetJoinRooming();
            _mGTWNw?.send(_mPro.en(CmdId.JoinRoomReq,roomJoin));
            return 0;
        }

        public int joinRoom(ulong roomID,string userProfile = "",bool isReconnection = false){
//            if(!mState.HaveLogin()) return -1;
//            if(mState.HaveInRoom()) return -1;
            var roomJoin = new MsRoomInfo(isReconnection ? JoinRoomType.RejoinSpecialRoom:JoinRoomType.JoinSpecialRoom,
                MVS.UserID,roomID,MVS.GameID,0,0,0,userProfile);
            Log.i("[INFO] joinRoomWithRoomID:{0},userID:{1}",roomID,MVS.UserID);
            var buf = this._mPro.en(CmdId.JoinRoomReq,roomJoin);
            this.mState.SetJoinRooming();
            this._mGTWNw?.send(buf);
            return 0;
        }

        public int joinWatchRoom(ulong roomID,string userProfile = ""){
            if(!mState.HaveLogin()) return -1;
            if(mState.HaveInWatch()) return -1;
            var roomJoin = new MsRoomInfo(JoinRoomType.JoinSpecialRoom,MVS.UserID,roomID,MVS.GameID,0,0,0,userProfile);
            _mGTWNw?.send(_mPro.en(CmdId.JoinWatchRoomReq,roomJoin));
            return 0;
        }

        public int joinRoomWithProperty(Dictionary<string,string> property,uint maxPlayer,string userProfile = ""){
            if(!mState.HaveLogin()) return -1;
            if(mState.HaveInRoom()) return -1;
            if(maxPlayer>MVS.Config.MAXPLAYER_LIMIT||maxPlayer<MVS.Config.MINPLAYER_LIMIT) return -20;
            if(userProfile.Length>512) return -21;
            var roomJoin = new MsRoomInfo(JoinRoomType.JoinRoomWithProperty,MVS.UserID,0,MVS.GameID,maxPlayer,0,0,
                userProfile,property);
            mState.SetJoinRooming();
            _mGTWNw?.send(_mPro.en(CmdId.JoinRoomReq,roomJoin));
            return 0;
        }

        public int joinOpen(string userProfile = "",bool isOff = false){
            if(!mState.HaveLogin()) return -1;
            if(!mState.HaveInRoom()) return -1;
            if(userProfile.Length>512) return -21;
            _mGTWNw?.send(_mPro.en(isOff ? CmdId.JoinOverReq:CmdId.JoinOpenReq,MVS.GameID,MVS.RoomID,MVS.UserID,
                StringUtil.toUtf8Array(userProfile)));
            return 0;
        }

        public int joinOver(string userProfile = ""){ return joinOpen(userProfile,true); }

        public int leaveRoom(string userProfile = ""){
//            if(!mState.HaveInRoom()) return -1;
            _mGTWNw?.send(_mPro.en(CmdId.LeaveRoomReq,userProfile));
            return 0;
        }

        public int leaveWatchRoom(string userProfile = ""){
            if(!mState.HaveLogin()) return -1;
            if(!mState.HaveInWatch()) return -1;
            _mGTWNw?.send(_mPro.en(CmdId.LeaveWatchRoomReq,userProfile));
            return 0;
        }

        public int setRoomProperty(ulong roomID,string roomProperty = ""){
            if(!mState.HaveLogin()){ return -1; }

            if(roomProperty.Length==0) return -1;
            if(roomProperty.Length>1024) return -21;
            _mGTWNw?.send(_mPro.en(CmdId.SetRoomPropertyReq,MVS.GameID,roomID,MVS.UserID,
                StringUtil.toUtf8Array(roomProperty)));
            return 0;
        }

        public int getRoomDetail(ulong roomID){
            if(!mState.HaveLogin()){ return -1; }

            _mGTWNw?.send(_mPro.en(CmdId.GetRoomDetailReq,MVS.GameID,roomID));
            return 0;
        }

        public int getRoomListEx(MsRoomFilter filter){
            if(!mState.HaveLogin()){ return -1; }

            if(filter==null){ filter = new MsRoomFilter(); }

            _mGTWNw?.send(_mPro.en(CmdId.RoomListExReq,MVS.GameID,filter));
            return 0;
        }

        public int getWatchRoomList(MsRoomFilter filter){
            if(!mState.HaveLogin()){ return -1; }

            if(filter==null){ filter = new MsRoomFilter(); }

            _mGTWNw?.send(_mPro.en(CmdId.GetWatchRoomsReq,MVS.GameID,filter));
            return 0;
        }

        private void loginConnect(){
            _mGTWNw?.close();
            this._mGTWNw = new NW(MVS.Host.HOST_GATWAY_ADDR,this);
            var buf = this._mPro.en(CmdId.LoginReq,MVS.UserID,MVS.Token,MVS.GameID,MVS.Appkey,MVS.DeviceID);
            this.mState.SetLogin();
            this._mGTWNw.send(buf);
            Log.i("login,userID"+MVS.UserID+", token:"+MVS.Token);
        }

        public void onConnect(string host){ context?.InvokeRepeating("heartBeatServer",5.0f,5.0f); }

        public void onMsg(byte[] buf){
            Pro.He header = _mPro.de(buf);
            if(header==null){ return; }

            Log.d("[INFO] onMsg.header.cmd:{0}",header.cmd);
            switch(header.cmd){
                case (int)CmdId.LoginRsp:
                    LoginRsp loginRsp = (LoginRsp)header.deserialization;
                    if(loginRsp.Status==ErrorCode.Ok){
                        mState.setState(State.LOGIN);
                    }
                    else{
                        Log.w("[WARN] login fail:{0}",loginRsp.Status);
                        mState.clearState(State.LOGIN);
                        return;
                    }

                    MVS.RoomID = (loginRsp.RoomID!=0 ? loginRsp.RoomID:MVS.RoomID);
                    MVS.TeamID = (loginRsp.TeamID!=0 ? loginRsp.TeamID:MVS.TeamID);
                    if(mState.isHaveState(State.RECONNECT)){
                        reconnect();
                    }

                    callback("loginResponse",loginRsp);
                    break;

                case (int)CmdId.LogoutRsp:
                    callback("logoutResponse",header.deserialization);
                    break;
                case (int)CmdId.CreateRoomRsp:
                    connectHotel((CreateRoomRsp)header.deserialization);
                    break;
                case (int)CmdId.JoinRoomRsp:
                    var joinRoomRsp = (JoinRoomRsp)header.deserialization;
                    if(joinRoomRsp.Status!=ErrorCode.Ok){
                        Log.w("[WARN] fail, join room:{0}",joinRoomRsp);
                        mState.clearState(State.ROOM);
                        this.onErr("",(int)joinRoomRsp.Status,"join room fail:"+joinRoomRsp);
                        return;
                    }

                    connectHotel((JoinRoomRsp)header.deserialization);
                    break;

                case (int)CmdId.NoticeUserJoinReq:
                    callback("joinRoomNotify",((NoticeJoin)header.deserialization).User);
                    break;
                case (int)CmdId.JoinOverRsp:
                    callback("joinOverResponse",(JoinOverRsp)header.deserialization);
                    break;
                case (int)CmdId.JoinOverNotify:
                    callback("joinOverNotify",(JoinOverNotify)header.deserialization);
                    break;
                case (int)CmdId.JoinOpenRsp:
                    callback("joinOpenResponse",(JoinOpenRsp)header.deserialization);
                    break;
                case (int)CmdId.JoinOpenNotify:
                    callback("joinOpenNotify",(JoinOpenNotify)header.deserialization);
                    break;
                case (int)CmdId.SetRoomPropertyRsp:
                    callback("setRoomPropertyResponse",(SetRoomPropertyRsp)header.deserialization);
                    break;
                case (int)CmdId.NoticeRoomProperty:
                    callback("setRoomPropertyNotify",(NoticeRoomProperty)header.deserialization);
                    break;
                case (int)CmdId.RoomListExRsp:
                    callback("getRoomListExResponse",(GetRoomListExRsp)header.deserialization);
                    break;
                case (int)CmdId.GetRoomDetailRsp:
                    callback("getRoomDetailResponse",(GetRoomDetailRsp)header.deserialization);
                    break;
                case (int)SDKHotelCmdID.CheckinAckCmdid:
                    if(lastJoinRoomRsp==null){
                        Log.w("[WARN] lastJoinRoomRsp is null when hotel ack");
                        break;
                    }

                    if(lastJoinRoomRsp is JoinRoomRsp){
                        List<PlayerInfo> userList = new List<PlayerInfo>();
                        var d = (JoinRoomRsp)lastJoinRoomRsp;
                        if(d.Users!=null&&d.Users.Count>0){
                            IEnumerator<PlayerInfo> iterator = d.Users.GetEnumerator();
                            while(true){
                                bool result = iterator.MoveNext();
                                if(!result){
                                    break;
                                }

                                userList.Add(iterator.Current);
                            }
                        }

                        callback("joinRoomResponse",(int)d.Status,userList,d.RoomInfo);
                    }
                    else if(lastJoinRoomRsp is CreateRoomRsp){
                        var d = (CreateRoomRsp)lastJoinRoomRsp;
                        callback("createRoomResponse",d);
                    }
                    else if(lastJoinRoomRsp is TeamMatchResultNotify){
                        callback("teamMatchResultNotify",lastJoinRoomRsp);
                    }
                    else{
                        Log.w("[warn] unknown hotel checkin:{0}",lastJoinRoomRsp);
                    }

                    break;


                case (int)SDKHotelCmdID.BroadcastAckCmdid:
                    callback("sendEventResponse",((BroadcastAck)header.deserialization).Status);
                    break;

                case (int)SDKHotelCmdID.NotifyCmdid:
                    Notify r = ((Notify)header.deserialization);
                    callback("sendEventNotify",r.SrcUid,r.CpProto.ToByteArray());
                    break;

                case (int)CmdId.LeaveRoomRsp:
                    _mHNw?.close();
                    _mHNw = null;
                    MVS.RoomID = 0;
                    mState.clearState(State.ROOM);
                    callback("leaveRoomResponse",((LeaveRoomRsp)header.deserialization));
                    break;
                case (int)CmdId.NoticeUserLeaveReq:
                    callback("leaveRoomNotify",((NoticeLeave)header.deserialization));
                    break;
                case (int)SDKHotelCmdID.FrameBroadcastAckCmdid:
                    //
                    callback("sendFrameEventResponse",((FrameBroadcastAck)header.deserialization));
                    break;
                case (int)SDKHotelCmdID.FrameDataNotifyCmdid:
                    frameCache.Enqueue((FrameDataNotify)header.deserialization);
                    break;
                case (int)SDKHotelCmdID.FrameSyncNotifyCmdid:
                    var frameData = new FrameDataNotify[frameCache.Count];
                    for(var i = 0;i<frameCache.Count;i++){
                        frameData[i] = frameCache.Dequeue();
                    }

                    callback("frameUpdate",
                        new MsFrameData(((FrameSyncNotify)header.deserialization).LastIdx,frameData));
                    break;
                case (int)SDKHotelCmdID.SetFrameSyncRateNotifyCmdid:
                    callback("setFrameSyncNotify",((SetFrameSyncRateNotify)header.deserialization));
                    break;
                case (int)SDKHotelCmdID.SetFrameSyncRateAckCmdid:
                    callback("setFrameSyncResponse",((SetFrameSyncRateAck)header.deserialization));
                    break;

                case (int)CmdId.NoticeTeamNetworkState:
                    callback("teamNetworkStateNotify",header.deserialization);
                    break;
                case (int)CmdId.NoticeRoomNetworkState:
                    callback("networkStateNotify",header.deserialization);
                    break;

                //watch
                case (int)CmdId.GetWatchRoomsRsp:
                    callback("getWatchRoomsResponse",(GetWatchRoomsRsp)header.deserialization);
                    break;
                case (int)CmdId.JoinWatchRoomRsp:
                    var rsp = (JoinWatchRoomRsp)header.deserialization;
                    if(rsp.Status==ErrorCode.Ok){
                        connectWatch(rsp);
                    }
                    else{
                        callback("errorResponse",rsp.Status,"join watch room is fail");
                    }

                    break;
                case (int)CmdId.JoinWatchRoomNotify:
                    callback("joinWatchRoomNotify",(JoinWatchRoomNotify)header.deserialization);
                    break;
                case (int)SDKWatchCmdID.EnterLiveRoomAckCmdid:
                    mState.SetWatch();
                    var liveEA = (EnterLiveRoomAck)header.deserialization;
                    callback("joinWatchRoomResponse",liveEA);
                    break;
                case (int)SDKWatchCmdID.EnterLiveRoomNotifyCmdid:
                    callback("enterLiveRoomNotify",(EnterLiveRoomNotify)header.deserialization);
                    break;
                case (int)CmdId.LeaveWatchRoomRsp:
                    _mWNw?.close();
                    _mWNw = null;
                    MVS.WatchRoomID = 0;
                    mState.clearState(State.WATCH);
                    callback("leaveWatchRoomResponse",(LeaveWatchRoomRsp)header.deserialization);
                    break;
                case (int)CmdId.LeaveWatchRoomNotify:
                    callback("leaveWatchRoomNotify",(LeaveWatchRoomNotify)header.deserialization);
                    break;
                case (int)SDKWatchCmdID.ExitLiveRoomNotifyCmdid:
                    callback("exitLiveRoomNotify",(ExitLiveRoomNotify)header.deserialization);
                    break;
                case (int)SDKWatchCmdID.LiveBroadcastAckCmdid:
                    callback("sendLiveEventResponse",(LiveBroadcastAck)header.deserialization);
                    break;
                case (int)SDKWatchCmdID.LiveBroadcastNotifyCmdid:
                    callback("sendLiveEventNotify",(LiveBroadcastNotify)header.deserialization);
                    break;
                case (int)SDKWatchCmdID.LiveFrameDataNotifyCmdid:
                    frameWatchCache.Enqueue((LiveFrameDataNotify)header.deserialization);
                    break;
                case (int)SDKWatchCmdID.LiveFrameSyncNotifyCmdid:
                    var frameLiveData = new LiveFrameDataNotify[frameWatchCache.Count];
                    for(var i = 0;i<frameLiveData.Length;i++){
                        frameLiveData[i] = frameWatchCache.Dequeue();
                    }

                    callback("liveFrameUpdate",(LiveFrameSyncNotify)header.deserialization,frameLiveData);
                    break;
                case (int)SDKWatchCmdID.SetLiveOffsetAckCmdid:
                    callback("setLiveOffsetResponse",(SetLiveOffsetAck)header.deserialization);
                    break;
                case (int)SDKWatchCmdID.LiveOverNotifyCmdid:
                    callback("liveOverNotify",(LiveOverNotify)header.deserialization);
                    break;


                //team
                case (int)CmdId.TeamMatchRsp:
                    callback("teamMatchResponse",(TeamMatchRsp)header.deserialization);
                    break;
                case (int)CmdId.TeamMatchStartNotify:
                    callback("teamMatchStartNotify",(TeamMatchStartNotify)header.deserialization);
                    break;
                case (int)CmdId.TeamMatchResultNotify:
                    var teamMatchResult = (TeamMatchResultNotify)header.deserialization;
                    if(teamMatchResult.Status==ErrorCode.Ok){
                        connectHotel(teamMatchResult);
                        mState.setState(State.ROOM);
                    }
                    else{
                        callback("teamMatchResultNotify",(TeamMatchResultNotify)header.deserialization);
                    }


                    break;
                case (int)CmdId.CancelTeamMatchRsp:
                    callback("cancelTeamMatchResponse",(CancelTeamMatchRsp)header.deserialization);
                    break;
                case (int)CmdId.CancelTeamMatchNotify:
                    callback("cancelTeamMatchNotify",(CancelTeamMatchNotify)header.deserialization);
                    break;
                case (int)CmdId.KickTeamMemberRsp:
                    callback("kickTeamMemberResponse",(KickTeamMemberRsp)header.deserialization);
                    break;
                case (int)CmdId.KickTeamMemberNotify:
                    callback("kickTeamMemberNotify",header.deserialization);
                    break;
                case (int)CmdId.SendTeamEventRsp:
                    callback("sendTeamEventResponse",(SendTeamEventRsp)header.deserialization);
                    break;
                case (int)CmdId.SendTeamEventNotify:
                    callback("sendTeamEventNotify",(SendTeamEventNotify)header.deserialization);
                    break;
                case (int)CmdId.LeaveTeamNotify:
                    callback("leaveTeamNotify",(LeaveTeamNotify)header.deserialization);
                    break;
                case (int)CmdId.LeaveTeamRsp:
                    callback("leaveTeamResponse",(LeaveTeamRsp)header.deserialization);
                    break;
                case (int)CmdId.JoinTeamNotify:
                    callback("joinTeamNotify",(JoinTeamNotify)header.deserialization);
                    break;
                case (int)CmdId.JoinTeamRsp:
                    var joinTeamRsp = (JoinTeamRsp)header.deserialization;
                    callback("joinTeamResponse",joinTeamRsp);
                    if(joinTeamRsp.Status==ErrorCode.Ok){
                        MVS.TeamID = joinTeamRsp.TeamInfo.TeamID;
                    }

                    break;
                case (int)CmdId.SetTeamPropertyRsp:
                    callback("setTeamPropertyResponse",(SetTeamPropertyRsp)header.deserialization);
                    break;
                case (int)CmdId.NoticeTeamProperty:
                    callback("setTeamPropertyNotify",(NoticeTeamProperty)header.deserialization);
                    break;
                case (int)CmdId.SetTeamUserProfileRsp:
                    callback("setTeamUserProfileResponse",(SetTeamUserProfileRsp)header.deserialization);
                    break;
                case (int)CmdId.NoticeTeamUserProfile:
                    callback("setTeamUserProfileNotify",(NoticeTeamUserProfile)header.deserialization);
                    break;
                case (int)CmdId.SetReconnectTimeoutRsp:
                    callback("setReconnectTimeoutResponse",(SetReconnectTimeoutRsp)header.deserialization);
                    break;
                case (int)CmdId.SetTeamReconnectTimeoutRsp:
                    callback("setTeamReconnectTimeoutResponse",(SetTeamReconnectTimeoutRsp)header.deserialization);
                    break;
                case (int)CmdId.CreateTeamRsp:
                    MVS.TeamID = ((CreateTeamRsp)header.deserialization).TeamID;
                    callback("createTeamResponse",header.deserialization);
                    break;
                case (int)CmdId.KickPlayerRsp:
                    callback("kickPlayerResponse",header.deserialization);
                    break;
                case (int)CmdId.KickPlayerNotify:
                    var k = (KickPlayerNotify)header.deserialization;
                    callback("kickPlayerNotify",header.deserialization);
                    if(MVS.UserID==k.UserID){
                        _mHNw?.close();
                    }

                    break;
            }
        }

        private void connectHotel(TeamMatchResultNotify d){
            BookInfo bookInfo = d.BookInfo;
            MVS.Host.HOST_HOTEL_ADDR = MVS.getHotelUrl(bookInfo);
            MVS.RoomID = d.RoomInfo.RoomID;
            this.frameCache = new Queue<FrameDataNotify>();
            this._mHNw = new NW(MVS.Host.HOST_HOTEL_ADDR,this);
            var buf = _mPro.en(SDKHotelCmdID.CheckinCmdid,bookInfo,d.RoomInfo,MVS.UserID,MVS.GameID);
            this._mHNw?.send(buf);
            this.lastJoinRoomRsp = d;
        }

        private void connectHotel(JoinRoomRsp d){
            BookInfo bookInfo = d.BookInfo;
            MVS.Host.HOST_HOTEL_ADDR = MVS.getHotelUrl(bookInfo);
            MVS.RoomID = d.RoomInfo.RoomID;
            this.frameCache = new Queue<FrameDataNotify>();
            this._mHNw = new NW(MVS.Host.HOST_HOTEL_ADDR,this);
            var buf = _mPro.en(SDKHotelCmdID.CheckinCmdid,bookInfo,d.RoomInfo,MVS.UserID,MVS.GameID);
            this._mHNw?.send(buf);
            this.lastJoinRoomRsp = d;
        }

        private void connectHotel(CreateRoomRsp d){
            BookInfo bookInfo = d.BookInfo;
            MVS.Host.HOST_HOTEL_ADDR = MVS.getHotelUrl(bookInfo);
            MVS.RoomID = d.RoomID;
            this.frameCache = new Queue<FrameDataNotify>();
            this._mHNw = new NW(MVS.Host.HOST_HOTEL_ADDR,this);
            var r = new RoomInfo();
            r.RoomID = MVS.RoomID;
            var buf = _mPro.en(SDKHotelCmdID.CheckinCmdid,bookInfo,r,MVS.UserID,MVS.GameID);
            this._mHNw?.send(buf);
            this.lastJoinRoomRsp = d;
        }

        private void connectWatch(JoinWatchRoomRsp d){
            BookInfo bookInfo = d.BookInfo;
            MVS.Host.HOST_WATCH_ADDR = MVS.getHotelUrl(bookInfo);
            MVS.WatchRoomID = d.RoomID;
            frameWatchCache = new Queue<LiveFrameDataNotify>();
            _mWNw = new NW(MVS.Host.HOST_WATCH_ADDR,this);
            var buf = _mPro.en(SDKWatchCmdID.EnterLiveRoomCmdid,bookInfo,d.RoomID,MVS.UserID,MVS.GameID,MVS.NodeID);
            _mWNw?.send(buf);
            lastJoinWatchRoomRsp = d;
        }

        public void onErr(string host,int errCode,string errMsg){
            if(errCode==1006){
                Log.w("connect.onErr:  code:{0},msg:{1}",host,errCode,errMsg);
            }

            if(errCode>=1000){
                if(host==MVS.Host.HOST_HOTEL_ADDR){
                    _mHNw = null;
                    mState.clearState(State.ROOM);
                    Log.d("[INFO] [disconnect] hotel:{0}",host);
                }

                if(host==MVS.Host.HOST_WATCH_ADDR){
                    _mWNw = null;
                    mState.clearState(State.WATCH);
                    Log.d("[INFO] [disconnect] watch :{0}",host);
                }

                if(host==MVS.Host.HOST_GATWAY_ADDR){
                    _mGTWNw = null;
                    mState.clearState(State.LOGIN);

                    _mHNw?.close();
                    _mHNw = null;
                    mState.clearState(State.ROOM);

                    _mWNw?.close();
                    _mWNw = null;
                    mState.clearState(State.WATCH);

                    Log.d("[INFO] [disconnect] gateway:{0}",host);
                }
            }

            callback("errorResponse",errCode,errMsg);
        }

        private void callback(string listener,params object[] args){
            if(mRsp.ContainsKey(listener)){
                try{
                    mRsp[listener].DynamicInvoke(args);
                }
                catch(Exception e){
                    Log.e("callback.{0},    args:{1} ,    exception:{2}",listener,args,e.Message);
                }
            }
            else{
                Log.w("Not found listener : {0}",listener);
            }
        }


        /**
         * 在房间内广播消息,默认不发送给自己
         * destUserIDArray 接受消息的用户ID数组.
         * msgType 0-只发client 1-只发gs 2-client&gs
         * destType  0:include,1:exclude, other:ignore the destUserIDArray
         */
        private int sendEvent0(byte[] data,int cmd = (int)SDKHotelCmdID.BroadcastCmdid,uint[] destUserIDArray = null,
            int dstType = 1,int msgType = 0,ulong teamID = 0){
            if(data.Length>1024) return -21;
            if(destUserIDArray==null){
                destUserIDArray = new uint[]{ };
            }

            if(cmd==(int)CmdId.SendTeamEventReq){
                var buf = _mPro.en(cmd,teamID,destUserIDArray,dstType,msgType,data,MVS.GameID,MVS.UserID);
                _mGTWNw?.send(buf);
            }
            else{
                var buf = _mPro.en(cmd,MVS.RoomID,destUserIDArray,dstType,msgType,data,MVS.GameID);
                _mHNw?.send(buf);
            }

            return _mPro.s;
        }

        public int sendEvent(byte[] data,uint[] destUserIDArray = null,int destType = 1,int msgType = 0){
            if(!mState.HaveInRoom()) return -1;
            return sendEvent0(data,(int)SDKHotelCmdID.BroadcastCmdid,destUserIDArray,destType,msgType);
        }

        public int sendEvent(string str){ return sendEvent(StringUtil.toUtf8Array(str)); }

        public int sendTeamEvent(byte[] data,uint[] destUserIDArray = null,
            TeamDstType dstType = TeamDstType.DstTypeExclusive,TeamMsgType msgType = TeamMsgType.MsgTypeSdk1Gs0){
            if(!mState.HaveLogin()) return -1;
            return sendEvent0(data,(int)CmdId.SendTeamEventReq,destUserIDArray,(int)dstType,(int)msgType,MVS.TeamID);
        }

        public int sendTeamEvent(string str){ return sendTeamEvent(StringUtil.toUtf8Array(str)); }


        public int sendLiveEvent(byte[] data,uint[] destUserIDArray = null,int destType = 1,int msgType = 0){
            if(!mState.HaveInWatch()) return -1;
            return sendEvent0(data,(int)SDKWatchCmdID.LiveBroadcastCmdid,destUserIDArray,destType,(int)msgType);
        }

        public int sendLiveEvent(string str){ return sendLiveEvent(StringUtil.toUtf8Array(str)); }

        /**
         * @param frameRate ex:10/s . = 0 is off,>0 is on
         * @param {number} enableGS 0-gs不参与帧同步 1-gs参与帧同步
         * @param {number} cacheFrameMS 缓存帧的毫秒数 0 为不开启缓存功能；-1 为缓存所有数据
         */
        public int setFrameSync(uint frameRate = 2,bool enableGS = false,int cacheFrameMS = 0){
            if(!mState.HaveInRoom()) return -1;
            if(frameRate>20||frameRate<0) return -20;
            if(cacheFrameMS>600000) return -25;
            var buf = _mPro.en(SDKHotelCmdID.SetFrameSyncRateCmdid,MVS.GameID,MVS.RoomID,(uint)0,frameRate,
                (uint)(enableGS ? 1:0),cacheFrameMS);
            _mHNw?.send(buf);
            return 0;
        }

        /**
         * 打开观战，设置从哪个时间段开始获取观战数据
         * @param offsetMS {number} -1 表示从头， 0 表示不追 >0 表示最近多少ms
         */
        public int setLiveOffset(int offsetMS = -1){
            if(!mState.HaveInWatch()) return -1;
            _mWNw?.send(_mPro.en(SDKWatchCmdID.SetLiveOffsetCmdid,MVS.GameID,MVS.WatchRoomID,MVS.UserID,offsetMS));
            return 0;
        }

        public int sendFrameEvent(byte[] data,FrameOpt opt = FrameOpt.ONLY_CLIENT){
            if(!mState.HaveInRoom()) return -1;
            if(data.Length>1024) return -21;
            var buf = _mPro.en(SDKHotelCmdID.FrameBroadcastCmdid,MVS.RoomID,(uint)0,data,(int)opt);
            this._mHNw?.send(buf);
            return 0;
        }

        public int sendFrameEvent(string str){ return sendFrameEvent(StringUtil.toUtf8Array(str)); }


        public void listen(string methodName,Delegate func){ mRsp.Add(methodName,func); }

        public void registerUser(MatchvsResponse.registerUserResponse action){
            string url = MVS.Host.VS_OPEN_URL+MVS.Host.REGISTERUSER+"?mac=0"+"&deviceid="+MVS.DeviceID+"&channel="+
                         MVS.Channel+"&pid=13"+"&version="+1;
            var httpRequest = new HTTPRequest(new Uri(url),(HTTPRequest req,HTTPResponse rsp)=>{
                if(rsp.IsSuccess){
                    Log.i("registerUser:{0}",rsp.DataAsText);
                    JsonData jsonData = JsonMapper.ToObject(rsp.DataAsText);//这里的JsonFile.txt文件即为上面1中的文本文件

                    int status = int.Parse(jsonData["status"].ToString());
                    if(status==0){
                        var data = jsonData["data"].ToJson();
                        MsUser user = JsonMapper.ToObject<MsUser>(data);
                        action(user);
                        return;
                    }
                }
                else{
                    Log.w("registerUser fail: {0}",rsp.Message);
                }

                action(null);
            });
            httpRequest.MethodType = HTTPMethods.Post;
            httpRequest.Send();
        }

        public int createTeam(TeamInfo teamInfo = null,string userProfile = ""){
            if(mState.HaveInRoom()) return -1;
            if(userProfile.Length>512){
                return -21;
            }

            if(teamInfo==null){
                teamInfo = new TeamInfo();
                teamInfo.Capacity = 2;
                teamInfo.Password = "";
                teamInfo.Mode = 0;
                teamInfo.Visibility = 1;
            }

            MVS.PassWord = teamInfo.Password;
            _mGTWNw?.send(_mPro.en(CmdId.CreateTeamReq,MVS.UserID,userProfile,MVS.GameID,teamInfo));
            return 0;
        }

        public int joinTeam(ulong teamID,string password = "",string userProfile = "",bool isReconnect = false){
            if(mState.HaveInRoom()) return -1;
            if(!mState.HaveLogin()) return -1;
            if(userProfile.Length>512){
                return -21;
            }

            if(teamID==0){
                Log.w("[WARN] teamID is 0  :{0}",teamID);
                return -21;
            }

            MVS.TeamID = teamID;
            MVS.PassWord = password;
            _mGTWNw?.send(_mPro.en(CmdId.JoinTeamReq,MVS.UserID,userProfile,MVS.GameID,teamID,password,
                isReconnect ? JoinTeamType.RejoinTeam:JoinTeamType.JoinSpecialTeam));
            return 0;
        }

        public int leaveTeam(){
            if(!mState.HaveLogin()) return -1;
            _mGTWNw?.send(_mPro.en(CmdId.LeaveTeamReq,MVS.GameID,MVS.UserID,MVS.TeamID));
            return 0;
        }

        public int setTeamProperty(string teamProperty = ""){
            if(!mState.HaveLogin()) return -1;

            _mGTWNw?.send(_mPro.en(CmdId.SetTeamPropertyReq,MVS.GameID,MVS.TeamID,MVS.UserID,teamProperty));
            return 0;
        }

        public int setTeamUserProfile(string userProfile){
            if(!mState.HaveLogin()) return -1;
            _mGTWNw?.send(_mPro.en(CmdId.SetTeamUserProfileReq,MVS.GameID,MVS.TeamID,MVS.UserID,userProfile));
            return 0;
        }

        public int teamMatch(MsRoomInfo info = null,TeamMatchCond cond = null){
            if(mState.HaveInRoom()) return -1;
            if(info==null){
                info = new MsRoomInfo(JoinRoomType.NoJoin,MVS.UserID,0,MVS.GameID);
            }

            if(cond==null){
                cond = new TeamMatchCond();
                cond.Full = 0;//是否人满匹配，0-人不满也可以匹配，1-人满匹配 (人不满匹配不到会超时报422错误码)
                cond.Timeout = 15;
                cond.TeamNum = 2;
                cond.TeamMemberNum = 2;
                cond.Weight = 0;
                cond.WeightRange = 0;
                cond.WeightRule = 0;
            }

            _mGTWNw?.send(_mPro.en(CmdId.TeamMatchReq,MVS.GameID,MVS.UserID,MVS.TeamID,cond,info));
            return 0;
        }

        public int cancelTeamMatch(string cancelInfo = ""){
            if(mState.HaveInRoom()) return -1;
            _mGTWNw?.send(_mPro.en(CmdId.CancelTeamMatchReq,MVS.GameID,MVS.UserID,MVS.TeamID,cancelInfo));
            return 0;
        }

        public int kickTeamMember(uint kickUserID,string cancelInfo = ""){
            if(mState.HaveInRoom()) return -1;
            _mGTWNw?.send(_mPro.en(CmdId.KickTeamMemberReq,MVS.GameID,MVS.UserID,MVS.TeamID,cancelInfo,kickUserID));
            return 0;
        }

        public int setReconnectTimeout(int second = -1){
            if(!mState.HaveLogin()) return -1;
            _mGTWNw?.send(_mPro.en(CmdId.SetReconnectTimeoutReq,MVS.UserID,second));
            return 0;
        }

        public int setTeamReconnectTimeout(int second = -1){
            if(!mState.HaveLogin()) return -1;
            _mGTWNw?.send(_mPro.en(CmdId.SetTeamReconnectTimeoutReq,MVS.UserID,second));
            return 0;
        }

        public int kickPlayer(uint kickUserID,string why = ""){
            if(!mState.HaveInRoom()) return -1;
            _mGTWNw?.send(_mPro.en(CmdId.KickPlayerReq,MVS.GameID,MVS.UserID,MVS.RoomID,why,kickUserID));
            return 0;
        }
    }
}