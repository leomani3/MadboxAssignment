using System;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private EntityData entityData;
    
    private Dictionary<Type, EntityModule> m_modules = new();
    
    public EntityData EntityData => entityData;
    
    protected virtual void Awake()
    {
        RegisterModules();
    }

    protected virtual void OnEnable()
    {
        EntityManager.Instance?.Register(this);
    }

    protected virtual void OnDisable()
    {
        EntityManager.Instance?.Unregister(this);
    }
    
    private void RegisterModules()
    {
        var modules = GetComponents<EntityModule>();
        foreach (var module in modules)
        {
            var type = module.GetType();
            if (m_modules.TryAdd(type, module))
            {
                module.Initialize(this);
            }
            else
            {
                Debug.LogWarning($"[Entity] Duplicate module of type {type.Name} on '{name}'. Only the first one will be used.");
            }
        }
    }
    
    public T GetModule<T>() where T : EntityModule
    {
        if (m_modules.TryGetValue(typeof(T), out var module))
            return (T)module;

        throw new InvalidOperationException(
            $"[Entity] Module of type '{typeof(T).Name}' not found on entity '{name}'.");
    }
    
    public bool TryGetModule<T>(out T module) where T : EntityModule
    {
        if (m_modules.TryGetValue(typeof(T), out var raw))
        {
            module = (T)raw;
            return true;
        }

        module = null;
        return false;
    }

    public bool IsEnemy(Entity entity)
    {
        if (TryGetModule(out EntityTeamModule myTeamModule) && entity.TryGetModule(out EntityTeamModule otherTeamModule))
        {
            return otherTeamModule.EnemyTeam == myTeamModule.Team;
        }
        
        return false;
    }
    
    public bool HasModule<T>() where T : EntityModule => m_modules.ContainsKey(typeof(T));
}