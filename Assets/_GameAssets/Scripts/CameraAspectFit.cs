using Cinemachine;
using UnityEngine;

public class CameraAspectFit : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float targetAspect = 1080f / 2400f;

    void Start()
    {
        float screenAspect = (float)Screen.width / Screen.height;
        float scaleHeight = screenAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            virtualCamera.m_Lens.OrthographicSize /= scaleHeight;
        }
    }
}