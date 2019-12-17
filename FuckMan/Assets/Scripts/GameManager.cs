using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoSingleton<GameManager>
{
    public GameObject playerSpwanPoint;
    public GameObject playerPf;

    private List<CharacterBase> players = new List<CharacterBase>();
    private CharacterBase ownPlayer;

    void Start()
    {
        //GameNetWork.Inst.Match(null);
        GenerateLocalPlayer();
    }

    public void GenerateLocalPlayer()
    {
        ownPlayer = GenerateCharacter();
        ownPlayer.InintCharacter(0, true);
    }

    public void DestroyLocalPlayer()
    {
        DestroyCharacter(ownPlayer);
    }

    public CharacterBase GenerateCharacter()
    {
        GameObject player = Instantiate(playerPf) as GameObject;
        player.transform.parent = playerSpwanPoint.transform;
        player.transform.localPosition = Vector3.zero;
        player.transform.localPosition += new Vector3(getPlayerCount() * 1, 0, 0);
        CharacterBase character = player.GetComponent<CharacterBase>();
        players.Add(character);
        return character;
    }

    public void DestroyCharacter(CharacterBase player)
    {
        if(player != null)
        {
            if (players.Contains(player))
            {
                players.Remove(player);
            }
            GameObject.Destroy(player.gameObject);
        }
    }

    public int getPlayerCount()
    {
        return players.Count;
    }
}
