
using Google.Protobuf.Collections;
using Matchvs;
using System.Collections.Generic;

namespace Matchvs{
    public class MatchvsResponse{

        /**
          *
          * @param userInfo {MsRegistRsp//}
          */
        public delegate void registerUserResponse(MsUser userInfo);//{

        //Log.i(("[INFO] not found imp registerUserResponse"));
        //}
        /**
         *
         * @param loginRsp {MsLoginRsp//}
         */
        public delegate void loginResponse(LoginRsp loginRsp);//{

        //Log.i(("[INFO] not found imp loginResponse"));
        //}
        /**
         * MsLogoutRsp
         * @param status {number//}
         */
        public delegate void logoutResponse(LogoutRsp status);//{

        public delegate void createRoomResponse(CreateRoomRsp rsp);//{

        /**
         *
         * @param status
         * @param roomUserInfoList
         * @param roomInfo {MsRoomInfo//}
         */
        public delegate void joinRoomResponse(int status,List<PlayerInfo> roomUserInfoList,RoomInfo roomInfo);//{

        //Log.i(("[INFO] not found imp joinRoomResponse"));
        //}
        /**
         * message NoticeJoin
         *{
         *    PlayerInfo user = 1;
         *}
         * message PlayerInfo
         *{
         *    uint32 userID = 1;
         *    bytes userProfile = 2;
         *}
         * @param roomUserInfo {MsRoomUserInfo//}
         */
        public delegate void joinRoomNotify(PlayerInfo roomUserInfo);//{
        //Log.i(("[INFO] not found imp joinRoomNotify"));
        //}

        /**
         *
         * @param {MsJoinOverRsp//} rsp
         */
        public delegate void joinOverResponse(JoinOverRsp rsp);//{
        //Log.i(("[INFO] not found imp joinOverResponse"));
        //}

        /**
         *
         * @param notifyInfo {MsJoinOverNotifyInfo//}
         */
        public delegate void joinOverNotify(JoinOverNotify notifyInfo);//{
        //Log.i(("[INFO] not found imp joinOverNotify"));
        //}

        /**
         * message LeaveRoomRsp
         *{
         *    ErrorCode status = 1;//200.成功 | 403.房间关闭 | 404.房间不存在 | 500.服务器错误
         *    uint64 roomID = 2;
         *    uint32 userID = 3;
         *    bytes cpProto = 4;
         *}
         * @param rsp {LeaveRoomRsp//}
         */
        public delegate void leaveRoomResponse(LeaveRoomRsp rsp);//{

        //Log.i(("[INFO] not found imp leaveRoomResponse"));
        //}
        /**
         *
         * @param leaveRoomInfo {MsLeaveRoomNotify//}
         */
        public delegate void leaveRoomNotify(NoticeLeave rsp);//{

        //Log.i(("[INFO] not found imp leaveRoomNotify"));
        //}
        /**
         * MsKickPlayerRsp
         * @param rsp {MsKickPlayerRsp//}
         */
        public delegate void kickPlayerResponse(KickPlayerRsp rsp);//{

        //Log.i(("[INFO] not found imp kickPlayerResponse"));
        //}
        /**
         *
         * @param notify {MsKickPlayerNotify//}
         */
        public delegate void kickPlayerNotify(KickPlayerNotify notify);//{

        //Log.i(("[INFO] not found imp kickPlayerNotify"));
        //}
        /**
         *
         * @param rsp {MsSendEventRsp//}
         */
        public delegate void sendEventResponse(uint rsp);//{

        //Log.i(("[INFO] not found imp sendEventResponse"));
        //}
        /**
         *
         * @param tRsp {MsSendEventNotify//}
         */
        public delegate void sendEventNotify(uint userID,byte[] tRsp);//{

        //Log.i(("[INFO] not found imp sendEventNotify"));
        //}
        /**
         *
         * @param tRsp {MsGameServerNotifyInfo//}
         */
        public delegate void gameServerNotify(object tRsp);//{

        //Log.i(("[INFO] not found imp gameServerNotify"));
        //}
        /**
         *
         * @param errCode {Number//}
         * @param errMsg {string//}
         */
        public delegate void errorResponse(int errCode,string errMsg);//{

        //Log.i(("[INFO] not found imp errorResponse"));
        //}
        /**
         * status==200 is success.other is fail;
         * @param status {int//}
         */
        public delegate void initResponse(int status);//{

        //Log.i(("[INFO] not found imp initResponse"));
        //}
        /**
         *
         * @param notify{MsNetworkStateNotify//}
         */
        public delegate void networkStateNotify(RoomNetworkStateNotify notify);//{

        //Log.i(("[INFO] not found imp networkStateNotify"));
        //}
        public delegate void teamNetworkStateNotify(TeamNetworkStateNotify notify);//{

        //Log.i(("[INFO] not found imp teamNetworkStateNotify"));
        //}
        /**
         *
         * @param status {number//}
         * @param groups {Array<string>//}
         */
        public delegate void subscribeEventGroupResponse(object status,object groups);//{
        //Log.i(("[INFO] not found imp subscribeEventGroupResponse"));
        //}

        /**
         *
         * @param status {number//}
         * @param dstNum {number//}
         */
        public delegate void sendEventGroupResponse(object status,object dstNum);//{

        //Log.i(("[INFO] not found imp sendEventGroupResponse"));
        //}
        /**
         *
         * @param srcUserID {number//}
         * @param groups {Array<string>//}
         * @param cpProto {string//}
         */
        public delegate void sendEventGroupNotify(object srcUserID,object groups,object cpProto);//{

        //Log.i(("[INFO] not found imp sendEventGroupNotify"));
        //}
        /**
         *
         * @param rsp {MsSetChannelFrameSyncRsp//}
         */
        public delegate void setFrameSyncResponse(SetFrameSyncRateAck rsp);//{
        //Log.i(("[INFO] not found imp setFrameSyncResponse"));
        //}

        /**
         *
         * @param notify { MVS.MsSetFrameSyncNotify //}
         */
        public delegate void setFrameSyncNotify(SetFrameSyncRateNotify notify);//{

        //Log.i(("[INFO] not found imp setFrameSyncNotify"));
        //}
        /**
         *
         * @param rsp {MsSendFrameEventRsp//}
         */
        public delegate void sendFrameEventResponse(FrameBroadcastAck rsp);//{

        //Log.i(("[INFO] not found imp sendFrameEventResponse"));
        //}
        /**
         *
         * @param data {MsFrameData//}
         */
        public delegate void frameUpdate(MsFrameData data);//{

        //Log.i(("[INFO] not found imp joinRoomResponse"));
        //}
        /**
         *
         * @param data {number//}
         */
        public delegate void hotelHeartBeatRsp(object data);//{
        // Log.d(("[INFO] not found imp hotelHeartBeatRsp"));
        //}

        /**
         *
         * @param rsp {MsGatewaySpeedResponse//}
         */
        public delegate void gatewaySpeedResponse(object rsp);//{
        //Log.i(("[INFO] not found imp gatewaySpeedResponse"));
        //}

        /**
         *
         * @param rsp
         */
        public delegate void heartBeatResponse(object rsp);//{
        // Log.d(("[INFO] not found imp heartBeatResponse"));
        //}

        /**
         * 主动断开网络接口回调
         * @param rep
         */
        public delegate void disConnectResponse(object rep);//{
        //Log.i(("[INFO] not found imp disConnectResponse"));
        //}

        /**
         * 获取房间详细信息回调
         * @param rsp {MsGetRoomDetailRsp//}
         */
        public delegate void getRoomDetailResponse(GetRoomDetailRsp rsp);//{
        //Log.i(("[INFO] not found imp getRoomDetailResponse"));
        //}

        /**
         * 获取房间扩展信息返回
         * @param rsp {MsGetRoomListExRsp//}
         */
        public delegate void getRoomListExResponse(GetRoomListExRsp rsp);//{
        //Log.i(("[INFO] not found imp getRoomListExResponse"));
        //}

        /**
         *
         * @param rsp {MsSetRoomPropertyRspInfo//}
         */
        public delegate void setRoomPropertyResponse(SetRoomPropertyRsp rsp);//{
        //Log.i(("[INFO] not found imp setRoomPropertyResponse"));
        //}

        /**
         *
         * @param notify
         */
        public delegate void setRoomPropertyNotify(NoticeRoomProperty notify);//{
        //Log.i(("[INFO] not found imp setRoomPropertyNotify"));
        //}

        /**
         *
         * @param status
         * @param roomUserInfoList
         * @param roomInfo
         */
        public delegate void reconnectResponse(object status,object roomUserInfoList,object roomInfo);//{
        //Log.i(("[INFO] not found imp reconnectResponse"));
        //}

        public delegate void joinOpenNotify(JoinOpenNotify rsp);//{
        //Log.i(("[INFO] not found imp joinOpenNotify"));
        //}

        public delegate void joinOpenResponse(JoinOpenRsp notify);//{
        //Log.i(("[INFO] not found imp joinOpenResponse"));
        //}

        /**
         * 加入观战服务回调
         * @param rsp {MVS.EnterLiveRoomAck//} 200 成功
         */
        public delegate void joinWatchRoomResponse(EnterLiveRoomAck rsp);//{
        //Log.i(("[INFO] not found imp joinWatchRoomResponse"));
        //}

        /**
         * 加入观战服务异步回调
         * @param notify {MsRoomUserInfo//}
         */
        public delegate void joinWatchRoomNotify(JoinWatchRoomNotify notify);//{
        //Log.i(("[INFO] not found imp joinWatchRoomNotify"));
        //}

        /**
         * 离开观战服务回调
         * @param status
         */
        public delegate void leaveWatchRoomResponse(LeaveWatchRoomRsp status);//{
        //Log.i(("[INFO] not found imp leaveWatchRoomResponse"));
        //}

        /**
         * 离开观战服务异步回调
         * @param user {MVS.LeaveWatchRoomNotify//}
         */
        public delegate void leaveWatchRoomNotify(LeaveWatchRoomNotify user);//{
        //Log.i(("[INFO] not found imp leaveWatchRoomNotify"));
        //}

        public delegate void exitLiveRoomNotify(ExitLiveRoomNotify rsp);
        public delegate void enterLiveRoomNotify(EnterLiveRoomNotify rsp);
        /**
         * 获取观战房间列表回调
         * @param rooms {GetWatchRoomsRsp//}
         */
        public delegate void getWatchRoomsResponse(GetWatchRoomsRsp rsp);//{
        //Log.i(("[INFO] not found imp getWatchRoomsResponse"));
        //}


        public delegate void watchHeartBeat(object rsp);//{
        //Log.i(("[INFO] not found imp watchHeartBeat"));
        //}

        public delegate void liveBroadcastResponse(object rsp);//{
        //Log.i(("[INFO] not found imp liveBroadcastResponse"));
        //}

        public delegate void liveBroadcastNotify(object notify);//{
        //Log.i(("[INFO] not found imp liveBroadcastNotify"));

        //}

        public delegate void setLiveOffsetResponse(SetLiveOffsetAck rsp);


        public delegate void liveOverNotify(LiveOverNotify notify);

        public delegate void sendLiveEventResponse(LiveBroadcastAck notify);

        public delegate void sendLiveEventNotify(LiveBroadcastNotify notify);




        /**
         * 观战帧数据
         * @param data {MsFrameData//}
         */
        public delegate void liveFrameUpdate(LiveFrameSyncNotify rsp,LiveFrameDataNotify[] dataArray);//{
        //Log.i(("[INFO] not found imp liveFrameUpdate"));
        //}

        /**
         * 角色转换接口回调
         * @param rsp
         */
        public delegate void changeRoleResponse(object rsp);//{
        //Log.i(("[INFO] not found imp changeRoleResponse"));
        //}

        /**
         *
         * @param status {number//}
         * @constructor
         */
        public delegate void setReconnectTimeoutResponse(SetReconnectTimeoutRsp status);//{

        //Log.i(("[INFO] not found imp setReconnectTimeoutResponse"));
        //}
        public delegate void setTeamReconnectTimeoutResponse(SetTeamReconnectTimeoutRsp status);//{
        //Log.i(("[INFO] not found imp setTeamReconnectTimeoutResponse"));
        //}

        /**
         *
         * @param {number//} rsp.status
         * @param {string//} rsp.teamID
         * @param {number//} rsp.owner
         */
        public delegate void createTeamResponse(CreateTeamRsp rsp);//{
        //Log.i(("[INFO] not found imp createTeamResponse"));
        //}

        /**
         * 加入组队回调
         * @param {object//} rsp.team
         * @param {number//} rsp.status
         * @param {Array<object>//} rsp.userList
         */
        public delegate void joinTeamResponse(JoinTeamRsp rsp);//{
        //Log.i(("[INFO] not found imp joinTeamResponse"));
        //}

        /**
         * 加入组队异步回调
         * @param {object//} notify.user
         */
        public delegate void joinTeamNotify(JoinTeamNotify notify);//{
        //Log.i(("[INFO] not found imp joinTeamNotify"));
        //}


        /**
         *
         * @param {string//} rsp.teamID
         * @param {number//} rsp.status
         * @param {number//} rsp.userID
         */
        public delegate void leaveTeamResponse(LeaveTeamRsp rsp);//{
        //Log.i(("[INFO] not found imp leaveTeamResponse"));

        //}

        /**
         *
         * @param {string//} notify.teamID
         * @param {number//} notify.status
         * @param {number//} notify.userID
         * @param {String//} notify.teamProperty
         */
        public delegate void leaveTeamNotify(LeaveTeamNotify notify);//{
        //Log.i(("[INFO] not found imp leaveTeamNotify"));
        //}

        /**
         * @param {number//} rsp.status
         * @param {string//} rsp.teamID
         * @param {number//} rsp.userID
         * @param {String//} rsp.teamProperty
         */
        public delegate void setTeamPropertyResponse(SetTeamPropertyRsp rsp);//{
        //Log.i(("[INFO] not found imp setTeamPropertyResponse"));
        //}

        /**
         *
         * @param {number//} notify.status
         * @param {string//} notify.teamID
         * @param {number//} notify.userID
         * @param {String//} notify.teamUserProfile
         */
        public delegate void setTeamUserProfileNotify(NoticeTeamUserProfile notify);//{

        //Log.i(("[INFO] not found imp setTeamUserProfileNotify"));
        //}
        /**
         * @param {number//} rsp.status
         * @param {string//} rsp.teamID
         * @param {number//} rsp.userID
         * @param {String//} rsp.teamProperty
         */
        public delegate void setTeamUserProfileResponse(SetTeamUserProfileRsp rsp);//{
        //Log.i(("[INFO] not found imp setTeamUserProfileResponse"));
        //}

        /**
         *
         * @param {string//} notify.teamID
         * @param {number//} notify.userID
         * @param {String//} notify.teamProperty
         */
        public delegate void setTeamPropertyNotify(NoticeTeamProperty notify);//{
        //Log.i(("[INFO] not found imp setTeamPropertyNotify"));
        //}

        /**
         *
         * @param {number//} rsp.status
         */
        public delegate void teamMatchResponse(TeamMatchRsp rsp);//{
        //Log.i(("[INFO] not found imp teamMatchResponse"));
        //}

        /**
         *
         * @param notify.status
         * @param notify.brigades
         * @param notify.roomInfo
         */
        public delegate void teamMatchResultNotify(TeamMatchResultNotify notify);//{
        //Log.i(("[INFO] not found imp teamMatchResultNotify"));
        //}


        public delegate void teamMatchStartNotify(TeamMatchStartNotify notify);//{
        //Log.i(("[INFO] not found imp teamMatchStartNotify"));
        //}

        public delegate void getOffLineDataResponse(object rsp);//{
        //Log.i(("[INFO] not found imp getOffLineDataResponse"));
        //}

        /**
         * 取消组队匹配回调
         * @param {number//} rsp.status
         */
        public delegate void cancelTeamMatchResponse(CancelTeamMatchRsp rsp);//{
        //Log.i(("[INFO] not found imp cancelTeamMatchResponse"));
        //}

        /**
         * 取消队伍匹配，异步接口通知
         * @param {number//} notify.userID
         * @param {string//} notify.teamID
         * @param {string//} notify.cpProto
         */
        public delegate void cancelTeamMatchNotify(CancelTeamMatchNotify notify);//{
        //Log.i(("[INFO] not found imp cancelTeamMatchNotify"));
        //}

        /**
         *
         * @param {number//} rsp.status
         * @param {Array<number>//} rsp.dstUserIDs
         */
        public delegate void sendTeamEventResponse(SendTeamEventRsp rsp);//{
        //Log.i(("[INFO] not found imp sendTeamEventResponse"));

        //}

        public delegate void sendTeamEventNotify(SendTeamEventNotify notify);//{
        //Log.i(("[INFO] not found imp sendTeamEventNotify"));

        //}

        /**
         *
         * @param rsp
         */
        public delegate void kickTeamMemberResponse(KickTeamMemberRsp rsp);//{
        //Log.i(("[INFO] not found imp kickTeamMemberResponse"));
        //}

        public delegate void kickTeamMemberNotify(KickTeamMemberNotify notify);//{
        //Log.i(("[INFO] not found imp kickTeamMemberNotify"));
        //}

    }
}