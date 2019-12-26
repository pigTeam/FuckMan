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
    public static Dictionary<uint, Vector3> framePositionChecks = new Dictionary<uint, Vector3>();

    private processType initState = processType.None;
    private processType matchState = processType.None;
    private Action<bool> matchCallback;
    private MatchVSDataParser parser;

    private void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        DontDestroyOnLoad(this);
        parser = new MatchVSDataParser();
        this.heartBeat = new HeartBeat(this.heatBeatFunc);
    }

    public void clearPlayerFrame()
    {

    }

    public void OnJoinRoomResponse(int status, List<PlayerInfo> roomUserInfoList, RoomInfo roomInfo)
    {
        Loom.QueueOnMainThread(() =>
        {
            if (status == 200)
            {
                MatchvsEngine engine = MatchvsEngine.getInstance();
                OnMatchRres(true);
                if (roomInfo.Owner == GameNetWork.UserID)
                {
                    engine.setFrameSync(20, true, 0);
                }

                roomUserInfoList.ForEach(addPlayer);
                addPlayer(new PlayerInfo() { UserID = GameNetWork.UserID });
                if (GameManager.Inst.getPlayerCount() >= 2)
                {
                    engine.joinOver();
                }

                //init random
                UnityEngine.Random.InitState((int)roomInfo.RoomID);
            }
            else
            {
                OnMatchRres(false);
                Debug.LogError(" join room failed! ");
            }
        });
    }

    public void addPlayer(PlayerInfo playerInfo)
    {
        Debug.Log("  add player ------------------- :" + playerInfo.UserID);
        bool isSelf = playerInfo.UserID == UserID;
        CharacterBase player = GameManager.Inst.GenerateCharacter(isSelf);
        player.InintCharacter(playerInfo.UserID, isSelf, true);
    }


    void heatBeatFunc()
    {
        Debug.Log(" heart beat ");
    }

    public void Initialize()
    {
        if (initState == processType.None)
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

        if (matchState == processType.Processing)
        {
            if (suc)
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
        if (matchCallback != null)
        {
            matchCallback.Invoke(suc);
            matchCallback = null;
        }
    }

    public void Match(Action<bool> callback)
    {
        matchCallback += callback;
        if (matchState == processType.None)
        {
            matchState = processType.Processing;
            if (initState == processType.Complete)
            {
                MatchVSResponseInner.Inst.Match(Config.MaxPlayer);
            }
            else
            {
                Initialize();
            }
        }
    }

    #region event

    #endregion

    public void SendFrameData(FrameData data)
    {
        MatchvsEngine.getInstance().sendFrameEvent(DataUtil.Serialize(data));
    }

    public void HandleDamage(DamageComponent damage)
    {
        CharacterBase character = GameManager.Inst.GetCharacter(damage.targetUserId);
        if(character != null)
        {
            character.SetComponentData<DamageComponent>(damage);
        }
        else
        {
            Debug.LogError("Damage cannot find targetCharacter id = " + damage.targetUserId);
        }
    }

    public void ParseMatchVSData(MsFrameData data)
    {
        parser.Parse(data);
    }
}
