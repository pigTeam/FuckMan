using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System;

public enum AnimEvent
{
    SimpleAttackStart,
    SimpleAttackEnd,
}

public class AnimationEventListener : MonoBehaviour
{

    private static Dictionary<Entity, AnimationEventListener> listeners = new Dictionary<Entity, AnimationEventListener>();

    public static bool HasListenerSetup(Entity id)
    {
        return listeners.ContainsKey(id);
    }

    public static AnimationEventListener SetUp(Entity id,Action<Entity,AnimEvent> action)
    {
        AnimationEventListener listener = null;
        if (World.Active.EntityManager.Exists(id))
        {
            Transform transform = EntityUtility.Instance.GetComponent<Transform>(id);
            if(transform != null)
            {
                listener = transform.GetComponent<AnimationEventListener>();
                if(listener == null)
                {
                    listener = transform.gameObject.AddComponent<AnimationEventListener>();
                    listener.onAnimEvent += action;
                    listener.entity = id;
                }

                if(listener != null && !listeners.ContainsKey(id))
                {
                    listeners.Add(id, listener);
                }
            }
        }

        return listener;
    }

    public Action<Entity,AnimEvent> onAnimEvent;
    public Entity entity;

    void OnAnimEvent(string name)
    {
        AnimEvent eventType;
        Enum.TryParse<AnimEvent>(name, out eventType);
        
        if(onAnimEvent != null)
        {
            onAnimEvent.Invoke(entity, eventType);
        }
    }
}
