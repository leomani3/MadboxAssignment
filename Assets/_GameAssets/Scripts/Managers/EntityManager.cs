using System;
using System.Collections.Generic;
using MyBox;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

public class EntityManager : Singleton<EntityManager>
{
    private Entity m_player = new();
    private HashSet<Entity> m_enemies = new();
    private HashSet<Entity> m_entities = new();
    
    public HashSet<Entity> AllEntities => m_entities;
    public HashSet<Entity> Enemies => m_enemies;
    public Entity Player => m_player;
    
    internal void Register(Entity entity)
    {
        if (entity == null) return;

        if (!m_entities.Add(entity))
        {
            Debug.LogWarning($"[EntityManager] Entity '{entity.name}' ({entity.EntityData.displayedName}) is already registered.");
            return;
        }

        if (entity.TryGetModule(out EntityTeamModule teamModule))
        {
            if (teamModule.Team == Team.Player)
            {
                m_player = entity;
            }
            else if (teamModule.Team == Team.Enemy)
            {
                m_enemies.Add(entity);
            }
        }
        
        Debug.Log($"[EntityManager] Registered '{entity.name}' | id={entity.EntityData.displayedName} | total={m_entities.Count}");
    }

    internal void Unregister(Entity entity)
    {
        if (entity == null) return;

        if (!m_entities.Remove(entity))
            return;
        
        Debug.Log($"[EntityManager] Unregistered '{entity.name}' | id={entity.EntityData.displayedName} | total={m_entities.Count}");
    }
}