using UnityEngine;

[CreateAssetMenu(menuName = "Entities/EntityData", fileName = "EntityData")]
public class EntityData : ScriptableObject
{
    public string displayedName;
    public EntityPoolRef entityPoolRef;

    [Header("Stats (Replace by stat system later)")]
    public float maxHealth;
    public float moveSpeed;
    public float attackSpeed;
    public float attackDamage;
    public float attackRange;
}