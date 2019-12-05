using System;
using System.Collections.Generic;
using Matchvs;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class SimpleButton:MonoBehaviour{
    private int index = 0;
    public ulong teamID = 0;
    public InputField inputField;

    public ulong getTeamID(){
        ulong teamID = UInt64.Parse(inputField?.text);
        Log.i("input teamID:{0}",teamID);
        return teamID;
    }

    public void OnClick(Object o){
        Log.i("onclick:"+o.name);
        switch(o.name){
            case "BtnStart":
                GameObject.Find("Canvas").GetComponent<Demo>()?.startMatch();
                break;
            case "BtnStop":
                GameObject.Find("Canvas").GetComponent<Demo>()?.stopMatch();
                break;
            case "BtnSend":
                GameObject.Find("Canvas").GetComponent<Demo>()?.send("hello 中文测试! at:"+index++);
                break;
            case "BtnRegUser":
                GameObject.Find("Canvas").GetComponent<Demo>()?.registerUser();
                break;
            case "BtnSendFrame":
                GameObject.Find("Canvas").GetComponent<Demo>()?.sendFrameEvent("帧同步测试消息 at:"+index++);
                break;
            case "BtnLeave":
                MatchvsEngine.getInstance().leaveRoom();
                break;
            case "BtnJoinWatch":
                MatchvsEngine.getInstance().joinWatchRoom(Config.WatchRoomID);
                break;
            case "BtnLeaveWatch":
                MatchvsEngine.getInstance().leaveWatchRoom();
                break;
            case "BtnJoinOpen":
                MatchvsEngine.getInstance().joinOpen();
                break;
            case "BtnJoinOver":
                MatchvsEngine.getInstance().joinOver();
                break;
            case "BtnCreateRoom":
                MatchvsEngine.getInstance().createRoom();
                break;
            case "BtnReconnect":
                MatchvsEngine.getInstance().reconnect();
                break;
            case "BtnDisconnect":
                MatchvsEngine.getInstance().disconnect();
                break;
            case "BtnJoinProperty":
                var property = new Dictionary<string,string>();
                property.Add("Key","Unity");
                MatchvsEngine.getInstance().joinRoomWithProperty(property,2,"UnityJoinRoomProperty");
                break;
            case "BtnCreateTeam":
                MatchvsEngine.getInstance().createTeam();
                break;
            case "BtnJoinTeam":
                MatchvsEngine.getInstance().joinTeam(getTeamID());
                break;
            case "BtnLeaveTeam":
                MatchvsEngine.getInstance().leaveTeam();
                break;
            case "BtnTeamMatch":
                MatchvsEngine.getInstance().teamMatch();
                break;
            case "BtnKickTeam":
                MatchvsEngine.getInstance().kickTeamMember(Config.otherUserID);
                break;
            case "BtnKick":
                MatchvsEngine.getInstance().kickPlayer(Config.otherUserID);
                break;
            case "BtnSendTeam":
                MatchvsEngine.getInstance().sendTeamEvent("teamMsg");
                break;
            case "BtnJoinRoom":
                MatchvsEngine.getInstance().joinRandomRoom(Config.MaxPlayer," from unity,userID: "+Config.userID);
                break;
            case "BtnLogout":
                MatchvsEngine.getInstance().logout();
                break;
        }
    }
}