using System;
using System.Collections;
using System.Collections.Generic;
using Matchvs;
using UnityEngine;


public class MatchVSResponseInner : MatchvsResponse
{

    private static MatchVSResponseInner instance;
    private MatchVSResponseInner() { }
    public static MatchVSResponseInner Inst
    {
        get
        {
            if (instance == null)
            {
                instance = new MatchVSResponseInner();
            }
            return instance;
        }
    }

    private initResponse initRes;
    private registerUserResponse registerUserRes;
    private loginResponse loginRes;
    private errorResponse errorRes;
    private joinRoomResponse joinRoomRes;
    private joinRoomNotify joinRoomNot;
    private joinOverResponse joinOverRes;
    private sendFrameEventResponse sendFrameEventRes;
    private frameUpdate frameUp;
    private setFrameSyncResponse setFrameSyncRes;
    private setFrameSyncNotify setFrameSyncNot;


    private MatchvsEngine engine;
    private GameNetWork gameNet;

    public void bindAll(GameNetWork gameNet)
    {
        this.gameNet = gameNet;
        engine = MatchvsEngine.getInstance();
        registerUserRes = new registerUserResponse(registerUserResponseInner);
        initRes = new initResponse(initResponseInner);
        loginRes = new loginResponse(loginResponseInner);
        errorRes = new errorResponse(errorResponseInner);
        joinRoomRes = new joinRoomResponse(joinRoomResponseInner);
        joinRoomNot = new joinRoomNotify(joinRoomNotifyInner);
        joinOverRes = new joinOverResponse(joinOverResponseInner);
        sendFrameEventRes = new sendFrameEventResponse(sendFrameEventResponseInner);
        frameUp = new frameUpdate(frameUpdateInner);
        setFrameSyncRes = new setFrameSyncResponse(setFrameSyncResponseInner);
        setFrameSyncNot = new setFrameSyncNotify(setFrameSyncNotifyInner);



        engine.listen("initResponse", initRes);
        engine.listen("loginResponse", loginRes);
        engine.listen("errorResponse", errorRes);
        engine.listen("joinRoomResponse", joinRoomRes);
        engine.listen("joinRoomNotify", joinRoomNot);
        engine.listen("joinOverResponse", joinOverRes);
        engine.listen("sendFrameEventResponse", sendFrameEventRes);
        engine.listen("frameUpdate", frameUp);
        engine.listen("setFrameSyncResponse", setFrameSyncRes);
        engine.listen("setFrameSyncNotify", setFrameSyncNot);


    }

    public void Match(uint maxCount)
    {
        engine.joinRandomRoom(maxCount);
    }

    /**
           *
           * @param userInfo {MsRegistRsp//}
           */
    private void registerUserResponseInner(MsUser userInfo)
    {
        Debug.Log("registerUserResponseInner");
        Debug.Log(userInfo);
        GameNetWork.UserID = userInfo.userid;
        GameNetWork.Token = userInfo.token;
        engine.login(userInfo.userid, userInfo.token);
    }
    /**
     *
     * @param loginRsp {MsLoginRsp//}
     */
    private void loginResponseInner(LoginRsp loginRsp)
    {
        Debug.Log("loginResponseInner");
        Debug.Log(loginRsp);
        if (loginRsp.Status == ErrorCode.Ok)
        {
            GameNetWork.Inst.OnInitRes(true);
            //engine.joinRandomRoom(2);
        }
        else
        {
            GameNetWork.Inst.OnInitRes(false);
            Debug.LogError(" login failed !");
        }
    }
    /**
     * MsLogoutRsp
     * @param status {number//}
     */
    private void logoutResponseInner(LogoutRsp status)
    {
    }
    private void createRoomResponseInner(CreateRoomRsp rsp)
    {
    }
    /**
     *
     * @param status
     * @param roomUserInfoList
     * @param roomInfo {MsRoomInfo//}
     */
    private void joinRoomResponseInner(int status, List<PlayerInfo> roomUserInfoList, RoomInfo roomInfo)
    {
        gameNet.OnJoinRoomResponse(status, roomUserInfoList, roomInfo);
        
    }
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
    private void joinRoomNotifyInner(PlayerInfo roomUserInfo)
    {
        Debug.Log(" joinRoomNotifyInner ");
        gameNet.addPlayer(roomUserInfo);
        if (GameManager.Inst.getPlayerCount() >= 2)
        {
            engine.joinOver();
        }
    }
    /**
     *
     * @param {MsJoinOverRsp//} rsp
     */
    private void joinOverResponseInner(JoinOverRsp rsp)
    {
        Debug.Log(" joinOverResponseInner ");
        if (rsp.Status == ErrorCode.Ok)
        {
            GameNetWork.isStart = true;


        }
        else
        {
            Debug.LogError(" join over failed !");
        }

    }
    /**
     *
     * @param notifyInfo {MsJoinOverNotifyInfo//}
     */
    private void joinOverNotifyInner(JoinOverNotify notifyInfo)
    {
        Debug.Log(" ------------------ joinOverNotifyInner ----------------------");
        GameNetWork.isStart = true;
    }
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
    private void leaveRoomResponseInner(LeaveRoomRsp rsp)
    {
    }
    /**
     *
     * @param leaveRoomInfo {MsLeaveRoomNotify//}
     */
    private void leaveRoomNotifyInner(NoticeLeave rsp)
    {
    }
    /**
     * MsKickPlayerRsp
     * @param rsp {MsKickPlayerRsp//}
     */
    private void kickPlayerResponseInner(KickPlayerRsp rsp)
    {
    }
    /**
     *
     * @param notify {MsKickPlayerNotify//}
     */
    private void kickPlayerNotifyInner(KickPlayerNotify notify)
    {
    }
    /**
     *
     * @param rsp {MsSendEventRsp//}
     */
    private void sendEventResponseInner(uint rsp)
    {
    }
    /**
     *
     * @param tRsp {MsSendEventNotify//}
     */
    private void sendEventNotifyInner(uint userID, byte[] tRsp)
    {
    }
    /**
     *
     * @param tRsp {MsGameServerNotifyInfo//}
     */
    private void gameServerNotifyInner(object tRsp)
    {
    }
    /**
     *
     * @param errCode {Number//}
     * @param errMsg {string//}
     */
    private void errorResponseInner(int errCode, string errMsg)
    {
        Debug.LogError(errMsg);
    }
    /**
     * status==200 is success.other is fail;
     * @param status {int//}
     */
    private void initResponseInner(int status)
    {
        Debug.Log(" init response :" + status);
        if (status == 200)
        {
            engine.registerUser(registerUserRes);
        }
        else
        {
            Debug.LogError("MatchvsEngine init fail!");
        }
    }
    /**
     *
     * @param notify{MsNetworkStateNotify//}
     */
    private void networkStateNotifyInner(RoomNetworkStateNotify notify)
    {
    }
    private void teamNetworkStateNotifyInner(TeamNetworkStateNotify notify)
    {
    }
    /**
     *
     * @param status {number//}
     * @param groups {Array<string>//}
     */
    private void subscribeEventGroupResponseInner(object status, object groups)
    {
    }
    /**
     *
     * @param status {number//}
     * @param dstNum {number//}
     */
    private void sendEventGroupResponseInner(object status, object dstNum)
    {
    }
    /**
     *
     * @param srcUserID {number//}
     * @param groups {Array<string>//}
     * @param cpProto {string//}
     */
    private void sendEventGroupNotifyInner(object srcUserID, object groups, object cpProto)
    {
    }
    /**
     *
     * @param rsp {MsSetChannelFrameSyncRsp//}
     */
    private void setFrameSyncResponseInner(SetFrameSyncRateAck rsp)
    {

        Debug.Log(" setFrameSyncResponseInner: " + rsp);
    }
    /**
     *
     * @param notify { MVS.MsSetFrameSyncNotify //}
     */
    private void setFrameSyncNotifyInner(SetFrameSyncRateNotify notify)
    {
        //Debug.Log(" setFrameSyncNotifyInner: " + notify);
    }
    /**
     *
     * @param rsp {MsSendFrameEventRsp//}
     */
    private void sendFrameEventResponseInner(FrameBroadcastAck rsp)
    {

        Debug.Log(" sendFrameEventResponseInner: " + (System.DateTime.Now.Ticks - GameNetWork.FrameTime) / 1000000);
    }
    /**
     *
     * @param data {MsFrameData//}
     */
    private void frameUpdateInner(MsFrameData data)
    {
        if (data.frameItems.Length > 0)
        {
            for (int i = 0; i < data.frameItems.Length; i++)
            {
                var item = data.frameItems.GetValue(i);
                if (item != null)
                {
                    FrameDataNotify notify = (FrameDataNotify)item;

                    try
                    {
                        var frameData = DataUtil.Deserialize<FrameData>(notify.CpProto);
                        if (frameData.dataType == DataType.INPUT)
                        {
                            InputData inputData = (InputData)frameData.data;
                            List<MoveComponent> moveList;
                            if (GameNetWork.frameMoves.TryGetValue(notify.SrcUid, out moveList))
                            {

                                moveList.Add(inputData.move);

                            }
                            else
                            {
                                moveList = new List<MoveComponent>();
                                GameNetWork.frameMoves.Add(notify.SrcUid, moveList);
                            }



                            List<JumpComponent> jumpList;
                            if (GameNetWork.frameJumps.TryGetValue(notify.SrcUid, out jumpList))
                            {

                                jumpList.Add(inputData.jump);

                            }
                            else
                            {
                                jumpList = new List<JumpComponent>();
                                GameNetWork.frameJumps.Add(notify.SrcUid, jumpList);
                            }


                            List<SimpleAttackComponent> attackList;
                            if (GameNetWork.frameAttacks.TryGetValue(notify.SrcUid, out attackList))
                            {

                                attackList.Add(inputData.simpleAttack);

                            }
                            else
                            {
                                attackList = new List<SimpleAttackComponent>();
                                GameNetWork.frameAttacks.Add(notify.SrcUid, attackList);
                            }

                            Vector3 position = inputData.position.ToVector3();
                            if (GameNetWork.framePositionChecks.ContainsKey(notify.SrcUid))
                            {

                                GameNetWork.framePositionChecks[notify.SrcUid] = position;
                            }
                            else
                            {
                                GameNetWork.framePositionChecks.Add(notify.SrcUid, position);
                            }
                        }
                        else if (frameData.dataType == DataType.DAMAGE)
                        {
                            DamageComponent damageData = (DamageComponent)frameData.data;
                            gameNet.HandleDamage(damageData);
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }
            
            }

        }
    }

/**
 *
 * @param data {number//}
 */
private void hotelHeartBeatRspInner(object data)
{
}
/**
 *
 * @param rsp {MsGatewaySpeedResponse//}
 */
private void gatewaySpeedResponseInner(object rsp)
{
}
/**
 *
 * @param rsp
 */
private void heartBeatResponseInner(object rsp)
{
}
/**
 * 主动断开网络接口回调
 * @param rep
 */
private void disConnectResponseInner(object rep)
{
}
/**
 * 获取房间详细信息回调
 * @param rsp {MsGetRoomDetailRsp//}
 */
private void getRoomDetailResponseInner(GetRoomDetailRsp rsp)
{
}
/**
 * 获取房间扩展信息返回
 * @param rsp {MsGetRoomListExRsp//}
 */
private void getRoomListExResponseInner(GetRoomListExRsp rsp)
{
}
/**
 *
 * @param rsp {MsSetRoomPropertyRspInfo//}
 */
private void setRoomPropertyResponseInner(SetRoomPropertyRsp rsp)
{
}
/**
 *
 * @param notify
 */
private void setRoomPropertyNotifyInner(NoticeRoomProperty notify)
{
}
/**
 *
 * @param status
 * @param roomUserInfoList
 * @param roomInfo
 */
private void reconnectResponseInner(object status, object roomUserInfoList, object roomInfo)
{
}

private void joinOpenNotifyInner(JoinOpenNotify rsp)
{
}
private void joinOpenResponseInner(JoinOpenRsp notify)
{
}

/**
 * 加入观战服务回调
 * @param rsp {MVS.EnterLiveRoomAck//} 200 成功
 */
private void joinWatchRoomResponseInner(EnterLiveRoomAck rsp)
{
}
/**
 * 加入观战服务异步回调
 * @param notify {MsRoomUserInfo//}
 */
private void joinWatchRoomNotifyInner(JoinWatchRoomNotify notify)
{
}
/**
 * 离开观战服务回调
 * @param status
 */
private void leaveWatchRoomResponseInner(LeaveWatchRoomRsp status)
{
}
/**
 * 离开观战服务异步回调
 * @param user {MVS.LeaveWatchRoomNotify//}
 */
private void leaveWatchRoomNotifyInner(LeaveWatchRoomNotify user)
{
}
private void exitLiveRoomNotifyInner(ExitLiveRoomNotify rsp)
{
}
private void enterLiveRoomNotifyInner(EnterLiveRoomNotify rsp)
{
}
/**
 * 获取观战房间列表回调
 * @param rooms {GetWatchRoomsRsp//}
 */
private void getWatchRoomsResponseInner(GetWatchRoomsRsp rsp)
{
}

private void watchHeartBeatInner(object rsp)
{
}

private void liveBroadcastResponseInner(object rsp)
{
}

private void liveBroadcastNotifyInner(object notify)
{
}

private void setLiveOffsetResponseInner(SetLiveOffsetAck rsp)
{
}


private void liveOverNotifyInner(LiveOverNotify notify)
{
}

private void sendLiveEventResponseInner(LiveBroadcastAck notify)
{
}

private void sendLiveEventNotifyInner(LiveBroadcastNotify notify)
{
}




/**
 * 观战帧数据
 * @param data {MsFrameData//}
 */
private void liveFrameUpdateInner(LiveFrameSyncNotify rsp, LiveFrameDataNotify[] dataArray)
{
}

/**
 * 角色转换接口回调
 * @param rsp
 */
private void changeRoleResponseInner(object rsp)
{
}

/**
 *
 * @param status {number//}
 * @constructor
 */
private void setReconnectTimeoutResponseInner(SetReconnectTimeoutRsp status)
{
}
private void setTeamReconnectTimeoutResponseInner(SetTeamReconnectTimeoutRsp status)
{
}

/**
 *
 * @param {number//} rsp.status
 * @param {string//} rsp.teamID
 * @param {number//} rsp.owner
 */
private void createTeamResponseInner(CreateTeamRsp rsp)
{
}

/**
 * 加入组队回调
 * @param {object//} rsp.team
 * @param {number//} rsp.status
 * @param {Array<object>//} rsp.userList
 */
private void joinTeamResponseInner(JoinTeamRsp rsp)
{
}

/**
 * 加入组队异步回调
 * @param {object//} notify.user
 */
private void joinTeamNotifyInner(JoinTeamNotify notify)
{
}


/**
 *
 * @param {string//} rsp.teamID
 * @param {number//} rsp.status
 * @param {number//} rsp.userID
 */
private void leaveTeamResponseInner(LeaveTeamRsp rsp)
{
}

/**
 *
 * @param {string//} notify.teamID
 * @param {number//} notify.status
 * @param {number//} notify.userID
 * @param {String//} notify.teamProperty
 */
private void leaveTeamNotifyInner(LeaveTeamNotify notify)
{
}

/**
 * @param {number//} rsp.status
 * @param {string//} rsp.teamID
 * @param {number//} rsp.userID
 * @param {String//} rsp.teamProperty
 */
private void setTeamPropertyResponseInner(SetTeamPropertyRsp rsp)
{
}

/**
 *
 * @param {number//} notify.status
 * @param {string//} notify.teamID
 * @param {number//} notify.userID
 * @param {String//} notify.teamUserProfile
 */
private void setTeamUserProfileNotifyInner(NoticeTeamUserProfile notify)
{
}
/**
 * @param {number//} rsp.status
 * @param {string//} rsp.teamID
 * @param {number//} rsp.userID
 * @param {String//} rsp.teamProperty
 */
private void setTeamUserProfileResponseInner(SetTeamUserProfileRsp rsp)
{
}

/**
 *
 * @param {string//} notify.teamID
 * @param {number//} notify.userID
 * @param {String//} notify.teamProperty
 */
private void setTeamPropertyNotifyInner(NoticeTeamProperty notify)
{
}

/**
 *
 * @param {number//} rsp.status
 */
private void teamMatchResponseInner(TeamMatchRsp rsp)
{
}

/**
 *
 * @param notify.status
 * @param notify.brigades
 * @param notify.roomInfo
 */
private void teamMatchResultNotifyInner(TeamMatchResultNotify notify)
{
}


private void teamMatchStartNotifyInner(TeamMatchStartNotify notify)
{
}

private void getOffLineDataResponseInner(object rsp)
{
}

/**
 * 取消组队匹配回调
 * @param {number//} rsp.status
 */
private void cancelTeamMatchResponseInner(CancelTeamMatchRsp rsp)
{
}

/**
 * 取消队伍匹配，异步接口通知
 * @param {number//} notify.userID
 * @param {string//} notify.teamID
 * @param {string//} notify.cpProto
 */
private void cancelTeamMatchNotifyInner(CancelTeamMatchNotify notify)
{
}

/**
 *
 * @param {number//} rsp.status
 * @param {Array<number>//} rsp.dstUserIDs
 */
private void sendTeamEventResponseInner(SendTeamEventRsp rsp)
{
}

private void sendTeamEventNotifyInner(SendTeamEventNotify notify)
{
}

/**
 *
 * @param rsp
 */
private void kickTeamMemberResponseInner(KickTeamMemberRsp rsp)
{
}

private void kickTeamMemberNotifyInner(KickTeamMemberNotify notify)
{
}
}
