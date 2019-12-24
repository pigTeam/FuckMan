using UnityEngine;
using System.Collections;
using Unity.Entities;
using System.Collections.Generic;

public class NetSyncSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<NetSyncComponent, MoveComponent, UserDataComponent>()
            .ForEach((Entity id, ref MoveComponent move, ref UserDataComponent user) => {

                List<MoveComponent> moveList;
                if (GameNetWork.frameMoves.TryGetValue(user.userID, out moveList))
                {
                    if (moveList.Count > 0)
                    {
                        var data = moveList[0];
                        move.speed = data.speed;

                        moveList.Remove(data);
                    }
                }
            });

        Entities.WithAll<NetSyncComponent, UserDataComponent, SimpleAttackComponent>()
            .ForEach((Entity id, ref SimpleAttackComponent atk, ref UserDataComponent user) => {

                List<SimpleAttackComponent> attackList;
                if (GameNetWork.frameAttacks.TryGetValue(user.userID, out attackList))
                {
                    if (attackList.Count > 0)
                    {
                        var data = attackList[0];
                        atk.attackTrigger = data.attackTrigger;
                        attackList.Remove(data);
                    }
                }
            });

        Entities.WithAll<NetSyncComponent, UserDataComponent, JumpComponent>()
            .ForEach((Entity id, ref JumpComponent jump, ref UserDataComponent user) => {

                List<JumpComponent> jumpList;
                if (GameNetWork.frameJumps.TryGetValue(user.userID, out jumpList))
                {
                    if (jumpList.Count > 0)
                    {
                        var data = jumpList[0];

                        jump.jumpTrigger = data.jumpTrigger;
                        jumpList.Remove(data);
                    }

                }
            });
    }

}
