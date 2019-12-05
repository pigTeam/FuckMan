using UnityEngine;
using System.Collections;
using Unity.Entities;

public class Knight : MonoBehaviour
{
    private EntityManager entityManager;
    private Entity thisEntity;

    public float moveSpeed;
    public float jumpForce;

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


    // Update is called once per frame
    void Update()
    {
    }

    #region Setup Components

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
