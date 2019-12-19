using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pool<T> where T : Object
{
    private List<T> pool = new List<T>();

    public virtual T GetItem()
    {
        T result = null;
        if (pool.Count > 0)
        {
            result = pool[0];
            pool.Remove(result);
        }
        else
        {
            result = CreateItem();
        }

        return result;
    }

    protected virtual T CreateItem()
    {
        return null;
    }

    public virtual void DestroyItem(T t)
    {

    }
   
}
