using DG.Tweening;
using UnityEngine;

public class CustomCanvas : MonoBehaviour
{
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private CanvasGroup m_canvasGroup;

    private Tween m_fadeTween;
    
    public virtual void Open(bool _instant = false)
    {
        m_fadeTween.Kill();
        
        m_canvas.enabled = true;
        m_canvasGroup.alpha = 0;

        if (_instant)
        {
            m_canvasGroup.alpha = 1;
        }
        else
        {
            m_fadeTween = m_canvasGroup.DOFade(1, 0.2f).SetUpdate(true);
        }

    }

    public virtual void Close(bool _instant = false)
    {
        m_fadeTween.Kill();

        if (_instant)
        {
            m_canvasGroup.alpha = 0;
            m_canvas.enabled = false;
        }
        else
        {
            m_fadeTween = m_canvasGroup.DOFade(0, 0.2f).SetUpdate(true);
            m_fadeTween.onComplete += () =>
            {
                m_canvas.enabled = false;
            };
        }
    }
}