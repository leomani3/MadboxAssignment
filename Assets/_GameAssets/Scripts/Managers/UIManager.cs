using MyBox;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Canvas m_gameCanvas;
    
    public Canvas GameCanvas => m_gameCanvas;
}