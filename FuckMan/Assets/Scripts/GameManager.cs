using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoSingleton<GameManager>
{
    public GameObject playerSpwanPoint;
    public GameObject playerPf;

    private List<GameObject> players = new List<GameObject>();

    void Start()
    {
        GameNetWork.Inst.Match(null);
    }


    public GameObject GenerateCharacter()
    {
        GameObject player = Instantiate(playerPf) as GameObject;
        player.transform.parent = playerSpwanPoint.transform;
        player.transform.localPosition = Vector3.zero;
        player.transform.localPosition += new Vector3(getPlayerCount() * 1, 0, 0);
        Knight ctrl = player.GetComponent<Knight>();
        players.Add(player);
        return player;
    }

    public int getPlayerCount()
    {
        return players.Count;
    }
}
