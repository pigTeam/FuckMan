using UnityEngine;
using System.Collections;
using Unity.Entities;
using System.Collections.Generic;

public class MoveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {

        //Entities.WithAllReadOnly<MoveComponent>().ForEach((Entity id, ref MoveComponent move) => {
        //    Transform transform = EntityUtility.Instance.GetComponent<Transform>(id);
        //    if (transform != null)
        //    {
        //        transform.Translate(Vector3.right * move.speed * Time.deltaTime);
        //        SpriteRenderer spriteRenderer = EntityUtility.Instance.GetComponent<SpriteRenderer>(id);
        //        if (spriteRenderer != null && move.speed != 0)
        //        {
        //            spriteRenderer.flipX = move.speed < 0 ? true : false;
        //        }
        //    }

        //});

        Entities.WithAllReadOnly<UserDataComponent, MoveComponent>().ForEach((Entity id, ref UserDataComponent userData, ref MoveComponent move) =>
        {
            Transform transform = EntityUtility.Instance.GetComponent<Transform>(id);
            if (transform != null)
            {
                List<MoveComponent> moveList;
                if (GameNetWork.frameMoves.TryGetValue(userData.userID, out moveList))
                {
                    if (moveList.Count > 0)
                    {
                        var data = moveList[0];
                        move.speed = data.speed;
                        transform.Translate(Vector3.right * move.speed * Time.deltaTime);
                        SpriteRenderer spriteRenderer = EntityUtility.Instance.GetComponent<SpriteRenderer>(id);
                        if (spriteRenderer != null && move.speed != 0)
                        {
                            spriteRenderer.flipX = move.speed < 0 ? true : false;
                        }

                        moveList.Remove(data);
                    }
                }

            }


        });
    }
}
