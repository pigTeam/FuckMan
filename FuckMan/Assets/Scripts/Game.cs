using System.Collections;
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

    public static Dictionary<uint, List<MoveComponent>> frameMoves = new Dictionary<uint, List<MoveComponent>>();
    public static Dictionary<uint, List<JumpComponent>> frameJumps = new Dictionary<uint, List<JumpComponent>>();
    public static Dictionary<uint, List<SimpleAttackComponent>> frameAttacks = new Dictionary<uint, List<SimpleAttackComponent>>();

    public GameObject players;
    
    
    // Start is called before the first frame update
    void Start()
    {
       
        this.heartBeat = new HeartBeat(this.heatBeatFunc);
        MatchVSResponseInner.Inst.bindAll(this);
        MatchvsEngine.getInstance().init(this, GameConfig.GameID, GameConfig.AppKey, GameConfig.channel, GameConfig.platform);
        
    }

    public void clearPlayerFrame()
    {
     
    }

    public void addPlayer(PlayerInfo playerInfo)
    {

        Debug.Log("  add player ------------------- :" + playerInfo.UserID);

        Object playerObj = Resources.Load("Knight", typeof(GameObject));
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
    // Update is called once per frame
    void Update()
    {
       
    }

   
    void heatBeatFunc()
    {
        Debug.Log(" heart beat ");
    }
}
