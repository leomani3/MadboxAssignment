using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Transform m_content;

    private Tween m_scaleTween;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_scaleTween.Kill();
        m_scaleTween = m_content.DOScale(1.1f, 0.2f).SetUpdate(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        m_scaleTween.Kill();
        m_scaleTween = m_content.DOScale(1, 0.2f).SetUpdate(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_scaleTween.Kill();
        m_scaleTween = m_content.DOScale(0.9f, 0.2f).SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_scaleTween.Kill();
        m_scaleTween = m_content.DOScale(1, 0.2f).SetUpdate(true);
    }
}