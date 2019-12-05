using UnityEngine;
using System.Collections;
using Unity.Entities;

public class SimpleAttackSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<SimpleAttackComponent>().ForEach((Entity id, ref SimpleAttackComponent attackData) => { 
            if(attackData.attackTrigger)
            {
                attackData.attackTrigger = false;
            }
        });
    }

}
