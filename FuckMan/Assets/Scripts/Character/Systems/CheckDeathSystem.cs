using UnityEngine;
using System.Collections;
using Unity.Entities;

public class CheckDeathSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAllReadOnly<UserDataComponent>().ForEach((Entity id, ref UserDataComponent userData) =>
        {
            GameManager.Inst.HandlePlayerDeath(id);
        });
    }

}
