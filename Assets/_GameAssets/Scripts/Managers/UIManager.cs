using System;
using MyBox;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private CustomCanvas m_gameCanvas;
    [SerializeField] private CustomCanvas m_winCanvas;
    [SerializeField] private CustomCanvas m_loseCanvas;
    
    [SerializeField] private CustomCanvas m_defaultCanvas;
    
    public CustomCanvas GameCanvas => m_gameCanvas;
    public CustomCanvas WinCanvas => m_winCanvas;
    public CustomCanvas LoseCanvas => m_loseCanvas;
    
    private CustomCanvas m_currentCanvas;

    private void Awake()
    {
        m_gameCanvas.Close(true);
        m_winCanvas.Close(true);
        m_loseCanvas.Close(true);
        
        OpenCanvas(m_defaultCanvas, true);
    }

    public void OpenCanvas(CustomCanvas newCanvas, bool instant = false)
    {
        if (m_currentCanvas != null)
        {
            m_currentCanvas.Close(instant);
        }

        m_currentCanvas = newCanvas;
        m_currentCanvas.Open(instant);
    }
}