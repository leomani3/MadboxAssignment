using UnityEngine;

public abstract class EntityModule : MonoBehaviour
{
    public Entity Owner { get; private set; }
    public bool IsInitialized { get; private set; }
    
    internal void Initialize(Entity owner)
    {
        if (IsInitialized)
        {
            Debug.LogWarning($"[EntityModule] {GetType().Name} on '{name}' is already initialized.");
            return;
        }

        Owner = owner;
        IsInitialized = true;
        OnInitialize();
    }
    
    protected virtual void OnInitialize() { }
}