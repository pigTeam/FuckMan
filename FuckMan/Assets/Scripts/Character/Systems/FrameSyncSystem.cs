using UnityEngine;
using System.Collections;
using Unity.Entities;

public class FrameSyncSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<UserDataComponent, FrameAttackComponent, FrameJumpComponent, FrameMoveComponent>().ForEach((Entity id, ref UserDataComponent user, ref FrameAttackComponent attack, ref FrameJumpComponent jump, ref FrameMoveComponent move) => {

            if (user.userID > 0 && Game.frameNotifies.Count > 0)
            {
                var frameData = Game.frameNotifies[0];
                if (frameData.SrcUid == user.userID)
                {
                    var frame = DataUtil.Deserialize<FrameData>(frameData.CpProto);
                    move.moves.Add(frame.move);
                    jump.jumps.Add(frame.jump);
                    attack.attacks.Add(frame.simpleAttack);
                }
            }
        });
    }
}
