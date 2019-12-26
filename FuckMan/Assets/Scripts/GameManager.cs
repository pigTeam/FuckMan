using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public class GameManager : MonoSingleton<GameManager>
{
    public Transform playerSpwanPointA;
    public Transform playerSpwanPointB;
    public GameObject playerPf;

    [SerializeField]
    private Transform deadLine;

    private List<int> playerIdList = new List<int>();
    private List<CharacterBase> players = new List<CharacterBase>();

    void Start()
    {
        //GameNetWork.Inst.Match(null);
        GenerateLocalPlayer(0,true);
        GenerateLocalPlayer(1, false);
    }

    public CharacterBase GenerateLocalPlayer(uint id,bool isSelf)
    {
        CharacterBase ownPlayer = GenerateCharacter(isSelf);
        ownPlayer.InintCharacter(id, isSelf);
        return ownPlayer;
    }

    public CharacterBase GenerateCharacter(bool needCameraFollow)
    {
        GameObject player = Instantiate(playerPf) as GameObject;
        Transform spawnPoint = playerSpwanPointA.transform;
        if (players.Count % 2 != 0)
        {
            spawnPoint = playerSpwanPointB.transform;
        }

        player.transform.parent = spawnPoint.parent;
        player.transform.position = spawnPoint.position+(players.Count / 2) * new Vector3(1,0,0);
        player.transform.localPosition += new Vector3(getPlayerCount() * 1, 0, 0);
        CharacterBase character = player.GetComponent<CharacterBase>();
        players.Add(character);
        if(needCameraFollow)
        {
            CameraManager.Inst.LockFollowTarget(player.transform);
        }
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

    public int[,] GetPlayerStatus()
    {
        int[,] result = new int[2, 2];
        if(players.Count > 0)
        {
            for (int i = 0; i < 2; i++)
            {
                InjuryComponent injury;
                if(players[i].GetComponentData<InjuryComponent>(out injury))
                {
                    result[i,0] = injury.value;
                }
                ShotDownComponent shotDown;
                if (players[i].GetComponentData<ShotDownComponent>(out shotDown))
                {
                    result[i, 1] = shotDown.time;
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

    public void HandlePlayerDeath(Entity playerEntity)
    {
        Transform trans = EntityUtility.Instance.GetComponent<Transform>(playerEntity);
        if (trans != null && trans.position.y < deadLine.position.y)
        {
            //Dead
            Vector3 position = new Vector3(Random.Range(-2.5f, 2.5f), 2, 0);
            trans.position = position;
            Rigidbody2D rigidbody = trans.GetComponent<Rigidbody2D>();
            if(rigidbody != null)
            {
                rigidbody.velocity = Vector2.zero;
            }

            //write status
            CharacterBase character = trans.GetComponent<CharacterBase>();
            if(character != null)
            {
                ShotDownComponent shotDown;
                if(character.GetComponentData<ShotDownComponent>(out shotDown))
                {
                    shotDown.time += 1;
                }
                else
                {
                    shotDown = new ShotDownComponent() { time = 1 };
                }
                character.SetComponentData<ShotDownComponent>(shotDown);
            }
        }
    }
}
