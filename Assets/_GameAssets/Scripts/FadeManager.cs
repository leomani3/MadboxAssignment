using System;
using System.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

public class FadeManager : MyBox.Singleton<FadeManager>
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private float m_fadeDuration = 0.5f;
    [SerializeField] private float m_fadePause = 0.5f;

    public float FadeDuration => m_fadeDuration;
    
    [Button]
    public async void FadeIn(Action callback, float delay = 0f, Task fadeOutTaskToWaitFor = null)
    {
        await Task.Delay((int)(delay * 1000));

        m_canvasGroup.blocksRaycasts = true;
        m_canvasGroup.DOFade(1, m_fadeDuration).onComplete += async () =>
        {
            callback?.Invoke();
            await Task.Delay((int)(m_fadePause * 1000));
            if (fadeOutTaskToWaitFor != null)
                await fadeOutTaskToWaitFor;
            FadeOut();
        };
    }   
    
    private void FadeOut()
    {
        m_canvasGroup.DOFade(0, m_fadeDuration).onComplete += () =>
        {
            m_canvasGroup.blocksRaycasts = false;
        };
    }
}
