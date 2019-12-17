using UnityEngine;
using System.Collections;
using Unity.Entities;
using System.Collections.Generic;

public class MoveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {

        Entities.WithAllReadOnly<MoveComponent>().ForEach((Entity id, ref MoveComponent move) =>
        {
            Transform transform = EntityUtility.Instance.GetComponent<Transform>(id);
            if (transform != null)
            {
                transform.Translate(Vector3.right * move.speed * Time.deltaTime);
                SpriteRenderer spriteRenderer = EntityUtility.Instance.GetComponent<SpriteRenderer>(id);
                if (spriteRenderer != null && move.speed != 0)
                {
                    spriteRenderer.flipX = move.speed < 0 ? true : false;
                }
            }

        });


    }
}
