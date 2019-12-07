using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public class Knight : MonoBehaviour
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
    // Use this for initialization
    void Start()
    {
        GameObjectEntity gameObjectEntity = GetComponent<GameObjectEntity>();
        thisEntity = gameObjectEntity.Entity;
        entityManager = gameObjectEntity.EntityManager;

        SetUpMoveComponent();
        
        SetupAnimationComponent();
        SetupJumpComponent();
        SetupSimpleAttackComponent();
    }

    public void bindUser(uint uid)
    {
        GameObjectEntity gameObjectEntity = GetComponent<GameObjectEntity>();
        thisEntity = gameObjectEntity.Entity;
        entityManager = gameObjectEntity.EntityManager;
        userID = uid;
        SetUpUserDataComponent(uid, userID == Game.UserID);
        if (userID == Game.UserID)
        {
            // self
            SetupInputComponent();
        }
    }
    // Update is called once per frame
    void Update()
    {
    }

    #region Setup Components

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

    void SetupInputComponent()
    {
        entityManager.AddComponent(thisEntity, typeof(InputComponent));
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
