using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class EntityManager : Singleton<EntityManager>
{
    public Action<Entity> OnEntityRegistered;
    public Action<Entity> OnEntityUnregistered;
    
    private readonly HashSet<Entity> m_entities = new();
    
    public HashSet<Entity> AllEntities => m_entities;

    internal void Register(Entity entity)
    {
        if (entity == null) return;

        if (!m_entities.Add(entity))
        {
            Debug.LogWarning($"[EntityManager] Entity '{entity.name}' ({entity.EntityData.displayedName}) is already registered.");
            return;
        }
        
        OnEntityRegistered?.Invoke(entity);
        Debug.Log($"[EntityManager] Registered '{entity.name}' | id={entity.EntityData.displayedName} | total={m_entities.Count}");
    }

    internal void Unregister(Entity entity)
    {
        if (entity == null) return;

        if (!m_entities.Remove(entity))
            return;
        
        OnEntityUnregistered?.Invoke(entity);
        Debug.Log($"[EntityManager] Unregistered '{entity.name}' | id={entity.EntityData.displayedName} | total={m_entities.Count}");
    }
}