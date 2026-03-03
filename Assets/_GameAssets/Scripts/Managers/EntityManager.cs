using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class EntityManager : Singleton<EntityManager>
{
    public Action<Entity> OnEntityRegistered;
    public Action<Entity> OnEntityUnregistered;

    private readonly Dictionary<string, Entity> m_entitiesById = new();
    private readonly HashSet<Entity> m_entities = new();
    
    public HashSet<Entity> AllEntities => m_entities;
    public Entity GetById(string id) => m_entitiesById.TryGetValue(id, out var e) ? e : null;

    internal void Register(Entity entity)
    {
        if (entity == null) return;

        if (!m_entities.Add(entity))
        {
            Debug.LogWarning($"[EntityManager] Entity '{entity.name}' ({entity.EntityId}) is already registered.");
            return;
        }

        m_entitiesById[entity.EntityId] = entity;
        OnEntityRegistered?.Invoke(entity);
        Debug.Log($"[EntityManager] Registered '{entity.name}' | id={entity.EntityId} | total={m_entities.Count}");
    }

    internal void Unregister(Entity entity)
    {
        if (entity == null) return;

        if (!m_entities.Remove(entity))
            return;

        m_entitiesById.Remove(entity.EntityId);
        OnEntityUnregistered?.Invoke(entity);
        Debug.Log($"[EntityManager] Unregistered '{entity.name}' | id={entity.EntityId} | total={m_entities.Count}");
    }
}