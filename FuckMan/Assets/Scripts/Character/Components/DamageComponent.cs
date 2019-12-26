using UnityEngine;
using System.Collections;
using Unity.Entities;
using System;

[Serializable]
public struct DamageComponent : IComponentData
{
    public int value;
    public uint fromUserId;
    public uint targetUserId;
    public Dir2D dir;
}

[Serializable]
public struct Dir2D
{
    public float x;
    public float y;

    public Dir2D(Vector2 vector2)
    {
        x = vector2.x;
        y = vector2.y;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
}
