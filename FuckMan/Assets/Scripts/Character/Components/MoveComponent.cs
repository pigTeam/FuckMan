using UnityEngine;
using System.Collections;
using Unity.Entities;

public struct MoveComponent : IComponentData
{
    public float maxSpeed;
    public float speed;
}
