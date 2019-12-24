using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataType
{
    
    public static int INPUT = 0;
    public static int DAMAGE = 1;
}


[Serializable]
public struct FrameData
{
    public int dataType;
    public object data;
}

[Serializable]
public struct InputData
{
    public MoveComponent move;
    public JumpComponent jump;
    public SimpleAttackComponent simpleAttack;
    public Vect3 position;
}

[Serializable]
public struct Vect3
{
    public float x;
    public float y;
    public float z;

    public Vect3(Vector3 vector3)
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

