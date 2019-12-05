using UnityEngine;
using System.Collections;
using Unity.Entities;

public class MoveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAllReadOnly<MoveComponent>().ForEach((Entity id, ref MoveComponent moveData) => {
            Transform transform = EntityUtility.Instance.GetComponent<Transform>(id);
            if(transform != null)
            {
                transform.Translate(Vector3.right * moveData.speed * Time.deltaTime);
            }

            SpriteRenderer spriteRenderer = EntityUtility.Instance.GetComponent<SpriteRenderer>(id);
            if(spriteRenderer != null && moveData.speed != 0)
            {
                spriteRenderer.flipX = moveData.speed < 0 ? true : false;
            }
        });    
    }
}
