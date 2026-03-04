using DG.Tweening;
using UnityEngine;

public class EntitySheenModule : EntityModule
{
    [SerializeField, Min(0f)] private float sheenHoldDuration = 0.05f;
    [SerializeField, Min(0f)] private float sheenFadeDuration = 0.15f;
    
    private Renderer[] m_renderers;
    private Color[] m_originalColors;
    private Sequence m_sheenSequence;
    
    private const string EMISSION_COLOR_PROPERTY = "_EmissionColor";
    
    protected override void OnInitialize()
    {
        base.OnInitialize();
        
        CacheRenderers();
    }

    private void CacheRenderers()
    {
        m_renderers = Owner.GetComponentsInChildren<Renderer>(includeInactive: true);
        m_originalColors = new Color[m_renderers.Length];

        for (int i = 0; i < m_renderers.Length; i++)
        {
            Material mat = m_renderers[i].material;
            
            mat.EnableKeyword("_EMISSION");

            m_originalColors[i] = mat.HasProperty(EMISSION_COLOR_PROPERTY)
                ? mat.GetColor(EMISSION_COLOR_PROPERTY)
                : Color.black;
        }
    }
    
    public void PlayWhiteSheen()
    {
        m_sheenSequence?.Kill(complete: false);
        m_sheenSequence = DOTween.Sequence().SetLink(Owner.gameObject);

        for (int i = 0; i < m_renderers.Length; i++)
        {
            if (m_renderers[i] == null) continue;

            Material mat = m_renderers[i].material;
            if (!mat.HasProperty(EMISSION_COLOR_PROPERTY)) continue;

            Color original = m_originalColors[i];
            
            mat.SetColor(EMISSION_COLOR_PROPERTY, Color.white);
            
            Tween fade = mat
                .DOColor(original, EMISSION_COLOR_PROPERTY, sheenFadeDuration)
                .SetDelay(sheenHoldDuration)
                .SetEase(Ease.OutQuad);

            m_sheenSequence.Join(fade);
        }

        m_sheenSequence.Play();
    }
}