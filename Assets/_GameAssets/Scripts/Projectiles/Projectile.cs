using Lean.Pool;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour, IPoolable
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

        ContactPoint contact = collision.GetContact(0);
        SpawnImpactVFX(contact.point, contact.normal);
        Despawn();
    }
    
    public static Projectile Launch(ProjectilePoolRef poolRef, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        if (poolRef == null || poolRef.pool == null)
        {
            Debug.LogWarning("[Projectile] Launch called with a null pool reference.");
            return null;
        }

        return LeanPool.Spawn(poolRef.pool.Prefab, spawnPosition, spawnRotation)
                       .GetComponent<Projectile>();
    }

    private void SpawnMuzzleFlash()
    {
        // if (muzzleFlashPoolRef == null || muzzleFlashPoolRef.pool == null)
        //     return;
        //
        // muzzleFlashPoolRef.pool.Spawn(transform.position, transform.rotation, muzzleFlashPoolRef.pool.transform);
    }

    private void SpawnImpactVFX(Vector3 position, Vector3 normal)
    {
        if (impactPoolRef == null || impactPoolRef.pool == null)
            return;

        Quaternion rotation = Quaternion.LookRotation(normal);
        ParticleSystem vfx = LeanPool.Spawn(
            impactPoolRef.pool.Prefab,
            position,
            rotation);

        if (vfx != null)
        {
            vfx.Play();
            float duration = vfx.main.duration + vfx.main.startLifetime.constantMax;
        }
    }

    private void Despawn()
    {
        LeanPool.Despawn(this);
    }

    public void OnSpawn()
    {
        spawnTime = Time.time;
        rb.velocity = transform.forward * speed;

        SpawnMuzzleFlash();
    }

    public void OnDespawn()
    {
        throw new System.NotImplementedException();
    }
}