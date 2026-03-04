using System;
using System.Collections.Generic;
using MyBox;
using Sirenix.OdinInspector;
using UnityEngine;
using Lean.Pool;
using DG.Tweening;

public class EntityHealthModule : EntityModule
{
    [Header("Health Settings")]
    [SerializeField, Min(1f)] private float maxHealth = 100f;

    [Header("Health Bar")]
    [SerializeField] private HealthBarPoolRef healthBarPoolRef;
    // World-space offset applied to the entity's position when placing the bar
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0f, 2f, 0f);

    [Header("Damage Feedback – Punch Scale")]
    [SerializeField] private Vector3 punchScale = new Vector3(0.3f, -0.2f, 0f);
    [SerializeField, Min(0f)] private float punchDuration = 0.35f;
    [SerializeField, Min(1)] private int punchVibrato = 6;
    [SerializeField, Range(0f, 1f)] private float punchElasticity = 0.5f;

    [Header("Damage Feedback – White Sheen")]
    // How long the full white flash lasts before fading back
    [SerializeField, Min(0f)] private float sheenHoldDuration = 0.05f;
    [SerializeField, Min(0f)] private float sheenFadeDuration = 0.15f;

    public event Action<float, float, float> OnHealthChanged;
    public event Action<float, float> OnDamageTaken;
    public event Action<float, float> OnHealed;
    public event Action OnDeath;

    private float m_currentHealth;
    private bool m_isDead;
    private HealthBar m_healthBar;

    // Cached renderers and their original colours for the white-sheen effect
    private Renderer[] m_renderers;
    private Color[] m_originalColors;
    private Sequence m_sheenSequence;
    private Tween m_punchTween;

    // ─── Emission property for the white-sheen flash ───
    private static readonly int s_emissionColorID = Shader.PropertyToID("_EmissionColor");

    public float CurrentHealth => m_currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => m_isDead;
    public float HealthPercent => maxHealth > 0f ? m_currentHealth / maxHealth : 0f;

    protected override void OnInitialize()
    {
        m_currentHealth = maxHealth;
        m_isDead = false;

        CacheRenderers();
        SpawnHealthBar();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Renderer Cache
    // ─────────────────────────────────────────────────────────────────────────

    private void CacheRenderers()
    {
        m_renderers = Owner.GetComponentsInChildren<Renderer>(includeInactive: true);
        m_originalColors = new Color[m_renderers.Length];

        for (int i = 0; i < m_renderers.Length; i++)
        {
            Material mat = m_renderers[i].material; // creates instance automatically

            // Ensure the emission keyword is enabled so SetColor has a visible effect
            mat.EnableKeyword("_EMISSION");

            m_originalColors[i] = mat.HasProperty(s_emissionColorID)
                ? mat.GetColor(s_emissionColorID)
                : Color.black; // emission off by default = black
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Damage Feedback
    // ─────────────────────────────────────────────────────────────────────────

    private void PlayDamageFeedback()
    {
        PlayPunchScale();
        PlayWhiteSheen();
    }

    private void PlayPunchScale()
    {
        // Kill any running punch so hits don't stack oddly
        m_punchTween?.Kill(complete: true);
        Owner.transform.localScale = Vector3.one; // reset before new punch

        m_punchTween = Owner.transform
            .DOPunchScale(punchScale, punchDuration, punchVibrato, punchElasticity)
            .SetUpdate(UpdateType.Normal)
            .SetLink(Owner.gameObject);
    }

    private void PlayWhiteSheen()
    {
        m_sheenSequence?.Kill(complete: false);
        m_sheenSequence = DOTween.Sequence().SetLink(Owner.gameObject);

        for (int i = 0; i < m_renderers.Length; i++)
        {
            if (m_renderers[i] == null) continue;

            Material mat = m_renderers[i].material;
            if (!mat.HasProperty(s_emissionColorID)) continue;

            Color original = m_originalColors[i];

            // Snap emission to bright white immediately
            mat.SetColor(s_emissionColorID, Color.white);

            // Fade emission back to its original value (usually black = off)
            Tween fade = mat
                .DOColor(original, s_emissionColorID, sheenFadeDuration)
                .SetDelay(sheenHoldDuration)
                .SetEase(Ease.OutQuad);

            m_sheenSequence.Join(fade);
        }

        m_sheenSequence.Play();
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
        PlayDamageFeedback();

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

        // Kill feedback tweens cleanly on death
        m_punchTween?.Kill(complete: true);
        m_sheenSequence?.Kill(complete: false);

        Debug.Log($"[EntityHealthModule] '{Owner.name}' has died.");
        OnDeath?.Invoke();

        if (m_healthBar != null)
            m_healthBar.HideGauge(false, () => DespawnHealthBar());

        EntityManager.Instance?.Unregister(Owner);

        //Todo : start anim then despawn via pool
        // Destroy(Owner.gameObject);
    }
}