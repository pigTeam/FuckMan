﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Matchvs;
using System;

public class CharacterBase : MonoBehaviour
{
    protected EntityManager entityManager;
    protected Entity thisEntity;
    protected uint userID = 0;
    public float moveSpeed;
    public float jumpForce;
    public int simpleAttackDamage;

    protected bool _isNet;
    protected bool _isSelf;


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
        _isSelf = isSelf;
        _isNet = isNet;

        SetUpMoveComponent();
        SetupAnimationComponent();
        SetupJumpComponent();
        SetupSimpleAttackComponent();
        SetUpEffectComponent();
        SetUpUserDataComponent(uid, isSelf);
        SetUpInjuryComponent();
        SetUpShotDownComponent();
        if (isNet)
        {
            SetUpNetInputComponent();
            SetUpNetSyncComponent();
        }
        else if(isSelf)
        {
            SetupCustomInputComponent();
        }
    }
    // Update is called once per frame
    void Update()
    {
    }

    #region Setup Components

    void SetUpShotDownComponent()
    {
        entityManager.AddComponent(thisEntity, typeof(ShotDownComponent));
    }
    void SetUpInjuryComponent()
    {
        entityManager.AddComponent(thisEntity, typeof(InjuryComponent));
    }

    void SetUpEffectComponent()
    {
        entityManager.AddComponent(thisEntity, typeof(EffectComponent));
    }

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

    public virtual void HandleAnimEvent(AnimEvent eventType)
    {

    }

   

    public bool GetComponentData<T>(out T result) where T : struct, IComponentData
    {
        result = new T();
        return EntityUtility.Instance.GetComponentData<T>(thisEntity,out result);
    }

    public void SetComponentData<T>(T data) where T :struct,IComponentData
    {
        if(entityManager.HasComponent<T>(thisEntity))
        {
            entityManager.SetComponentData<T>(thisEntity, data);
        }
        else
        {
            entityManager.AddComponentData<T>(thisEntity, data);
        }
    }

    public virtual void DamageOtherCharacter(CharacterBase other,Vector2 dir)
    {
        //Debug.LogError(this.gameObject.name + "-->" + other.name + "===>" + simpleAttackDamage);
        var damage = new DamageComponent() 
        { fromUserId = userID, targetUserId = other.userID,
            value = simpleAttackDamage,dir = new Dir2D(dir) };

        if (_isNet)
        {
            if(_isSelf)
            {
                //Send DamageEvent
                FrameData frame = new FrameData() { dataType = DataType.Damage, data = damage };
                GameNetWork.Inst.SendFrameData(frame);
            }
        }
        else
        {
            other.SetComponentData<DamageComponent>(damage);
        }
    }
}
