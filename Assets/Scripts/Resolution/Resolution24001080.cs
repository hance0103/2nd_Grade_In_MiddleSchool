using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
    public float gridCellSize = 1.0f;
    public int gridHeight = 10;

    private float targetAspect = 2400f / 1080f; // 고정할 해상도 비율

    void Start()
    {
        AdjustCameraSize();
    }

    void AdjustCameraSize()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera mainCamera = Camera.main;

        if (scaleHeight < 1.0f)
        {
            // 세로가 더 큰 경우: 검은 띠를 좌우에 추가
            Rect rect = mainCamera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            mainCamera.rect = rect;
        }
        else
        {
            // 가로가 더 큰 경우: 검은 띠를 상하에 추가
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = mainCamera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            mainCamera.rect = rect;
        }
    }
}