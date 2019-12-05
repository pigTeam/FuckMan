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
        SetupInputComponent();
        SetupAnimationComponent();
        SetupJumpComponent();
        SetupSimpleAttackComponent();
    }

    public void bindUser(uint uid)
    {
        userID = uid;
        SetUpUserDataComponent(uid);
        if (userID == Game.UserID)
        {
            // self

        }
    }
    // Update is called once per frame
    void Update()
    {
    }

    #region Setup Components

    void SetUpUserDataComponent(uint id)
    {
        var user = new UserDataComponent() { userID = id };
        entityManager.AddComponentData(thisEntity, user);
    }

    void SetUpMoveComponent()
    {
        var moveCmp = new MoveComponent() { maxSpeed = moveSpeed };
        entityManager.AddComponentData(thisEntity, moveCmp);

        var frameMove = new FrameMoveComponent() { moves = new List<Movement>() };
        entityManager.AddComponentData(thisEntity, frameMove);
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

        var jumpFrame = new FrameJumpComponent() { jumps = new List<Jumpment>()};
        entityManager.AddComponentData(thisEntity, jumpFrame);
    }

    void SetupSimpleAttackComponent()
    {
        var simpleAtkCmp = new SimpleAttackComponent() {  };
        entityManager.AddComponentData(thisEntity, simpleAtkCmp);

        var attackFrame = new FrameAttackComponent() { attacks = new List<Attackment>()};
        entityManager.AddComponentData(thisEntity, attackFrame);
    }
    #endregion

}
