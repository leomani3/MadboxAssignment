using System;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private EntityData entityData;
    [SerializeField] private Collider m_collider;
    [SerializeField] private GameObject m_targetedIndicatorObject;
    
    private Dictionary<Type, EntityModule> m_modules = new Dictionary<Type, EntityModule>();
    private bool m_canBeTargeted;
    private LeanEntityPool m_originPool;
    
    public bool CanBeTargeted
    {
        get => m_canBeTargeted;
        set
        {
            m_canBeTargeted = value;
            if (!m_canBeTargeted)
            {
                SetTargeted(false);
            }
        }
    }
    
    public EntityData EntityData => entityData;
    public Collider Collider => m_collider;

    public void Setup(LeanEntityPool originPool)
    {
        m_originPool = originPool;
    }
    
    protected virtual void Awake()
    {
        RegisterModules();
    }

    private void Start()
    {
        EntityManager.Instance?.Register(this);
    }

    public void SetTargeted(bool targeted)
    {
        if (m_targetedIndicatorObject != null)
            m_targetedIndicatorObject.SetActive(targeted);
    }

    private void RegisterModules()
    {
        var modules = GetComponents<EntityModule>();
        foreach (var module in modules)
        {
            var type = module.GetType();
            while (type != null && typeof(EntityModule).IsAssignableFrom(type))
            {
                m_modules.TryAdd(type, module);
                type = type.BaseType;
            }
        
            module.Initialize(this);
        }

        if (TryGetModule(out EntityHealthModule healthModule))
        {
            healthModule.OnDeath += DespawnAnUnregister;
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

    private void DespawnAnUnregister()
    {
        if (TryGetModule(out EntityHealthModule healthModule))
        {
            healthModule.OnDeath -= DespawnAnUnregister;
        }
        
        if (m_originPool != null)
        {
            m_originPool.Despawn(this);
        }
        
        EntityManager.Instance?.Unregister(this);
    }
}