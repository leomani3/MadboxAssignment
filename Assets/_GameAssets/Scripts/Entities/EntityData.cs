using UnityEngine;

[CreateAssetMenu(menuName = "Entities/EntityData", fileName = "EntityData")]
public class EntityData : ScriptableObject
{
    public string displayedName;
    public EntityPoolRef entityPoolRef;
}