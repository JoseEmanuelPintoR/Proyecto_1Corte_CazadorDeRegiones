using UnityEngine;

public class SafeAreaController : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect ultimaSafeArea;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        AplicarSafeArea();
    }

    void Update()
    {
        if (Screen.safeArea != ultimaSafeArea)
        {
            AplicarSafeArea();
        }
    }

    void AplicarSafeArea()
    {
        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;

        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        ultimaSafeArea = safeArea;
    }
}
