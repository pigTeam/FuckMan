using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public class CharacterBase : MonoBehaviour
{
    private EntityManager entityManager;
    private Entity thisEntity;
    private uint userID = 0;
    public float moveSpeed;
    public float jumpForce;

    public uint getUserID()
    {
        return this.userID;
    } 

    public void InintCharacter(uint uid,bool isSelf = true,bool isNet = false)
    {
        GameObjectEntity gameObjectEntity = GetComponent<GameObjectEntity>();
        thisEntity = gameObjectEntity.Entity;
        entityManager = gameObjectEntity.EntityManager;

        userID = uid;

        SetUpMoveComponent();
        SetupAnimationComponent();
        SetupJumpComponent();
        SetupSimpleAttackComponent();
        SetUpUserDataComponent(uid, isSelf);
        if (isNet)
        {
            SetUpNetInputComponent();
            SetUpNetSyncComponent();
        }
        else
        {
            SetupCustomInputComponent();
        }
    }
    // Update is called once per frame
    void Update()
    {
    }

    #region Setup Components

    void SetUpNetInputComponent()
    {
        entityManager.AddComponent(thisEntity, typeof(NetInputComponent));
    }

    void SetUpNetSyncComponent()
    {
        entityManager.AddComponent(thisEntity, typeof(NetSyncComponent));
    }


    void SetUpUserDataComponent(uint id, bool isSelf)
    {
        var user = new UserDataComponent() { userID = id ,isSelf = isSelf };
        entityManager.AddComponentData(thisEntity, user);
    }

    void SetUpMoveComponent()
    {
        var moveCmp = new MoveComponent() { maxSpeed = moveSpeed };
        entityManager.AddComponentData(thisEntity, moveCmp);

    }

    void SetupCustomInputComponent()
    {
        entityManager.AddComponent(thisEntity, typeof(CustomInputComponent));
    }

    void RemoveCustomInputComponent()
    {
        if(entityManager.HasComponent<CustomInputComponent>(thisEntity))
        {
            entityManager.RemoveComponent<CustomInputComponent>(thisEntity);
        }
    }

    void SetupAnimationComponent()
    {
        entityManager.AddComponent(thisEntity, typeof(AnimationComponent));
    }

    void SetupJumpComponent()
    {
        var jumpCmp = new JumpComponent() { jumpForce = this.jumpForce };
        entityManager.AddComponentData(thisEntity, jumpCmp);

    }

    void SetupSimpleAttackComponent()
    {
        var simpleAtkCmp = new SimpleAttackComponent() {  };
        entityManager.AddComponentData(thisEntity, simpleAtkCmp);

    }
    #endregion

}
