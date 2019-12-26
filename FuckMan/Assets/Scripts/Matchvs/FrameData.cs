using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum DataType
{
    Input = 0,
    Damage,
    PositionSync,
}


[Serializable]
public struct FrameData
{
    public DataType dataType;
    public object data;
}

[Serializable]
public struct InputData
{
    public MoveComponent move;
    public JumpComponent jump;
    public SimpleAttackComponent simpleAttack;
}

[Serializable]
public struct PositionData
{
    public float x;
    public float y;
    public float z;

    public PositionData(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

