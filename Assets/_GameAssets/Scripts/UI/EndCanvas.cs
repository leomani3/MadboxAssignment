using Lean.Pool;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndCanvas : CustomCanvas
{
    [SerializeField] private Button m_continueButton;

    private void Awake()
    {
        m_continueButton.onClick.AddListener(() => OnWinButtonClicked());
    }

    private void OnWinButtonClicked()
    {
        FadeManager.Instance.FadeIn(() =>
        {
            LeanPool.DespawnAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }
}