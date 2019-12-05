using UnityEngine;
using System.Collections;
using Unity.Entities;

public class SimpleAttackSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<FrameAttackComponent, SimpleAttackComponent>().ForEach((Entity id, ref FrameAttackComponent attackData, ref SimpleAttackComponent attack) => { 
           if (attackData.attacks.Count > 0)
            {
                var data = attackData.attacks[0];
                attack.attackTrigger = data.attackTrigger;
                attackData.attacks.Remove(data);
            }
        });
    }

}
