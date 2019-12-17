using UnityEngine;
using System.Collections;
using Unity.Entities;
using System.Collections.Generic;
public class SimpleAttackSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<SimpleAttackComponent>().ForEach((Entity id, ref SimpleAttackComponent attack) => {

    
        });
    }

}
