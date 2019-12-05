using UnityEngine;
using System.Collections;
using Unity.Entities;

public class MoveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAllReadOnly<FrameMoveComponent, MoveComponent>().ForEach((Entity id, ref FrameMoveComponent moveData, ref MoveComponent move) => {
            Transform transform = EntityUtility.Instance.GetComponent<Transform>(id);
            if(transform != null && moveData.moves.Count > 0)
            {
                var data = moveData.moves[0];
                move.speed = data.speed;
                transform.Translate(Vector3.right * data.speed * Time.deltaTime);
                SpriteRenderer spriteRenderer = EntityUtility.Instance.GetComponent<SpriteRenderer>(id);
                if (spriteRenderer != null && data.speed != 0)
                {
                    spriteRenderer.flipX = data.speed < 0 ? true : false;
                }

                moveData.moves.Remove(data);
            }

           
        });    
    }
}
