using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct FrameData
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

