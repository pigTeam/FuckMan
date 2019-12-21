using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public class EntityUtility : Singleton<EntityUtility>
{
    private Dictionary<Entity, Dictionary<string, Component>> entity_Components = new Dictionary<Entity, Dictionary<string, Component>>();

    private Dictionary<Entity, Dictionary<string, IComponentData>> entity_ComponentDatas = new Dictionary<Entity, Dictionary<string, IComponentData>>();

    public T GetComponent<T>(Entity entity) where T : Component
    {
        T result = null;
        string typeName = typeof(T).Name;

        Dictionary<string, Component> dic;
        if (entity_Components.TryGetValue(entity, out dic))
        {
            Component mb;
            if (dic.TryGetValue(typeName, out mb))
            {
                result = (T)mb;
            }
            else
            {
                result = GetComponentFromEntity<T>(entity);
                if(result != null)
                {
                    dic.Add(typeName, result);
                }
            }
        }
        else
        {
            result = GetComponentFromEntity<T>(entity);
            if (result != null)
            {
                dic = new Dictionary<string, Component>();
                dic.Add(typeName, result);
                entity_Components.Add(entity, dic);
            }
        }


        return result;
    }

    private T GetComponentFromEntity<T>(Entity entity) where T: Component
    {
        T result = null;
        if (World.Active.EntityManager.Exists(entity))
        {
            if(World.Active.EntityManager.HasComponent<T>(entity))
            {
                result = World.Active.EntityManager.GetComponentObject<T>(entity);
                if(result == null)
                {
                    Debug.LogError("GetComponentFromEntity Error:" + typeof(T).Name);
                }
            }
            else
            {
                Debug.LogError("Entity haven't component :" + typeof(T).Name);
            }
        }
        else 
        {
            Debug.LogError("entity does not exist!" + entity);
        }

        return result;
    }

    public T RemoveAndPushEntityComponentData<T>(Entity entity) where T : struct,IComponentData
    {
        if(World.Active.EntityManager.Exists(entity))
        {
            if(World.Active.EntityManager.HasComponent<T>(entity))
            {
                T component = World.Active.EntityManager.GetComponentData<T>(entity);
                Dictionary<string, IComponentData> dic;
                if (!entity_ComponentDatas.TryGetValue(entity,out dic))
                {
                    dic = new Dictionary<string, IComponentData>();
                    entity_ComponentDatas.Add(entity, dic);
                }

                string componentName = component.GetType().Name;
                if (dic.ContainsKey(componentName))
                {
                    dic.Remove(componentName);
                }
                dic.Add(componentName, component);
                World.Active.EntityManager.RemoveComponent<T>(entity);

                return component;
            }
        }
        return new T();
    }

    public T PopAndAddEntityComponentData<T>(Entity entity) where T : struct,IComponentData
    {
        EntityManager entityManager = World.Active.EntityManager;
        if(entityManager.Exists(entity))
        {
            Dictionary<string, IComponentData> dic;
            if(entity_ComponentDatas.TryGetValue(entity,out dic))
            {
                string componentName = typeof(T).Name;
                IComponentData d;
                if(dic.TryGetValue(componentName,out d))
                {
                    T data = (T)d;

                    dic.Remove(componentName);
                    entityManager.AddComponentData(entity, data);
                    return data;
                }
            }
        }
        return new T();
    }

    public bool GetComponentData<T>(Entity entity,out T result) where T : struct, IComponentData
    {
        if (World.Active.EntityManager.Exists(entity))
        {
            if (World.Active.EntityManager.HasComponent<T>(entity))
            {
                result = World.Active.EntityManager.GetComponentData<T>(entity);

                return true;
            }
        }
        result = new T();
        return false;
    }
}
