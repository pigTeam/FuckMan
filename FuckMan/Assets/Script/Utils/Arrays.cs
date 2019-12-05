

using System;

public class ArrayUtil  {

    public static object[] splice(object[] s,int index,int length) {
        object[] newA = new object[length];
        Array.Copy(s, index, newA, 0, length);
        return newA;
    }
}
