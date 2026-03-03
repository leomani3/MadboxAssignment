using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class EntityManager : Singleton<EntityManager>
{
    private Entity m_player;
    private HashSet<Entity> m_enemies = new HashSet<Entity>();
    private HashSet<Entity> m_entities = new HashSet<Entity>();
    
    private Dictionary<Collider, Entity> m_entitiesByColliders = new Dictionary<Collider, Entity>();
    
    public HashSet<Entity> AllEntities => m_entities;
    public Dictionary<Collider, Entity> EntitiesByCollider => m_entitiesByColliders;
    public HashSet<Entity> Enemies => m_enemies;
    public Entity Player => m_player;
    
    internal void Register(Entity entity)
    {
        if (entity == null) return;

        if (!m_entities.Add(entity))
        {
            return;
        }

        if (!m_entitiesByColliders.ContainsKey(entity.Collider))
        {
            m_entitiesByColliders.Add(entity.Collider, entity);
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
    }

    internal void Unregister(Entity entity)
    {
        if (entity == null) return;

        if (!m_entities.Remove(entity))
            return;
        
        if (m_entitiesByColliders.ContainsKey(entity.Collider))
        {
            m_entitiesByColliders.Remove(entity.Collider);
        }
    }

    public Entity FindClosestEnemy(Entity sourceEntity)
    {
        Entity closest = null;
        float closestDistance = float.MaxValue;

        foreach (Entity entity in m_entities)
        {
            if (entity != sourceEntity && entity.IsEnemy(sourceEntity))
            {
                float distance = Vector3.Distance(sourceEntity.transform.position, entity.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = entity;
                }
            }
        }

        return closest;
    }
}