using Lean.Pool;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField]private Rigidbody rb;
    
    [Header("Movement")]
    [SerializeField] private float speed = 30f;
    [SerializeField] private float maxLifetime = 5f;

    [Header("VFX Pools")]
    [SerializeField] private ParticleSystemPoolRef muzzleFlashPoolRef;
    [SerializeField] private ParticleSystemPoolRef impactPoolRef;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionMask = ~0;
    
    private float spawnTime;
    private LeanProjectilePool m_originPool;
    private float m_damage;

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (Time.time - spawnTime >= maxLifetime)
        {
            Despawn();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collisionMask & (1 << collision.gameObject.layer)) == 0)
            return;

        if (EntityManager.Instance.EntitiesByCollider.TryGetValue(collision.collider, out Entity entity))
        {
            if (entity.TryGetModule(out EntityHealthModule healthModule))
            {
                healthModule.TakeDamage(m_damage);
            }
        }

        ContactPoint contact = collision.GetContact(0);
        SpawnImpactVFX(contact.point);
        Despawn();
    }
    
    public void Launch(LeanProjectilePool originPool, Vector3 targetPos, float damage)
    {
        m_originPool = originPool;
        spawnTime = Time.time;
        rb.velocity = (targetPos - transform.position).normalized * speed;
        transform.rotation = Quaternion.LookRotation(rb.velocity);

        m_damage = damage;

        SpawnMuzzleFlash();
    }

    private void SpawnMuzzleFlash()
    {
        if (muzzleFlashPoolRef == null || muzzleFlashPoolRef.pool == null)
            return;
        
        muzzleFlashPoolRef.pool.Spawn(transform.position, transform.rotation, muzzleFlashPoolRef.pool.transform);
    }

    private void SpawnImpactVFX(Vector3 position)
    {
        if (impactPoolRef == null || impactPoolRef.pool == null)
            return;
        
        impactPoolRef.pool.Spawn(position, transform.rotation, impactPoolRef.pool.transform);
    }

    private void Despawn()
    {
        m_originPool.Despawn(this);
    }
}