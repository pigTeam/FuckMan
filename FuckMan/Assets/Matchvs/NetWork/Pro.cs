using System;
using BestHTTP.WebSocket;
using System.Text;
using Matchvs;
using System.IO;
using MiscUtil.IO;
using MiscUtil.Conversion;
using Google.Protobuf;
using System.Collections.Generic;
using Google.Protobuf.Collections;

namespace Matchvs{
    public class Pro{
        public int s = 0;

        private delegate byte[] Er(params object[] arguments);

        private delegate He Dr(He he);

        private Dictionary<int,Er> mE = new Dictionary<int,Er>();

        private Dictionary<int,Dr> mD = new Dictionary<int,Dr>();


        private int F = 16;
        private int V = 2;
        private uint u = 0;

        public class He{
            public int size = 0;
            public int seq = 0;
            public int cmd = 0;
            public int version = 0;
            public int userID = 0;
            public byte[] data;
            public object deserialization;

            public string toString(){
                return "[size   ]: "+this.size+" [seq    ]:"+this.seq+" [cmd    ]:"+this.cmd+" [version]:"+this.version+
                       " [userID ]:"+this.userID;
            }
        }

        public Pro(){ ini(); }

        private byte[] fHe(byte[] dataArray,int cmd,bool isLoginReq = false){
            var buffer = new MemoryStream(F+dataArray.Length);
            EndianBinaryWriter dataView = new EndianBinaryWriter(EndianBitConverter.Little,buffer);
            dataView.Write(F+dataArray.Length);//size; +4
            dataView.Write(s++);//seq +4
            dataView.Write((short)cmd);//cmd; +2
            if(isLoginReq){
                dataView.Write((short)3);//version +2
            }
            else{
                dataView.Write((short)V);//version +2
            }

            dataView.Write(u);//userID +4

            dataView.Write(dataArray);
            dataView.Flush();
            buffer.Flush();

            //to byte
            byte[] bytes = new byte[buffer.Length];
            buffer.Seek(0,SeekOrigin.Begin);
            buffer.Read(bytes,0,bytes.Length);

            buffer.Close();
            dataView.Close();

            return bytes;
        }

        private He parseHeader(byte[] msg){
            EndianBinaryReader dataView = new EndianBinaryReader(EndianBitConverter.Little,new MemoryStream(msg));
            var head = new He();
            head.size = dataView.ReadInt32();//size; +4
            head.seq = dataView.ReadInt32();//seq +4
            head.cmd = dataView.ReadInt16();//cmd; +2
            head.version = dataView.ReadInt16();//version +2
            head.userID = dataView.ReadInt32();//userID +4
            if(head.cmd!=(int)CmdId.HeartBeatReq&&head.cmd!=(int)SDKHotelCmdID.HeartbeatAckCmdid&&
               head.cmd!=(int)SDKHotelCmdID.BroadcastAckCmdid&&head.cmd!=(int)SDKHotelCmdID.NotifyCmdid&&
               head.cmd!=(int)SDKHotelCmdID.FrameDataNotifyCmdid&&head.cmd!=(int)SDKHotelCmdID.FrameSyncNotifyCmdid&&
               head.cmd!=(int)SDKWatchCmdID.LiveHeartbeatAckCmdid&&head.cmd!=(int)SDKWatchCmdID.LiveBroadcastAckCmdid&&
               head.cmd!=(int)SDKWatchCmdID.LiveBroadcastNotifyCmdid&&
               head.cmd!=(int)SDKWatchCmdID.LiveFrameDataNotifyCmdid&&
               head.cmd!=(int)SDKWatchCmdID.LiveFrameSyncNotifyCmdid){
                Log.i("parseHeader,cmd:{0}",head.cmd);
            }

            return head;
        }

        public He de(byte[] msg){
            He head = null;
            try{
                 head = parseHeader(msg);
                if(head!=null&&head.size>F){
                    head.data = new byte[head.size-F];
                    Array.Copy(msg,F,head.data,0,head.size-F);
                }

                if(mD.ContainsKey(head.cmd)){
                    Log.info("[Decode] Decoders cmd ,[{0}]",head.cmd);
                    return mD[head.cmd](head);
                }
                else{
                    Log.warn("[WARN] not found encode,{0}",head.cmd);
                }
            }
            catch(Exception e){
                Log.e("[ERROR] decode cmd:{0}, case:{1},",head?.cmd,e.Message);
            }

            return null;
        }

        public byte[] en(params object[] arguments){
            var cmd = (int)arguments[0];
            Log.d("req.cmd:{0},arguments.size:{1}",cmd,arguments.Length);
            if(mE.ContainsKey(cmd)){
                //arguments = Arrays.splice(arguments, 1, arguments.Length);
                return mE[cmd](arguments);
            }
            else{
                Log.e("Not Found Encoder For cmd:{0} ",cmd);
            }

            return null;
        }

        private ByteString tbs(string str){ return ByteString.CopyFrom(StringUtil.toUtf8Array(str)); }

        private void ini(){
            //login
            mD[(int)CmdId.LoginRsp] = (He header)=>{
                LoginRsp rsp = (LoginRsp)LoginRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("LoginRsp.Status:{0}->{1}",rsp.Status,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };
            mD[(int)CmdId.LogoutRsp] = (He header)=>{
                var rsp = (LogoutRsp)LogoutRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("LogoutRsp.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            mE[(int)CmdId.LoginReq] = (object[] arguments)=>{
                u = (uint)arguments[1];
                LoginReq req = new LoginReq();
                req.Token = MD5.MD5Encrypt((string)arguments[2]);
                req.GameID = (uint)arguments[3];
                req.AppKey = (string)arguments[4];
                req.DeviceID = (string)arguments[5];
                req.SdkVer = "3";

                req.Sign = MD5.MD5Encrypt(string.Format("{0}&UserID={1}&GameID={2}&VersionSdk={3}&{4}",req.AppKey,
                    u,req.GameID,3,req.Token));
                return fHe(req.ToByteArray(),(int)arguments[0],true);
            };

            mE[(int)CmdId.LogoutReq] = (object[] arguments)=>{
                return fHe(StringUtil.toUtf8Array((string)arguments[1]),(int)arguments[0]);
            };


            //heartbeat

            mE[(int)CmdId.HeartBeatReq] = (object[] arguments)=>{
                HeartbeatReq req = new HeartbeatReq();
                req.GameID = (uint)arguments[1];
                req.RoomID = (ulong)arguments[2];
                return fHe(req.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)CmdId.HeartBeatReq] = (He header)=>{
                return header;
            };

            //hotel heartbeat
            mE[(int)SDKHotelCmdID.HeartbeatCmdid] = (object[] arguments)=>{
                Heartbeat req = new Heartbeat();
                req.GameID = (uint)arguments[1];
                req.RoomID = (ulong)arguments[2];
                req.UserID = (uint)arguments[3];

                return fHe(req.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)SDKHotelCmdID.HeartbeatAckCmdid] = (He header)=>{
                HeartbeatAck rsp = (HeartbeatAck)HeartbeatAck.Descriptor.Parser.ParseFrom(header.data);
                Log.d("rsp.Status:{0}->{1}",rsp.Status,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };



            //createRoom
            mE[(int)CmdId.CreateRoomReq] = (object[] arguments)=>{
                MsRoomInfo p = (MsRoomInfo)arguments[1];

                PlayerInfo pi = new PlayerInfo();
                pi.UserID = p.userID;
                pi.UserProfile = tbs(p.userProfile);


                RoomInfo ri = new RoomInfo();
                ri.MaxPlayer = p.maxPlayer;
                ri.CanWatch = p.canWatch;
                ri.Mode = p.mode;
                ri.Visibility = p.visibility;
                ri.RoomName = p.roomName;
                ri.RoomProperty = tbs(p.roomProperty);

                CreateRoom req = new CreateRoom();
                req.PlayerInfo = pi;
                req.RoomInfo = ri;
                req.GameID = p.gameID;
                req.WatchSetting = p.watchSetting;


                return fHe(req.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)CmdId.CreateRoomRsp] = (He header)=>{
                CreateRoomRsp rsp = (CreateRoomRsp)CreateRoomRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("CreateRoomRsp.Status:{0}->{1}",rsp.Status,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };


            //joinRoom
            mE[(int)CmdId.JoinRoomReq] = (object[] arguments)=>{
                MsRoomInfo p = (MsRoomInfo)arguments[1];

                PlayerInfo pi = new PlayerInfo();
                pi.UserID = p.userID;
                pi.UserProfile = tbs(p.userProfile);


                RoomInfo ri = new RoomInfo();
                ri.MaxPlayer = p.maxPlayer;
                ri.CanWatch = p.canWatch;
                ri.Mode = p.mode;
                ri.Visibility = p.visibility;
                ri.RoomID = p.roomID;

                JoinRoomReq req = new JoinRoomReq();
                req.PlayerInfo = pi;
                req.RoomInfo = ri;
                req.GameID = p.gameID;
                req.JoinType = p.joinType;
                req.CpProto = tbs(p.userProfile);
                req.WatchSetting = p.watchSetting;
                var tags = p.tags;
                if(tags!=null){
                    foreach(var k in tags.Keys){
                        var tag = new keyValue();
                        tag.Key = k;
                        tag.Value = tags[k];
                        req.Tags.Add(tag);
                    }
                }

                return fHe(req.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)CmdId.JoinRoomRsp] = (He header)=>{
                JoinRoomRsp rsp = (JoinRoomRsp)JoinRoomRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("JoinRoomRsp.Status:{0}->{1}",rsp.Status,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };

            mD[(int)CmdId.NoticeUserJoinReq] = (He header)=>{
                NoticeJoin rsp = (NoticeJoin)NoticeJoin.Descriptor.Parser.ParseFrom(header.data);
                Log.d("NoticeUserJoinReq.UserID:{0}->{1}",rsp.User.UserID,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };

            mE[(int)CmdId.LeaveRoomReq] = (object[] arguments)=>{
                LeaveRoomReq p = new LeaveRoomReq();
                p.GameID = MVS.GameID;
                p.UserID = MVS.UserID;
                p.RoomID = MVS.RoomID;
                p.CpProto = ByteString.CopyFrom(StringUtil.toUtf8Array((string)arguments[1]));
                return fHe(p.ToByteArray(),(int)arguments[0]);
            };
            mD[(int)CmdId.LeaveRoomRsp] = (He header)=>{
                LeaveRoomRsp rsp = (LeaveRoomRsp)LeaveRoomRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("LeaveRoomRsp.UserID:{0}->{1}",rsp.UserID,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };
            mD[(int)CmdId.NoticeUserLeaveReq] = (He header)=>{
                NoticeLeave rsp = (NoticeLeave)NoticeLeave.Descriptor.Parser.ParseFrom(header.data);
                Log.d("NoticeUserLeave.UserID:{0}->{1}",rsp.UserID,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };

            //hotel
            mE[(int)SDKHotelCmdID.CheckinCmdid] = (object[] arguments)=>{
                var pkg = new CheckIn();
                pkg.GameID = (uint)arguments[4];
                pkg.RoomID = ((RoomInfo)arguments[2]).RoomID;
                pkg.UserID = (uint)arguments[3];
                var bookInfo = (BookInfo)arguments[1];
                pkg.BookID = bookInfo.BookID;
                pkg.Key = bookInfo.BookKey;

                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)SDKHotelCmdID.CheckinAckCmdid] = (He header)=>{
                CheckInAck rsp = (CheckInAck)CheckInAck.Descriptor.Parser.ParseFrom(header.data);
                Log.d("CheckInAck.BookID:{0}->{1}",rsp.BookID,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };

            //sendEvent
            mE[(int)SDKHotelCmdID.BroadcastCmdid] = (object[] arguments)=>{
                var pkg = new Broadcast();
                pkg.RoomID = (ulong)arguments[1];
                uint[] dstUids = (uint[])arguments[2];
                foreach(var i in dstUids){
                    pkg.DstUids.Add(i);
                }

                // 低8位 由 0-3  | destType |msgType 组合 0000|00|00
                uint priority = 2;
                uint flag = (uint)(((priority&0x0F)<<4)+(((int)arguments[3]&0x03)<<2)+((int)arguments[4]&0x03));
                pkg.Flag = flag;

                pkg.CpProto = ByteString.CopyFrom((byte[])arguments[5]);
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)SDKHotelCmdID.BroadcastAckCmdid] = (He header)=>{
                BroadcastAck rsp = (BroadcastAck)BroadcastAck.Descriptor.Parser.ParseFrom(header.data);
                Log.d("BroadcastAck.Status:{0}->{1}",rsp.Status,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };
            mD[(int)SDKHotelCmdID.NotifyCmdid] = (He header)=>{
                Notify rsp = (Notify)Notify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("Notify.SrcUid:{0}->data.len:{1}",rsp.SrcUid,rsp.CpProto.Length);
                header.deserialization = rsp;
                return header;
            };

            //FrameSync
            mE[(int)SDKHotelCmdID.FrameBroadcastCmdid] = (object[] arguments)=>{
                var pkg = new FrameBroadcast();
                pkg.RoomID = (ulong)arguments[1];
                pkg.Priority = (uint)arguments[2];
                pkg.CpProto = ByteString.CopyFrom((byte[])arguments[3]);
                pkg.Operation = (int)arguments[4];
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };
            mE[(int)SDKHotelCmdID.SetFrameSyncRateCmdid] = (object[] arguments)=>{
                var pkg = new SetFrameSyncRate();
                pkg.GameID = (uint)arguments[1];
                pkg.RoomID = (ulong)arguments[2];
                pkg.Priority = (uint)arguments[3];
                pkg.FrameRate = (uint)arguments[4];
                pkg.EnableGS = (uint)arguments[5];
                pkg.CacheFrameMS = (int)arguments[6];
                pkg.FrameIdx = 1;
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)SDKHotelCmdID.FrameBroadcastAckCmdid] = (He header)=>{
                FrameBroadcastAck rsp = (FrameBroadcastAck)FrameBroadcastAck.Descriptor.Parser.ParseFrom(header.data);
                Log.d("FrameBroadcastAck.Status:{0}->{1}",rsp.Status,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };
            mD[(int)SDKHotelCmdID.FrameDataNotifyCmdid] = (He header)=>{
                FrameDataNotify rsp = (FrameDataNotify)FrameDataNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("FrameDataNotify.SrcUid:{0}->data.len:{1}",rsp.SrcUid,rsp.CpProto.Length);
                header.deserialization = rsp;
                return header;
            };
            mD[(int)SDKHotelCmdID.FrameSyncNotifyCmdid] = (He header)=>{
                FrameSyncNotify rsp = (FrameSyncNotify)FrameSyncNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("FrameSyncNotify.LastIdx:{0}->TimeStamp:{1}",rsp.LastIdx,rsp.TimeStamp);
                header.deserialization = rsp;
                return header;
            };

            mD[(int)SDKHotelCmdID.SetFrameSyncRateAckCmdid] = (He header)=>{
                SetFrameSyncRateAck rsp =
                    (SetFrameSyncRateAck)SetFrameSyncRateAck.Descriptor.Parser.ParseFrom(header.data);
                Log.d("SetFrameSyncRateAck.Status:{0}->{1}",rsp.Status,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };
            mD[(int)SDKHotelCmdID.SetFrameSyncRateNotifyCmdid] = (He header)=>{
                SetFrameSyncRateNotify rsp =
                    (SetFrameSyncRateNotify)SetFrameSyncRateNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("SetFrameSyncRateNotify.FrameRate:{0}->{1}",rsp.FrameRate,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };

            //turn on/off for join room
            mE[(int)CmdId.JoinOpenReq] = (object[] arguments)=>{
                var pkg = new JoinOpenReq();
                pkg.GameID = (uint)arguments[1];
                pkg.RoomID = (ulong)arguments[2];
                pkg.UserID = (uint)arguments[3];
                pkg.CpProto = ByteString.CopyFrom((byte[])arguments[4]);
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };
            mE[(int)CmdId.JoinOverReq] = (object[] arguments)=>{
                var pkg = new JoinOverReq();
                pkg.GameID = (uint)arguments[1];
                pkg.RoomID = (ulong)arguments[2];
                pkg.UserID = (uint)arguments[3];
                pkg.CpProto = ByteString.CopyFrom((byte[])arguments[4]);
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)CmdId.JoinOverRsp] = (He header)=>{
                JoinOverRsp rsp = (JoinOverRsp)JoinOverRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("JoinOverRsp.Status:{0}->{1}",rsp.Status,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };
            mD[(int)CmdId.JoinOpenRsp] = (He header)=>{
                JoinOpenRsp rsp = (JoinOpenRsp)JoinOpenRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("JoinOpenRsp.Status:{0}->{1}",rsp.Status,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };

            mD[(int)CmdId.JoinOverNotify] = (He header)=>{
                JoinOverNotify rsp = (JoinOverNotify)JoinOverNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("JoinOverNotify.UserID:{0}->{1}",rsp.SrcUserID,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };
            mD[(int)CmdId.JoinOpenNotify] = (He header)=>{
                JoinOpenNotify rsp = (JoinOpenNotify)JoinOpenNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("JoinOpenNotify.UserID:{0}->{1}",rsp.UserID,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };

            //SetRoomProperty
            mE[(int)CmdId.SetRoomPropertyReq] = (object[] arguments)=>{
                var pkg = new SetRoomPropertyReq();
                pkg.GameID = (uint)arguments[1];
                pkg.RoomID = (ulong)arguments[2];
                pkg.UserID = (uint)arguments[3];
                pkg.RoomProperty = ByteString.CopyFrom((byte[])arguments[4]);
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)CmdId.SetRoomPropertyRsp] = (He header)=>{
                SetRoomPropertyRsp rsp =
                    (SetRoomPropertyRsp)SetRoomPropertyRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("SetRoomPropertyRsp.UserID:{0}->{1}",rsp.UserID,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };
            mD[(int)CmdId.NoticeRoomProperty] = (He header)=>{
                NoticeRoomProperty rsp =
                    (NoticeRoomProperty)NoticeRoomProperty.Descriptor.Parser.ParseFrom(header.data);
                Log.d("NoticeRoomProperty.UserID:{0}->{1}",rsp.UserID,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };

            //getRoomDetail
            mE[(int)CmdId.GetRoomDetailReq] = (object[] arguments)=>{
                var pkg = new GetRoomDetailReq();
                pkg.GameID = (uint)arguments[1];
                pkg.RoomID = (ulong)arguments[2];
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)CmdId.GetRoomDetailRsp] = (He header)=>{
                GetRoomDetailRsp rsp = (GetRoomDetailRsp)GetRoomDetailRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("GetRoomDetailRsp:{0}->{1}",rsp.Status,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };

            //getRoomList
            mE[(int)CmdId.RoomListExReq] = (object[] arguments)=>{
                var pkg = new GetRoomListExReq();
                var filter = (MsRoomFilter)arguments[2];

                var roomFilter = new RoomFilter();
                roomFilter.MaxPlayer = (filter.maxPlayer);
                roomFilter.Mode = filter.mode;
                roomFilter.Full = filter.full;
                roomFilter.CanWatch = (filter.canWatch);
                roomFilter.RoomProperty = ByteString.CopyFrom(StringUtil.toUtf8Array(filter.roomProperty));
                roomFilter.State = filter.state;
                roomFilter.GetSystemRoom = filter.getSystemRoom;

                pkg.GameID = (uint)arguments[1];
                pkg.RoomFilter = roomFilter;
                pkg.Sort = (filter.sort);
                pkg.Order = (filter.order);
                pkg.PageNo = (filter.pageNo);
                pkg.PageSize = (filter.pageSize);
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };
            mD[(int)CmdId.RoomListExRsp] = (He header)=>{
                GetRoomListExRsp rsp = (GetRoomListExRsp)GetRoomListExRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("GetRoomDetailRsp:{0}->{1}",rsp.Status,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };

            //NetWorkState
            mD[(int)CmdId.NoticeRoomNetworkState] = (He header)=>{
                RoomNetworkStateNotify rsp =
                    (RoomNetworkStateNotify)RoomNetworkStateNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("RoomNetworkStateNotify:{0}",rsp.ToString());
                header.deserialization = rsp;
                return header;
            };
            mD[(int)CmdId.NoticeTeamNetworkState] = (He header)=>{
                TeamNetworkStateNotify rsp =
                    (TeamNetworkStateNotify)TeamNetworkStateNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("TeamNetworkStateNotify:{0}",rsp.ToString());
                header.deserialization = rsp;
                return header;
            };


            //watchRoomList
            mE[(int)CmdId.GetWatchRoomsReq] = (object[] arguments)=>{
                var pkg = new GetWatchRoomsReq();
                var filter = (MsRoomFilter)arguments[2];


                var roomFilter = new RoomFilter();
                roomFilter.MaxPlayer = (filter.maxPlayer);
                roomFilter.Mode = filter.mode;
                roomFilter.Full = filter.full;
                roomFilter.CanWatch = (filter.canWatch);
                roomFilter.RoomProperty = ByteString.CopyFrom(StringUtil.toUtf8Array(filter.roomProperty));
                roomFilter.State = filter.state;
                roomFilter.GetSystemRoom = filter.getSystemRoom;

                pkg.GameID = (uint)arguments[1];
                pkg.RoomFilter = roomFilter;
                pkg.Sort = (filter.sort);
                pkg.Order = (filter.order);
                pkg.PageNo = (filter.pageNo);
                pkg.PageSize = (filter.pageSize);
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };
            mD[(int)CmdId.GetWatchRoomsRsp] = (He header)=>{
                GetWatchRoomsRsp rsp = (GetWatchRoomsRsp)GetWatchRoomsRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("GetWatchRoomsRsp:{0}->{1}",rsp.Status,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };

            //joinWatch
            mE[(int)CmdId.JoinWatchRoomReq] = (object[] arguments)=>{
                var p = (MsRoomInfo)arguments[1];
                var req = new JoinWatchRoomReq();
                req.GameID = p.gameID;
                req.RoomID = p.roomID;
                req.UserID = p.userID;
                req.UserProfile = tbs(p.userProfile);
                return fHe(req.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)CmdId.JoinWatchRoomRsp] = (He header)=>{
                var rsp = (JoinWatchRoomRsp)JoinWatchRoomRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("JoinWatchRoomRsp.Status:{0}->{1}",rsp.Status,rsp.ToString());
                header.deserialization = rsp;
                return header;
            };

            mD[(int)CmdId.JoinWatchRoomNotify] = (He header)=>{
                var rsp = (JoinWatchRoomNotify)JoinWatchRoomNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("JoinWatchRoomNotify:{0}",rsp.ToString());
                header.deserialization = rsp;
                return header;
            };

            //leaveWatch
            mE[(int)CmdId.LeaveWatchRoomReq] = (object[] arguments)=>{
                var p = new LeaveWatchRoomReq();
                p.GameID = MVS.GameID;
                p.UserID = MVS.UserID;
                p.RoomID = MVS.RoomID;
                p.CpProto = ByteString.CopyFrom(StringUtil.toUtf8Array((string)arguments[1]));
                return fHe(p.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)CmdId.LeaveWatchRoomRsp] = (He header)=>{
                var rsp = (LeaveWatchRoomRsp)LeaveWatchRoomRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("LeaveWatchRoomRsp:{0}",rsp.ToString());
                header.deserialization = rsp;
                return header;
            };
            mD[(int)CmdId.LeaveWatchRoomNotify] = (He header)=>{
                var rsp = (LeaveWatchRoomNotify)LeaveWatchRoomNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("LeaveWatchRoomNotify:{0}->",rsp.ToString());
                header.deserialization = rsp;
                return header;
            };

            //WatchCheckIn
            mE[(int)SDKWatchCmdID.EnterLiveRoomCmdid] = (object[] arguments)=>{
                var pkg = new EnterLiveRoom();
                pkg.GameID = (uint)arguments[4];
                pkg.RoomID = (ulong)arguments[2];
                pkg.UserID = (uint)arguments[3];
                var bookInfo = (BookInfo)arguments[1];
                pkg.BookID = bookInfo.BookID;
                pkg.Ticket = bookInfo.BookKey;
                pkg.SetID = (uint)arguments[5];
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };
            mE[(int)SDKWatchCmdID.SetLiveOffsetCmdid] = (object[] arguments)=>{
                var pkg = new SetLiveOffset();
                pkg.GameID = (uint)arguments[1];
                pkg.RoomID = (ulong)arguments[2];
                pkg.UserID = (uint)arguments[3];
                pkg.OffsetMS = (int)arguments[4];
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)SDKWatchCmdID.EnterLiveRoomAckCmdid] = (He header)=>{
                var rsp = (EnterLiveRoomAck)EnterLiveRoomAck.Descriptor.Parser.ParseFrom(header.data);
                Log.d("EnterLiveRoomAck:{0}",rsp.ToString());
                header.deserialization = rsp;
                return header;
            };
            mD[(int)SDKWatchCmdID.EnterLiveRoomNotifyCmdid] = (He header)=>{
                var rsp = (EnterLiveRoomNotify)EnterLiveRoomNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("EnterLiveRoomNotify:{0}",rsp.ToString());
                header.deserialization = rsp;
                return header;
            };
            mD[(int)SDKWatchCmdID.ExitLiveRoomNotifyCmdid] = (He header)=>{
                var rsp = (ExitLiveRoomNotify)ExitLiveRoomNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("ExitLiveRoomNotify:{0}",rsp.ToString());
                header.deserialization = rsp;
                return header;
            };


            mD[(int)SDKWatchCmdID.SetLiveOffsetAckCmdid] = (He header)=>{
                var rsp = (SetLiveOffsetAck)SetLiveOffsetAck.Descriptor.Parser.ParseFrom(header.data);
                Log.d("SetLiveOffsetAck:{0}",rsp.ToString());
                header.deserialization = rsp;
                return header;
            };


            //LiveBroadcast
            mE[(int)SDKWatchCmdID.LiveBroadcastCmdid] = (object[] arguments)=>{
                var pkg = new LiveBroadcast();
                pkg.RoomID = (ulong)arguments[1];
                uint[] dstUids = (uint[])arguments[2];
                foreach(var i in dstUids){
                    pkg.DstUids.Add(i);
                }

                // 低8位 由 0-3  | destType |msgType 组合 0000|00|00
                uint priority = 2;
                uint flag = ((priority&0x0F)<<4)+(((uint)arguments[3]&0x03)<<2)+((uint)arguments[4]&0x03);
                pkg.Flag = flag;
                pkg.CpProto = ByteString.CopyFrom((byte[])arguments[5]);
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };
            mD[(int)SDKWatchCmdID.LiveBroadcastAckCmdid] = (He header)=>{
                var rsp = (LiveBroadcastAck)LiveBroadcastAck.Descriptor.Parser.ParseFrom(header.data);
                Log.d("LiveBroadcast:{0}",rsp.ToString());
                header.deserialization = rsp;
                return header;
            };
            mD[(int)SDKWatchCmdID.LiveBroadcastNotifyCmdid] = (He header)=>{
                var rsp = (LiveBroadcastNotify)LiveBroadcastNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("LiveBroadcastNotify:{0}",rsp.ToString());
                header.deserialization = rsp;
                return header;
            };


            mD[(int)SDKWatchCmdID.LiveFrameDataNotifyCmdid] = (He header)=>{
                var rsp = (LiveFrameDataNotify)LiveFrameDataNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("LiveFrameDataNotify.{0}",rsp);
                header.deserialization = rsp;
                return header;
            };
            mD[(int)SDKWatchCmdID.LiveFrameSyncNotifyCmdid] = (He header)=>{
                var rsp = (LiveFrameSyncNotify)LiveFrameSyncNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("LiveFrameSyncNotify:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            //watch heart beat
            mE[(int)SDKWatchCmdID.LiveHeartbeatCmdid] = (object[] arguments)=>{
                LiveHeartbeat req = new LiveHeartbeat();
                req.GameID = (uint)arguments[1];
                req.RoomID = (ulong)arguments[2];
                req.UserID = (uint)arguments[3];
                return fHe(req.ToByteArray(),(int)arguments[0]);
            };
            mD[(int)SDKWatchCmdID.LiveHeartbeatAckCmdid] = (He header)=>{
                var rsp = (LiveHeartbeatAck)LiveHeartbeatAck.Descriptor.Parser.ParseFrom(header.data);
                Log.d("LiveFrameSyncNotify:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };
            mD[(int)SDKWatchCmdID.LiveOverNotifyCmdid] = (He header)=>{
                var rsp = (LiveOverNotify)LiveOverNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("LiveOverNotify:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };


            //team - create
            mE[(int)CmdId.CreateTeamReq] = (arguments)=>{

                PlayerInfo pi = new PlayerInfo();
                pi.UserID = (uint)arguments[1];
                pi.UserProfile = tbs((string)arguments[2]);

                CreateTeamReq req = new CreateTeamReq();
                req.PlayerInfo = pi;
                req.GameID =(uint)arguments[3];
                req.TeamInfo = (TeamInfo)arguments[4];
                return fHe(req.ToByteArray(),(int)arguments[0]);
            };
            mD[(int)CmdId.CreateTeamRsp] = (header)=>{
                var rsp = (CreateTeamRsp)CreateTeamRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("CreateTeamRsp.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };
            //team - join
            mE[(int)CmdId.JoinTeamReq] = (arguments)=>{

                PlayerInfo pi = new PlayerInfo();
                pi.UserID = (uint)arguments[1];
                pi.UserProfile = tbs((string)arguments[2]);

                JoinTeamReq req = new JoinTeamReq();
                req.PlayerInfo = pi;
                req.GameID =(uint)arguments[3];
                req.TeamID = (ulong)arguments[4];
                req.Password = (string)arguments[5];
                req.JoinType = (JoinTeamType)arguments[6];
                return fHe(req.ToByteArray(),(int)arguments[0]);
            };
            mD[(int)CmdId.JoinTeamRsp] = (header)=>{
                var rsp = (JoinTeamRsp)JoinTeamRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("JoinTeamRsp.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            mD[(int)CmdId.JoinTeamNotify] = (He header)=>{
                var rsp = (JoinTeamNotify)JoinTeamNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("JoinTeamNotify.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            //team - leave
            mE[(int)CmdId.LeaveTeamReq] = (arguments)=>{
                LeaveTeamReq req = new LeaveTeamReq();
                req.GameID =(uint)arguments[1];
                req.TeamID = (ulong)arguments[3];
                req.UserID = (uint)arguments[2];
                return fHe(req.ToByteArray(),(int)arguments[0]);
            };
            mD[(int)CmdId.LeaveTeamRsp] = (header)=>{
                var rsp = (LeaveTeamRsp)LeaveTeamRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("LeaveTeamRsp.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            mD[(int)CmdId.LeaveTeamNotify] = (He header)=>{
                var rsp = (LeaveTeamNotify)LeaveTeamNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("LeaveTeamNotify.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            //team - match
            mE[(int)CmdId.TeamMatchReq] = (arguments)=>{
                TeamMatchReq req = new TeamMatchReq();
                req.GameID =(uint)arguments[1];
                req.UserID = (uint)arguments[2];
                req.TeamID = (ulong)arguments[3];
                req.Cond = (TeamMatchCond)arguments[4];

                MsRoomInfo p = (MsRoomInfo)arguments[5];
                RoomInfo ri = new RoomInfo();
                ri.RoomID = p.roomID;
                ri.RoomName = p.roomName;
                ri.MaxPlayer = p.maxPlayer;
                ri.Mode = p.mode;
                ri.CanWatch = p.canWatch;
                ri.Visibility = p.visibility;
                ri.RoomProperty = tbs(p.roomProperty);

                req.RoomInfo =ri;
                return fHe(req.ToByteArray(),(int)arguments[0]);
            };
            mD[(int)CmdId.TeamMatchRsp] = (header)=>{
                var rsp = (TeamMatchRsp)TeamMatchRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("TeamMatchRsp.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            mD[(int)CmdId.TeamMatchStartNotify] = (He header)=>{
                var rsp = (TeamMatchStartNotify)TeamMatchStartNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("TeamMatchStartNotify.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };
            mD[(int)CmdId.TeamMatchResultNotify] = (He header)=>{
                var rsp = (TeamMatchResultNotify)TeamMatchResultNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("TeamMatchResultNotify.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            //team - cancel match
            mE[(int)CmdId.CancelTeamMatchReq] = (arguments)=>{
                CancelTeamMatchReq req = new CancelTeamMatchReq();
                req.GameID =(uint)arguments[1];
                req.UserID = (uint)arguments[2];
                req.TeamID = (ulong)arguments[3];
                req.CpProto = tbs((string)arguments[4]);
                return fHe(req.ToByteArray(),(int)arguments[0]);
            };
            mD[(int)CmdId.CancelTeamMatchRsp] = (header)=>{
                var rsp = (CancelTeamMatchRsp)CancelTeamMatchRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("CancelTeamMatchRsp.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            mD[(int)CmdId.CancelTeamMatchNotify] = (header)=>{
                var rsp = (CancelTeamMatchNotify)CancelTeamMatchNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("CancelTeamMatchNotify.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            //team - kick match
            mE[(int)CmdId.KickTeamMemberReq] = (arguments)=>{
                KickTeamMemberReq req = new KickTeamMemberReq();
                req.GameID =(uint)arguments[1];
                req.UserID = (uint)arguments[2];
                req.TeamID = (ulong)arguments[3];
                req.CpProto = tbs((string)arguments[4]);
                req.DstUserID = (uint)arguments[5];
                return fHe(req.ToByteArray(),(int)arguments[0]);
            };
            mD[(int)CmdId.KickTeamMemberRsp] = (header)=>{
                var rsp = (KickTeamMemberRsp)KickTeamMemberRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("KickTeamMemberRsp.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            mD[(int)CmdId.KickTeamMemberNotify] = (header)=>{
                var rsp = (KickTeamMemberNotify)KickTeamMemberNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("KickTeamMemberNotify:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            //team sendEvent
            mE[(int)CmdId.SendTeamEventReq] = ( arguments)=>{
                var pkg = new SendTeamEventReq();
                pkg.TeamID = (ulong)arguments[1];
                uint[] dstUids = (uint[])arguments[2];
                foreach(var i in dstUids){
                    pkg.DstUids.Add(i);
                }
                pkg.DstType = (TeamDstType)arguments[3];
                pkg.MsgType = (TeamMsgType)arguments[4];
                pkg.CpProto = ByteString.CopyFrom((byte[])arguments[5]);
                pkg.GameID = (uint)arguments[6];
                pkg.UserID = (uint)arguments[7];
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)CmdId.SendTeamEventRsp] = (header)=>{
                var rsp = (SendTeamEventRsp)SendTeamEventRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("SendTeamEventRsp.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            mD[(int)CmdId.SendTeamEventNotify] = (header)=>{
                var rsp = (SendTeamEventNotify)SendTeamEventNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("SendTeamEventNotify.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            //team profile
            mE[(int)CmdId.SetTeamPropertyReq] = ( arguments)=>{
                var pkg = new SetTeamPropertyReq();
                pkg.GameID = (uint)arguments[1];
                pkg.TeamID = (ulong)arguments[2];
                pkg.UserID = (uint)arguments[3];;
                pkg.TeamProperty =tbs((string)arguments[4]);;
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)CmdId.SetTeamPropertyRsp] = (header)=>{
                var rsp = (SetTeamPropertyRsp)SetTeamPropertyRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("SetTeamPropertyRsp.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            mD[(int)CmdId.NoticeTeamProperty] = (header)=>{
                var rsp = (NoticeTeamProperty)NoticeTeamProperty.Descriptor.Parser.ParseFrom(header.data);
                Log.d("NoticeTeamProperty.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            //team user profile
            mE[(int)CmdId.SetTeamUserProfileReq] = (arguments)=>{
                var pkg = new SetTeamUserProfileReq();
                pkg.GameID = (uint)arguments[1];
                pkg.TeamID = (ulong)arguments[2];
                pkg.UserID = (uint)arguments[3];
                pkg.UserProfile = tbs((string)arguments[4]);
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)CmdId.SetTeamUserProfileRsp] = (header)=>{
                var rsp = (SetTeamUserProfileRsp)SetTeamUserProfileRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("SetTeamUserProfileRsp.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            mD[(int)CmdId.NoticeTeamUserProfile] = (header)=>{
                var rsp = (NoticeTeamUserProfile)NoticeTeamUserProfile.Descriptor.Parser.ParseFrom(header.data);
                Log.d("NoticeTeamUserProfile.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };


            //timeout
            //team user profile
            mE[(int)CmdId.SetReconnectTimeoutReq] = (arguments)=>{
                var pkg = new SetReconnectTimeoutReq();
                pkg.UserID = (uint)arguments[1];
                pkg.Timeout = (int)arguments[2];
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)CmdId.SetReconnectTimeoutRsp] = (header)=>{
                var rsp = (SetReconnectTimeoutRsp)SetReconnectTimeoutRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("SetReconnectTimeoutRsp.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };
            mE[(int)CmdId.SetTeamReconnectTimeoutReq] = (arguments)=>{
                var pkg = new SetTeamReconnectTimeoutReq();
                pkg.UserID = (uint)arguments[1];
                pkg.Timeout = (int)arguments[2];
                return fHe(pkg.ToByteArray(),(int)arguments[0]);
            };

            mD[(int)CmdId.SetTeamReconnectTimeoutRsp] = (header)=>{
                var rsp = (SetTeamReconnectTimeoutRsp)SetTeamReconnectTimeoutRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("SetTeamReconnectTimeoutRsp.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };


            //room - kick
            mE[(int)CmdId.KickPlayerReq] = (arguments)=>{
                KickPlayer req = new KickPlayer();
                req.SrcUserID = (uint)arguments[2];
                req.RoomID = (ulong)arguments[3];
                req.CpProto = tbs((string)arguments[4]);
                req.UserID = (uint)arguments[5];
                return fHe(req.ToByteArray(),(int)arguments[0]);
            };
            mD[(int)CmdId.KickPlayerRsp] = (header)=>{
                var rsp = (KickPlayerRsp)KickPlayerRsp.Descriptor.Parser.ParseFrom(header.data);
                Log.d("KickPlayerRsp.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };

            mD[(int)CmdId.KickPlayerNotify] = (header)=>{
                var rsp = (KickPlayerNotify)KickPlayerNotify.Descriptor.Parser.ParseFrom(header.data);
                Log.d("KickPlayerNotify.Status:{0}",rsp);
                header.deserialization = rsp;
                return header;
            };
        }
    }
}