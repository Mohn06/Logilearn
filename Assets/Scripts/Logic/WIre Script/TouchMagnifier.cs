using UnityEngine;
using UnityEngine.UI;

public class TouchMagnifier : MonoBehaviour
{
    [Header("UI")]
    public RectTransform magnifier;

    [Header("Camera")]
    public Camera mainCamera;
    public Camera magnifierCamera;

    [Header("Settings")]
    public float offsetX = 0f;
    public float offsetY = 120f;
    public float worldZ = -10f;

    void Start()
    {
        if (magnifier != null)
            magnifier.gameObject.SetActive(false);
    }

    public void Show(Vector2 screenPos)
    {
        if (magnifier != null)
            magnifier.gameObject.SetActive(true);

        Move(screenPos);
    }

    public void Move(Vector2 screenPos)
    {
        if (magnifier != null)
            magnifier.position = screenPos + new Vector2(offsetX, offsetY);

        if (mainCamera != null && magnifierCamera != null)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, Mathf.Abs(mainCamera.transform.position.z))
            );

            magnifierCamera.transform.position = new Vector3(worldPos.x, worldPos.y, worldZ);
        }
    }

    public void Hide()
    {
        if (magnifier != null)
            magnifier.gameObject.SetActive(false);
    }
}