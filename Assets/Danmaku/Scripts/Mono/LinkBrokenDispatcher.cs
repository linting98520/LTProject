using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public enum LinkType : byte
{
    None = 0,
    BuildingCell = 1
}

public class LinkBrokenDispatcher : MonoBehaviour
{
    private EntityQuery eventQuery;
    private Dictionary<LinkType, Action<int>> handlers = new Dictionary<LinkType, Action<int>>();

    private void Awake()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        eventQuery = em.CreateEntityQuery(typeof(EntityLinkBrokenEvent));
    }

    public void Register(LinkType type, Action<int> handler)
    {
        if (handlers.TryGetValue(type, out Action<int> action))
        {
            handlers[type] += handler;
        }
        else
        {
            handlers[type] = handler;
        }
    }

    public void Unregister(LinkType type, Action<int> handler)
    {
        if (handlers.TryGetValue(type, out Action<int> action))
        {
            handlers[type] -= handler;
        }
    }

    private void Update()
    {
        if (eventQuery == null) return;

        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var events = eventQuery.ToComponentDataArray<EntityLinkBrokenEvent>(Allocator.Temp);
        var entities = eventQuery.ToEntityArray(Allocator.Temp);

        for (int i = 0; i < events.Length; i++)
        {
            var evt = events[i];
            if (handlers.TryGetValue(evt.Type, out var handler))
            {
                handler?.Invoke(evt.LinkedInstanceID);
            }
            entityManager.DestroyEntity(entities[i]);
        }

        events.Dispose();
        entities.Dispose();
    }
}
