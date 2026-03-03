using Lean.Pool;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectilePoolRef", menuName = "ScriptableObjects/ProjectilePoolRef")]
public class ProjectilePoolRef : ScriptableObject
{
    public LeanProjectilePool pool;
}
