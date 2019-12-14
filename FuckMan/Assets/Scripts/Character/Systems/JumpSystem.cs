using UnityEngine;
using System.Collections;
using Unity.Entities;
using System.Collections.Generic;
public class JumpSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<UserDataComponent, JumpComponent>().ForEach((Entity id, ref UserDataComponent userData, ref JumpComponent jump) => {

            List<JumpComponent> jumpList;
            if (GameNetWork.frameJumps.TryGetValue(userData.userID, out jumpList))
            {
                if (jumpList.Count > 0)
                {
                    var data = jumpList[0];
                    if (data.jumpTrigger)
                    {
                        Rigidbody2D rigidbody2D = EntityUtility.Instance.GetComponent<Rigidbody2D>(id);
                        if (rigidbody2D != null && rigidbody2D.IsTouchingLayers(LayerMask.GetMask("Land")))
                        {
                            rigidbody2D.AddForce(new Vector2(0, data.jumpForce), ForceMode2D.Impulse);
                        }


                    }
                    jump.jumpTrigger = data.jumpTrigger;
                    jumpList.Remove(data);
                }

            }
            
        });
    }
}
