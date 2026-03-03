using System;
using UnityEngine;

public class EntityHealthModule : EntityModule
{
    [Header("Health Settings")]
    [SerializeField, Min(1f)] private float maxHealth = 100f;
    [SerializeField] private float startingHealth = -1f;   // -1 → use maxHealth
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField, Min(0f)] private float destroyDelay = 0f;
    
    public event Action<float, float, float> OnHealthChanged;
    public event Action<float, float> OnDamageTaken;
    public event Action<float, float> OnHealed;
    
    public event Action<Entity> OnDeath;
    
    private float m_currentHealth;
    private bool m_isDead;

    public float CurrentHealth => m_currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => m_isDead;
    public float HealthPercent => maxHealth > 0f ? m_currentHealth / maxHealth : 0f;
    
    protected override void OnInitialize()
    {
        m_currentHealth = startingHealth < 0f ? maxHealth : Mathf.Clamp(startingHealth, 0f, maxHealth);
        m_isDead = false;
    }
    
    public void TakeDamage(float amount)
    {
        if (m_isDead || amount <= 0f) return;

        float previous = m_currentHealth;
        m_currentHealth = Mathf.Max(0f, m_currentHealth - amount);
        float delta = m_currentHealth - previous;   // always negative here

        OnDamageTaken?.Invoke(amount, m_currentHealth);
        OnHealthChanged?.Invoke(m_currentHealth, maxHealth, delta);

        if (m_currentHealth <= 0f)
            Die();
    }
    
    public void Heal(float amount)
    {
        if (m_isDead || amount <= 0f) return;

        float previous = m_currentHealth;
        m_currentHealth = Mathf.Min(maxHealth, m_currentHealth + amount);
        float delta = m_currentHealth - previous;   // always positive here

        OnHealed?.Invoke(amount, m_currentHealth);
        OnHealthChanged?.Invoke(m_currentHealth, maxHealth, delta);
    }
    
    public void RestoreFullHealth()
    {
        if (m_isDead) return;

        float previous = m_currentHealth;
        m_currentHealth = maxHealth;

        OnHealthChanged?.Invoke(m_currentHealth, maxHealth, m_currentHealth - previous);
    }
    
    public void SetMaxHealth(float newMax, bool scaleCurrentHealth = false)
    {
        if (newMax <= 0f) return;

        if (scaleCurrentHealth && maxHealth > 0f)
            m_currentHealth = m_currentHealth / maxHealth * newMax;

        maxHealth = newMax;
        m_currentHealth = Mathf.Clamp(m_currentHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(m_currentHealth, maxHealth, 0f);
    }
    
    public void InstantKill()
    {
        if (m_isDead) return;
        m_currentHealth = 0f;
        Die();
    }
    
    public void Revive(float healthOnRevive = -1f)
    {
        if (!m_isDead) return;
        m_isDead = false;
        m_currentHealth = healthOnRevive < 0f
            ? maxHealth
            : Mathf.Clamp(healthOnRevive, 0f, maxHealth);

        OnHealthChanged?.Invoke(m_currentHealth, maxHealth, m_currentHealth);
    }
    
    private void Die()
    {
        if (m_isDead) return;
        m_isDead = true;

        Debug.Log($"[EntityHealthModule] '{Owner.name}' has died.");
        OnDeath?.Invoke(Owner);

        if (destroyOnDeath)
            Destroy(Owner.gameObject, destroyDelay);
    }
}