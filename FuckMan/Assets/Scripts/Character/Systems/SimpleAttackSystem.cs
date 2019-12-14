using UnityEngine;
using System.Collections;
using Unity.Entities;
using System.Collections.Generic;
public class SimpleAttackSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<UserDataComponent, SimpleAttackComponent>().ForEach((Entity id, ref UserDataComponent userData, ref SimpleAttackComponent attack) => {

            List<SimpleAttackComponent> attackList;
            if (GameNetWork.frameAttacks.TryGetValue(userData.userID, out attackList))
            {
                if (attackList.Count > 0)
                {
                    var data = attackList[0];
                    attack.attackTrigger = data.attackTrigger;
                    attackList.Remove(data);
                }
            }
        });
    }

}
