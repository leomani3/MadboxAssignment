using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XpBar : MonoBehaviour
{
    [SerializeField] private Slider m_slider;
    [SerializeField] private TextMeshProUGUI m_levelText;
    [SerializeField] private float m_tweenDuration = 0.5f;

    private const float MAX_XP = 15f;
    private Coroutine m_tweenCoroutine;

    private void OnEnable()
    {
        GameData.Instance.OnExperienceChanged += OnExperienceChanged;
        m_slider.value = GameData.Instance.currentExperience / MAX_XP;
        UpdateLevelText(GameData.Instance.currentLevel);
    }

    private void OnDisable()
    {
        GameData.Instance.OnExperienceChanged -= OnExperienceChanged;
    }

    private void OnExperienceChanged(float newXp)
    {
        UpdateLevelText(GameData.Instance.currentLevel);

        float targetValue = newXp / MAX_XP;

        if (m_tweenCoroutine != null)
            StopCoroutine(m_tweenCoroutine);

        m_tweenCoroutine = StartCoroutine(TweenSlider(targetValue));
    }

    private void UpdateLevelText(int level)
    {
        if (m_levelText != null)
            m_levelText.text = (level + 1).ToString();
    }

    private IEnumerator TweenSlider(float targetValue)
    {
        float startValue = m_slider.value;
        float elapsed = 0f;

        while (elapsed < m_tweenDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / m_tweenDuration;
            t = 1f - Mathf.Pow(1f - t, 3f);
            m_slider.value = Mathf.Lerp(startValue, targetValue, t);
            yield return null;
        }

        m_slider.value = targetValue;
        m_tweenCoroutine = null;
    }
}