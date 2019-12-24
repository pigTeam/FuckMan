using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoSingleton<GameManager>
{
    public GameObject playerSpwanPoint;
    public GameObject playerPf;

    private List<int> playerIdList = new List<int>();
    private List<CharacterBase> players = new List<CharacterBase>();

    void Start()
    {
        //GameNetWork.Inst.Match(null);
        GenerateLocalPlayer(0,true);
        GenerateLocalPlayer(1,false);
    }

    public void GenerateLocalPlayer(uint id,bool isSelf)
    {
        CharacterBase ownPlayer = GenerateCharacter();
        ownPlayer.InintCharacter(id, isSelf);
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

    public void DestroyAllCharacter()
    {
        for (int i = 0; i < players.Count; i++)
        {
            GameObject.Destroy(players[i].gameObject);
        }
        players.Clear();
    }

    public int getPlayerCount()
    {
        return players.Count;
    }

    public List<int> GetPlayerInjuries()
    {
        List<int> result = null;
        if(players.Count > 0)
        {
            result = new List<int>();

            for (int i = 0; i < players.Count; i++)
            {
                InjuryComponent injury;
                if(players[i].GetComponentData<InjuryComponent>(out injury))
                {
                    result.Add(injury.value);
                }
            }
        }

        return result;
    }

    public CharacterBase GetCharacter(uint id)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if(players[i].getUserID() == id)
            {
                return players[i];
            }
        }
        return null;
    }
}
