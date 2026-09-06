using UnityEngine;
using UnityEngine.UI;

public class TrashUIArea : MonoBehaviour
{
    public static TrashUIArea Instance;

    private RectTransform rect;

    void Awake()
    {
        Instance = this;
        rect = GetComponent<RectTransform>();
    }

    public bool IsPointerInside(Vector2 screenPos)
    {
        if (rect == null) return false;

        return RectTransformUtility.RectangleContainsScreenPoint(
            rect,
            screenPos,
            null
        );
    }
}