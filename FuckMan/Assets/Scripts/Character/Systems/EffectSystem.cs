using UnityEngine;
using System.Collections;
using Unity.Entities;

public class EffectSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAllReadOnly<EffectComponent,MoveComponent>()
            .ForEach((Entity id, ref EffectComponent effect,ref MoveComponent move) =>
            {
                EffectHandler effectHandler = EntityUtility.Instance.GetComponent<EffectHandler>(id);
                if (effectHandler != null)
                {
                    effectHandler.OnMoveSpeed(move.speed);
                }
            });
    }

   
}
