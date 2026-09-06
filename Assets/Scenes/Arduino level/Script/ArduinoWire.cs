using UnityEngine;

public class ArduinoWire : MonoBehaviour
{
    public RectTransform rectTransform;

    public void Setup(Vector3 start, Vector3 end)
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (rt == null) return;

        Vector3 middle = (start + end) / 2f;
        transform.position = middle;

        Vector3 dir = end - start;
        float length = dir.magnitude;

        rt.sizeDelta = new Vector2(length, 6f);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);
    }
}