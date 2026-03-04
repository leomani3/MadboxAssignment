using System;
using MyBox;
using Sirenix.OdinInspector;
using UnityEngine;
using Lean.Pool;

public class EntityHealthModule : EntityModule
{
    [Header("Health Settings")]
    [SerializeField, Min(1f)] private float maxHealth = 100f;

    [Header("Health Bar")]
    [SerializeField] private HealthBarPoolRef healthBarPoolRef;
    // World-space offset applied to the entity's position when placing the bar
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0f, 2f, 0f);

    public event Action<float, float, float> OnHealthChanged;
    public event Action<float, float> OnDamageTaken;
    public event Action<float, float> OnHealed;
    public event Action OnDeath;

    private float m_currentHealth;
    private bool m_isDead;
    private HealthBar m_healthBar;

    public float CurrentHealth => m_currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => m_isDead;
    public float HealthPercent => maxHealth > 0f ? m_currentHealth / maxHealth : 0f;

    protected override void OnInitialize()
    {
        m_currentHealth = maxHealth;
        m_isDead = false;

        SpawnHealthBar();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Health Bar
    // ─────────────────────────────────────────────────────────────────────────

    private void SpawnHealthBar()
    {
        // Spawn into the game canvas so it renders in screen space
        Transform canvasTransform = UIManager.Instance.GameCanvas.transform;
        m_healthBar = healthBarPoolRef.pool.Spawn(canvasTransform);

        // Anchor the bar to a point above the entity and set initial value
        Transform trackTarget = GetOffsetTrackingTransform();
        m_healthBar.Setup(trackTarget, m_currentHealth, maxHealth);
    }

    private void DespawnHealthBar()
    {
        if (m_healthBar == null) return;
        healthBarPoolRef.pool.Despawn(m_healthBar);
        m_healthBar = null;
    }

    // Returns a cached child Transform that sits at healthBarOffset above the entity.
    // GenericGauge.Update() will track it each frame via WorldToScreenPoint.
    private Transform GetOffsetTrackingTransform()
    {
        const string anchorName = "_HealthBarAnchor";
        Transform existing = Owner.transform.Find(anchorName);
        if (existing != null) return existing;

        GameObject anchor = new GameObject(anchorName);
        anchor.transform.SetParent(Owner.transform, worldPositionStays: false);
        anchor.transform.localPosition = healthBarOffset;
        return anchor.transform;
    }

    private void UpdateHealthBar()
    {
        if (m_healthBar == null) return;
        m_healthBar.SetValue(m_currentHealth, maxHealth);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Public API
    // ─────────────────────────────────────────────────────────────────────────

    [Button]
    public void TakeDamage(float amount)
    {
        if (m_isDead || amount <= 0f) return;

        float previous = m_currentHealth;
        m_currentHealth = Mathf.Max(0f, m_currentHealth - amount);
        float delta = m_currentHealth - previous;

        FloatingTextManager.Instance.SpawnUIText(
            CameraManager.Instance.MainCam.WorldToScreenPoint(transform.position.OffsetY(4)),
            delta.ToString(),
            GameConfig.Instance.m_normalDamageTextConfig);

        UpdateHealthBar();

        OnDamageTaken?.Invoke(amount, m_currentHealth);
        OnHealthChanged?.Invoke(m_currentHealth, maxHealth, delta);

        if (m_currentHealth <= 0f)
            Die();
    }

    [Button]
    public void Heal(float amount)
    {
        if (m_isDead || amount <= 0f) return;

        float previous = m_currentHealth;
        m_currentHealth = Mathf.Min(maxHealth, m_currentHealth + amount);
        float delta = m_currentHealth - previous;

        UpdateHealthBar();

        OnHealed?.Invoke(amount, m_currentHealth);
        OnHealthChanged?.Invoke(m_currentHealth, maxHealth, delta);
    }

    public void RestoreFullHealth()
    {
        if (m_isDead) return;

        float previous = m_currentHealth;
        m_currentHealth = maxHealth;

        UpdateHealthBar();

        OnHealthChanged?.Invoke(m_currentHealth, maxHealth, m_currentHealth - previous);
    }

    public void SetMaxHealth(float newMax, bool scaleCurrentHealth = false)
    {
        if (newMax <= 0f) return;

        if (scaleCurrentHealth && maxHealth > 0f)
            m_currentHealth = m_currentHealth / maxHealth * newMax;

        maxHealth = newMax;
        m_currentHealth = Mathf.Clamp(m_currentHealth, 0f, maxHealth);

        UpdateHealthBar();

        OnHealthChanged?.Invoke(m_currentHealth, maxHealth, 0f);
    }

    private void Die()
    {
        if (m_isDead) return;
        m_isDead = true;

        Debug.Log($"[EntityHealthModule] '{Owner.name}' has died.");
        OnDeath?.Invoke();
        
        if (m_healthBar != null)
            m_healthBar.HideGauge(false, () => DespawnHealthBar());

        EntityManager.Instance?.Unregister(Owner);
        
        //Todo : start anim then despawn via pool
        // Destroy(Owner.gameObject);
    }
}