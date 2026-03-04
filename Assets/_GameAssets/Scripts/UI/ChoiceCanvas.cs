using Lean.Pool;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChoiceCanvas : CustomCanvas
{
    public void OnChoiceClicked()
    {
        UIManager.Instance.OpenCanvas(UIManager.Instance.GameCanvas);
    }

    public override void Open(bool _instant = false)
    {
        base.Open(_instant);
        Time.timeScale = 0;
    }

    public override void Close(bool _instant = false)
    {
        base.Close(_instant);
        Time.timeScale = 1;
    }
}