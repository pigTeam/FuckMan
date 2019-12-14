using UnityEngine;
using System.Collections;

public class UIUtil : Singleton<UIUtil>
{
    public T GetComponentInChildren<T>(Transform target,string name) where T:Component
    {
        T result = null;

        if(target != null)
        {
            T[] cmps = target.GetComponentsInChildren<T>();
            for (int i = 0; i < cmps.Length; i++)
            {
                if(cmps[i].name==name)
                {
                    result = cmps[i];
                    break;
                }
            }
        }


        return result;
    }
}
