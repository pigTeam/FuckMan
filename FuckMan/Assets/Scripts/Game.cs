﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matchvs;

using Unity.Entities;
public class Game : UnityContext
{
    public static long FrameTime;
    public static uint UserID;
    public static string Token;
    public static bool isStart = false;

    public static List<FrameDataNotify> frameNotifies = new List<FrameDataNotify>(); 

    public GameObject players;
    
    private List<Knight> playerCtrlList = new List<Knight>();
    // Start is called before the first frame update
    void Start()
    {
       
        this.heartBeat = new HeartBeat(this.heatBeatFunc);
        MatchVSResponseInner.Inst.bindAll(this);
        MatchvsEngine.getInstance().init(this, GameConfig.GameID, GameConfig.AppKey, GameConfig.channel, GameConfig.platform);
        
    }

    public void clearPlayerFrame()
    {
        playerCtrlList.ForEach((Knight ctrl) => {
            if (ctrl != null)
            {
                //ctrl.playerFrame.clear();
            }
        });
    }

    public void addPlayer(PlayerInfo playerInfo)
    {

        Debug.Log("  add player ------------------- ");
        Debug.Log(playerInfo);
        Object playerObj = Resources.Load("Knight", typeof(GameObject));
        GameObject player = Instantiate(playerObj) as GameObject;
        player.transform.parent = players.transform;
        player.transform.localPosition = Vector3.zero;
        player.transform.localPosition += new Vector3(getPlayerCount() * 1, 0, 0);
        Knight ctrl = player.GetComponent<Knight>();
        ctrl.bindUser(playerInfo.UserID);
        playerCtrlList.Add(ctrl);
       
    }

    public int getPlayerCount()
    {
        return players.transform.childCount;
    }
    // Update is called once per frame
    void Update()
    {
       
    }

   
    void heatBeatFunc()
    {
        Debug.Log(" heart beat ");
    }
}
