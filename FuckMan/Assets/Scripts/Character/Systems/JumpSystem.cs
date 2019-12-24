using UnityEngine;
using System.Collections;
using Unity.Entities;
using System.Collections.Generic;
public class JumpSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<JumpComponent>().ForEach((Entity id, ref JumpComponent jump) => {

            if (jump.jumpTrigger)
            {
                Rigidbody2D rigidbody2D = EntityUtility.Instance.GetComponent<Rigidbody2D>(id);
                if (rigidbody2D != null && rigidbody2D.IsTouchingLayers(LayerMask.GetMask("Land")))
                {
                    rigidbody2D.AddForce(new Vector2(0, jump.jumpForce), ForceMode2D.Impulse);
                }
                jump.jumpTrigger = false;
            }

          
            
        });
    }
}
