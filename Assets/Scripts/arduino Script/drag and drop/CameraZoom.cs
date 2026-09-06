using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 0.5f;

    public float minZoom = 3f;
    public float maxZoom = 12f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (cam == null) return;

        HandleMouseZoom();
        HandleTouchZoom();
    }

    void HandleMouseZoom()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            cam.orthographicSize -= scroll * zoomSpeed * Time.deltaTime;
            ClampZoom();
        }
    }

    void HandleTouchZoom()
    {
        if (Touchscreen.current == null) return;

        var touches = Touchscreen.current.touches;

        if (touches.Count < 2) return;

        if (!touches[0].press.isPressed || !touches[1].press.isPressed)
            return;

        Vector2 t0 = touches[0].position.ReadValue();
        Vector2 t1 = touches[1].position.ReadValue();

        Vector2 t0Prev = t0 - touches[0].delta.ReadValue();
        Vector2 t1Prev = t1 - touches[1].delta.ReadValue();

        float prevDist = Vector2.Distance(t0Prev, t1Prev);
        float currDist = Vector2.Distance(t0, t1);

        float diff = currDist - prevDist;

        cam.orthographicSize -= diff * 0.01f;

        ClampZoom();
    }

    void ClampZoom()
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }
}