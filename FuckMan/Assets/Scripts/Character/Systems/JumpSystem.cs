using UnityEngine;
using System.Collections;
using Unity.Entities;

public class JumpSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<FrameJumpComponent, JumpComponent>().ForEach((Entity id, ref FrameJumpComponent jumpData, ref JumpComponent jump) => {
            if (jumpData.jumps.Count > 0)
            {
                var data = jumpData.jumps[0];
                if (data.jumpTrigger)
                {
                    Rigidbody2D rigidbody2D = EntityUtility.Instance.GetComponent<Rigidbody2D>(id);
                    if (rigidbody2D != null && rigidbody2D.IsTouchingLayers(LayerMask.GetMask("Land")))
                    {
                        rigidbody2D.AddForce(new Vector2(0, data.jumpForce), ForceMode2D.Impulse);
                    }
                    
                    
                }
                jump.jumpTrigger = data.jumpTrigger;
                jumpData.jumps.Remove(data);

            }
            
        });
    }
}
