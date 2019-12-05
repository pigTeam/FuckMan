using UnityEngine;
using System;
using Matchvs;
using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine.UI;

public class Demo:MonoBehaviour{
    private LinkedList<string> Text = new LinkedList<string>();

    public void startMatch(){
        MatchvsResponse.initResponse initResponse = (int errCode)=>{
            appendLog("initResponse{0}",errCode);
            MatchvsEngine.getInstance().login(Config.userID,Config.token);
            appendLog("login, userID:{0},token:{1}",Config.userID,Config.token);
        };

        MatchvsResponse.loginResponse loginResponse = (LoginRsp rsp)=>{
            if(rsp.RoomID==0){
                appendLog("loginResponse:{0}",rsp);
                appendLog("auto joinRandomRoom,  MaxPlayer:{0}",Config.MaxPlayer);
                MatchvsEngine.getInstance().setReconnectTimeout(59);
                MatchvsEngine.getInstance().setTeamReconnectTimeout(59);
            }
            else{
                appendLog(" login with reconnect info,  last roomID:{0},teamID:{1}\n",rsp.RoomID,rsp.TeamID);
            }
        };

        MatchvsEngine.getInstance().listen("logoutResponse",(MatchvsResponse.logoutResponse)(u=>{
            appendLog("logoutResponse: {0}",u);
        }));

        MatchvsResponse.errorResponse errorResponse = (int errCode,string errMsg)=>{
            appendLog("errorResponse....   {0}:{1}.\n",errCode,errMsg);
        };


        MatchvsResponse.joinRoomResponse joinRoomResponse =
            (int status,List<PlayerInfo> roomUserInfoList,RoomInfo roomInfo)=>{
                appendLog("joinRoomResponse: roomID:{0},userList.size:{1}",roomInfo.RoomID,roomUserInfoList.Count);
                roomUserInfoList.ForEach((player)=>{
                    appendLog("          =======  user:{0}({1})",player.UserID,player.UserProfile);
                });
                if(roomUserInfoList.Count<=1){
                    MatchvsEngine.getInstance().setFrameSync(Config.FrameRate);
                    appendLog("setFrameSync,frameRate: {0}",Config.FrameRate);
                }

                MatchvsEngine.getInstance().getRoomDetail(roomInfo.RoomID);
                MatchvsEngine.getInstance().getRoomListEx(new MsRoomFilter());
                MatchvsEngine.getInstance().setRoomProperty(roomInfo.RoomID,"UnityDemoRoom");
                MatchvsEngine.getInstance().getWatchRoomList(new MsRoomFilter());
            };
        MatchvsResponse.joinRoomNotify joinRoomNotify = (PlayerInfo roomUserInfo)=>{
            appendLog("joinRoomNotify{0}",roomUserInfo.UserID);
            Config.otherUserID = roomUserInfo.UserID;
        };

        MatchvsResponse.sendEventResponse sendEventResponse = (uint r)=>{
            appendLog("sendEventResponse.states:{0}",r);
        };
        MatchvsResponse.sendEventNotify sendEventNotify = (uint u,byte[] data)=>{
            appendLog("recv msg from userid '{0}' :{1}",u,StringUtil.fromUtf8Array(data));
        };

        MatchvsResponse.leaveRoomResponse leaveRoomResponse = (LeaveRoomRsp u)=>{
            appendLog("leaveRoomResponse {0}",u);
        };

        MatchvsResponse.leaveRoomNotify leaveRoomNotify = (NoticeLeave u)=>{
            appendLog("leaveRoomNotify {0},{1}",u.UserID,u);
        };
        MatchvsResponse.setFrameSyncResponse setFrameSyncResponse = (SetFrameSyncRateAck u)=>{
            appendLog("setFrameSyncResponse {0},{1}",u.Status,u);
        };
        MatchvsResponse.setFrameSyncNotify setFrameSyncNotify = (SetFrameSyncRateNotify u)=>{
            appendLog("setFrameSyncNotify {0},{1}",u.FrameRate,u);
        };
        MatchvsResponse.frameUpdate frameUpdate = (MsFrameData u)=>{
            appendLog("frameUpdate.frameIndex: {0}, data.count:{1}",u.frameIndex,u.frameItems.Length);
        };
        MatchvsEngine.getInstance().listen("sendFrameEventResponse",(MatchvsResponse.sendFrameEventResponse)((u)=>{
            appendLog("sendFrameEventResponse: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("loginResponse",loginResponse);
        MatchvsEngine.getInstance().listen("errorResponse",errorResponse);
        MatchvsEngine.getInstance().listen("initResponse",initResponse);

        MatchvsEngine.getInstance().listen("joinRoomResponse",joinRoomResponse);
        MatchvsEngine.getInstance().listen("joinRoomNotify",joinRoomNotify);
        MatchvsEngine.getInstance().listen("leaveRoomResponse",leaveRoomResponse);
        MatchvsEngine.getInstance().listen("leaveRoomNotify",leaveRoomNotify);

        MatchvsEngine.getInstance().listen("sendEventResponse",sendEventResponse);
        MatchvsEngine.getInstance().listen("sendEventNotify",sendEventNotify);
        MatchvsEngine.getInstance().listen("setFrameSyncResponse",setFrameSyncResponse);
        MatchvsEngine.getInstance().listen("frameUpdate",frameUpdate);


        MatchvsEngine.getInstance().listen("setRoomPropertyResponse",(MatchvsResponse.setRoomPropertyResponse)(u=>{
            appendLog("setRoomPropertyResponse: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("setRoomPropertyNotify",(MatchvsResponse.setRoomPropertyNotify)(u=>{
            appendLog("setRoomPropertyNotify: {0}",u);
        }));

        MatchvsEngine.getInstance().listen("getRoomListExResponse",(MatchvsResponse.getRoomListExResponse)(u=>{
            appendLog("getRoomListExResponse: {0}",u);
        }));

        MatchvsEngine.getInstance().listen("getRoomDetailResponse",(MatchvsResponse.getRoomDetailResponse)(u=>{
            appendLog("getRoomDetailResponse: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("teamNetworkStateNotify",(MatchvsResponse.teamNetworkStateNotify)(u=>{
            appendLog("teamNetworkStateNotify: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("networkStateNotify",(MatchvsResponse.networkStateNotify)(u=>{
            appendLog("networkStateNotify: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("getWatchRoomsResponse",(MatchvsResponse.getWatchRoomsResponse)(u=>{
            if(u.RoomInfoEx.Count>0){
                var iterator = u.RoomInfoEx.GetEnumerator();
                iterator.MoveNext();
                Config.WatchRoomID = iterator.Current.RoomID;
                appendLog("select a watch roomID: {0}",Config.WatchRoomID);
            }

            appendLog("getWatchRoomsResponse: {0}",u);
            Log.i("[INFO] getWatchRoomList:{0}",u);
        }));
        MatchvsEngine.getInstance().listen("setFrameSyncNotify",(MatchvsResponse.setFrameSyncNotify)(u=>{
            appendLog("setFrameSyncNotify: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("leaveWatchRoomResponse",(MatchvsResponse.leaveWatchRoomResponse)(u=>{
            appendLog("leaveWatchRoomResponse: {0}",u);
            Log.i("leaveWatchRoomResponse: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("leaveWatchRoomNotify",(MatchvsResponse.leaveWatchRoomNotify)(u=>{
            appendLog("leaveWatchRoomNotify: {0}",u);
            Log.i("leaveWatchRoomNotify: {0}",u);
        }));

        MatchvsEngine.getInstance().listen("joinWatchRoomResponse",(MatchvsResponse.joinWatchRoomResponse)(u=>{
            appendLog("joinWatchRoomResponse: {0}",u);
            Log.i("[INFO] joinWatchRoomResponse:{0}",u);

            MatchvsEngine.getInstance().setLiveOffset();
        }));
        MatchvsEngine.getInstance().listen("joinWatchRoomNotify",(MatchvsResponse.joinWatchRoomNotify)(u=>{
            appendLog("joinWatchRoomNotify: {0}",u);
            Log.i("joinWatchRoomNotify: {0}",u);
        }));

        MatchvsEngine.getInstance().listen("enterLiveRoomNotify",(MatchvsResponse.enterLiveRoomNotify)(u=>{
            appendLog("enterLiveRoomNotify: {0}",u);
            Log.i("enterLiveRoomNotify: {0}",u);
        }));

        MatchvsEngine.getInstance().listen("exitLiveRoomNotify",(MatchvsResponse.exitLiveRoomNotify)(u=>{
            appendLog("exitLiveRoomNotify: {0}",u);
            Log.i("exitLiveRoomNotify: {0}",u);
        }));

        MatchvsEngine.getInstance().listen("liveFrameUpdate",(MatchvsResponse.liveFrameUpdate)((u,v)=>{
            appendLog("liveFrameUpdate: {0} data.len:{1}",u,v.Length);
        }));

        MatchvsEngine.getInstance().listen("setLiveOffsetResponse",(MatchvsResponse.setLiveOffsetResponse)((u)=>{
            appendLog("setLiveOffsetResponse: {0}",u);
        }));


        MatchvsEngine.getInstance().listen("joinOpenNotify",(MatchvsResponse.joinOpenNotify)(u=>{
            appendLog("joinOpenNotify: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("joinOpenResponse",(MatchvsResponse.joinOpenResponse)(u=>{
            appendLog("joinOpenResponse: {0}",u);
        }));

        MatchvsEngine.getInstance().listen("joinOverResponse",(MatchvsResponse.joinOverResponse)(u=>{
            appendLog("joinOverResponse: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("joinOverNotify",(MatchvsResponse.joinOverNotify)(u=>{
            appendLog("joinOverNotify: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("createRoomResponse",(MatchvsResponse.createRoomResponse)(u=>{
            appendLog("createRoomResponse: {0}",u);
            Log.i("[INFO] roomID:{0}",u.RoomID);
        }));


        MatchvsEngine.getInstance().listen("leaveTeamResponse",(MatchvsResponse.leaveTeamResponse)(u=>{
            appendLog("leaveTeamResponse: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("leaveTeamNotify",(MatchvsResponse.leaveTeamNotify)(u=>{
            appendLog("leaveTeamNotify: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("joinTeamResponse",(MatchvsResponse.joinTeamResponse)(u=>{
            appendLog("joinTeamResponse: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("joinTeamNotify",(MatchvsResponse.joinTeamNotify)(u=>{
            appendLog("joinTeamNotify: {0}",u);
            Config.otherUserID = u.User.UserID;
        }));
        MatchvsEngine.getInstance().listen("createTeamResponse",(MatchvsResponse.createTeamResponse)(u=>{
            appendLog("createTeamResponse: {0}",u);
            Log.i("[INFO] teamID:{0}",u.TeamID);
            MatchvsEngine.getInstance().setTeamProperty("team property");
            MatchvsEngine.getInstance().setTeamUserProfile("team user profile");
        }));
        MatchvsEngine.getInstance().listen("teamMatchResponse",(MatchvsResponse.teamMatchResponse)(u=>{
            appendLog("teamMatchResponse: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("teamMatchStartNotify",(MatchvsResponse.teamMatchStartNotify)(u=>{
            appendLog("teamMatchStartNotify: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("teamMatchResultNotify",(MatchvsResponse.teamMatchResultNotify)(u=>{
            appendLog("teamMatchResultNotify: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("sendTeamEventNotify",(MatchvsResponse.sendTeamEventNotify)(u=>{
            appendLog("sendTeamEventNotify: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("sendTeamEventResponse",(MatchvsResponse.sendTeamEventResponse)(u=>{
            appendLog("sendTeamEventResponse: {0}",u);
        }));


        MatchvsEngine.getInstance().listen("setTeamReconnectTimeoutResponse",
            (MatchvsResponse.setTeamReconnectTimeoutResponse)(u=>{
                appendLog("setTeamReconnectTimeoutResponse: {0}",u);
            }));
        MatchvsEngine.getInstance().listen("setReconnectTimeoutResponse",
            (MatchvsResponse.setReconnectTimeoutResponse)(u=>{
                appendLog("setReconnectTimeoutResponse: {0}",u);
            }));
        MatchvsEngine.getInstance().listen("setTeamUserProfileResponse",
            (MatchvsResponse.setTeamUserProfileResponse)(u=>{
                appendLog("setTeamUserProfileResponse: {0}",u.UserProfile.ToStringUtf8());
            }));
        MatchvsEngine.getInstance().listen("setTeamUserProfileNotify",(MatchvsResponse.setTeamUserProfileNotify)(u=>{
            appendLog("setTeamUserProfileNotify: {0}",u.UserProfile.ToStringUtf8());
        }));
        MatchvsEngine.getInstance().listen("setTeamPropertyResponse",(MatchvsResponse.setTeamPropertyResponse)(u=>{
            appendLog("setTeamPropertyResponse: {0}",u.TeamProperty.ToStringUtf8());
        }));
        MatchvsEngine.getInstance().listen("setTeamPropertyNotify",(MatchvsResponse.setTeamPropertyNotify)(u=>{
            appendLog("setTeamPropertyNotify: {0}",u.TeamProperty.ToStringUtf8());
        }));

        MatchvsEngine.getInstance().listen("kickTeamMemberResponse",(MatchvsResponse.kickTeamMemberResponse)(u=>{
            appendLog("kickTeamMemberResponse: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("kickTeamMemberNotify",(MatchvsResponse.kickTeamMemberNotify)(u=>{
            appendLog("kickTeamMemberNotify: {0}",u);
        }));


        MatchvsEngine.getInstance().listen("kickPlayerResponse",(MatchvsResponse.kickPlayerResponse)(u=>{
            appendLog("kickPlayerResponse: {0}",u);
        }));
        MatchvsEngine.getInstance().listen("kickPlayerNotify",(MatchvsResponse.kickPlayerNotify)(u=>{
            appendLog("kickPlayerNotify: {0}",u);
        }));


        MatchvsEngine.getInstance().init(gameObject.GetComponent<UnityContext>(),Config.GameID,Config.AppKey);


        appendLog("init, GameID:{0},AppKey:{1}",Config.GameID,Config.AppKey);
    }

    public void stopMatch(){
        appendLog("unInit");
        MatchvsEngine.getInstance().unInit();
    }

    private void appendLog(string msg,params object[] msg1){
        Text.AddFirst(string.Format(msg,msg1));
        if(Text.Count>MaxLogLineCount){
            Text.RemoveLast();
        }

        string temp = "";
        foreach(var item in Text){
            temp += $"[{DateTime.Now}]: {item} \n";
        }

        GameObject.Find("Log").GetComponent<Text>().text = temp;
    }

    public int MaxLogLineCount = 20;

    public void send(string msg){
        MatchvsEngine.getInstance().sendEvent(msg);
        appendLog("send->"+msg);
    }

    public void sendFrameEvent(string msg){
        MatchvsEngine.getInstance().sendFrameEvent(msg);
        appendLog("sendFrameEvent->"+msg);
    }


    public void registerUser(){
        MatchvsEngine.getInstance().registerUser((MsUser user)=>{
            Config.token = user.token;
            Config.userID = user.userid;
            appendLog("register.user:{0}",user);
        });
    }
}