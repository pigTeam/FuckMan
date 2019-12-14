using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matchvs;

using Unity.Entities;
using System;

public class GameNetWork : UnityContext
{
    public enum processType
    {
        None,
        Processing,
        Complete,
    }

    public static GameNetWork Inst;

    public static long FrameTime;
    public static uint UserID;
    public static string Token;
    public static bool isStart = false;

    public static Dictionary<uint, List<MoveComponent>> frameMoves = new Dictionary<uint, List<MoveComponent>>();
    public static Dictionary<uint, List<JumpComponent>> frameJumps = new Dictionary<uint, List<JumpComponent>>();
    public static Dictionary<uint, List<SimpleAttackComponent>> frameAttacks = new Dictionary<uint, List<SimpleAttackComponent>>();

    public GameObject players;

    private processType initState =  processType.None;
    private processType matchState = processType.None;
    private Action<bool> matchCallback;

    private void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        DontDestroyOnLoad(this);
        this.heartBeat = new HeartBeat(this.heatBeatFunc);
    }



    public void clearPlayerFrame()
    {
     
    }

    public void addPlayer(PlayerInfo playerInfo)
    {

        Debug.Log("  add player ------------------- :" + playerInfo.UserID);

        UnityEngine.Object playerObj = Resources.Load("Knight", typeof(GameObject));
        GameObject player = Instantiate(playerObj) as GameObject;
        player.transform.parent = players.transform;
        player.transform.localPosition = Vector3.zero;
        player.transform.localPosition += new Vector3(getPlayerCount() * 1, 0, 0);
        Knight ctrl = player.GetComponent<Knight>();
        ctrl.bindUser(playerInfo.UserID);
       
    }

    public int getPlayerCount()
    {
        return players.transform.childCount;
    }

   
    void heatBeatFunc()
    {
        Debug.Log(" heart beat ");
    }

    public void Initialize()
    {
        if(initState == processType.None)
        {
            MatchVSResponseInner.Inst.bindAll(this);
            MatchvsEngine.getInstance().init(this, GameConfig.GameID, GameConfig.AppKey, GameConfig.channel, GameConfig.platform);
        }
    }

    public void OnInitRes(bool suc)
    {
        if (suc)
        {
            initState = processType.Complete;
        }
        else
        {
            initState = processType.None;
        }

        if(matchState == processType.Processing)
        {
            if(suc)
            {
                MatchVSResponseInner.Inst.Match(Config.MaxPlayer);
            }
            else
            {
                OnMatchRres(false);
            }
        }
    }
    
    public void OnMatchRres(bool suc)
    {
        Loom.QueueOnMainThread(() => {
            if (matchCallback != null)
            {
                matchCallback.Invoke(suc);
                matchCallback = null;
            }
        });
        
        
    }

    public void Match(Action<bool> callback)
    {
        if(matchState == processType.None)
        {
            matchState = processType.Processing;
            if(initState == processType.Complete)
            {
                MatchVSResponseInner.Inst.Match(Config.MaxPlayer);
            }
            else
            {
                Initialize();
            }
        }
        else
        {
            matchCallback += callback;
        }
    }
}
