using Lean.Pool;
using UnityEngine;

public class ProjectilePoolRefSetter : MonoBehaviour
{
    [SerializeField] private ProjectilePoolRef poolRef;

    private void Awake()
    {
        poolRef.pool = GetComponent<LeanProjectilePool>();
    }
}
