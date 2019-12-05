using UnityEngine;
using System.Collections;
using Unity.Entities;

public class JumpSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<JumpComponent>().ForEach((Entity id, ref JumpComponent jumpData) => {
            if(jumpData.jumpTrigger)
            {
                Rigidbody2D rigidbody2D = EntityUtility.Instance.GetComponent<Rigidbody2D>(id);
                if (rigidbody2D != null && rigidbody2D.IsTouchingLayers(LayerMask.GetMask("Land")))
                {
                    rigidbody2D.AddForce(new Vector2(0, jumpData.jumpForce), ForceMode2D.Impulse);
                }
                jumpData.jumpTrigger = false;
            }
        });
    }
}
