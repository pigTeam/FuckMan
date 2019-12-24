using UnityEngine;
using System.Collections;
using Unity.Entities;

public class PositionSyncSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<UserDataComponent>().ForEach((Entity id, ref UserDataComponent userData) =>
        {
            if (userData.userID != GameNetWork.UserID)
            {
                Vector3 checkPosition;
                if (GameNetWork.framePositionChecks.TryGetValue(userData.userID, out checkPosition))
                {
                    Transform trans = EntityUtility.Instance.GetComponent<Transform>(id);
                    if (Vector3.Distance(trans.position, checkPosition) > GameConfig.checkPositionThreshold)
                    {
                        if(GameConfig.needSyncPosition)
                        {
                            trans.position = checkPosition;
                        }
                    }

                    GameNetWork.framePositionChecks.Remove(userData.userID);
                }
            }
        });
    }

}
