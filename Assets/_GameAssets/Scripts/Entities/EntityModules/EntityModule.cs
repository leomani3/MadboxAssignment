using UnityEngine;

public abstract class EntityModule : MonoBehaviour
{
    public Entity Owner { get; private set; }
    
    internal void Initialize(Entity owner)
    {
        Owner = owner;
        OnInitialize();
    }
    
    protected virtual void OnInitialize() { }
    public virtual void OnAllModuleInitialized() { }
}