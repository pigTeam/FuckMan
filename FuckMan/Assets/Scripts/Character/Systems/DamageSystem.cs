using UnityEngine;
using System.Collections;
using Unity.Entities;

public class DamageSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<DamageComponent, InjuryComponent>()
            .ForEach((Entity id,ref DamageComponent damage,ref InjuryComponent injury)=> {
                Debug.LogError("Get damage from " + damage.fromUserId + ",damge = " + damage.value);
                injury.value += damage.value;
                EntityManager.RemoveComponent(id, typeof(DamageComponent));
            });
    }

}
