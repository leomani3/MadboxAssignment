using System;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Info")]
    [SerializeField] private string entityId;

    public string EntityId => entityId;
    private Dictionary<Type, EntityModule> m_modules = new();
    
    protected virtual void Awake()
    {
        if (string.IsNullOrEmpty(entityId))
            entityId = Guid.NewGuid().ToString();

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
    
    public bool HasModule<T>() where T : EntityModule => m_modules.ContainsKey(typeof(T));
}