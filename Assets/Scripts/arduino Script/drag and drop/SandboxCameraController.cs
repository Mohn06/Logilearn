using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class SandboxCameraController : MonoBehaviour
{
    [Header("Zoom")]
    public float mouseZoomSpeed = 0.02f;
    public float pinchZoomSpeed = 0.01f;
    public float minZoom = 3f;
    public float maxZoom = 12f;

    [Header("Pan")]
    public float panThreshold = 0.12f;
    public float panSmoothTime = 0.08f;

    [Header("Interaction Blocking")]
    public string toolboxLayerName = "Toolbox";
    public float socketBlockRadius = 0.18f;

    [Header("Camera Bounds")]
    public bool useBoardAreaBounds = true;
    public BoxCollider2D customBounds;

    private Camera cam;

    private bool pressStartedOnEmpty = false;
    private bool isPanning = false;

    private Vector2 panStartPointerWorldPos;
    private Vector3 panStartCameraPos;
    private Vector3 panVelocity;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (cam == null) return;

        HandleMouseZoom();
        HandleTouchPinchZoom();
        HandleCameraPan();

        ClampCameraToBounds();
    }

    void HandleMouseZoom()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.orthographicSize -= scroll * mouseZoomSpeed;
            ClampZoom();
        }
    }

    void HandleTouchPinchZoom()
    {
        if (Touchscreen.current == null) return;
        if (Touchscreen.current.touches.Count < 2) return;

        var t0 = Touchscreen.current.touches[0];
        var t1 = Touchscreen.current.touches[1];

        if (!t0.press.isPressed || !t1.press.isPressed) return;

        Vector2 p0 = t0.position.ReadValue();
        Vector2 p1 = t1.position.ReadValue();

        Vector2 p0Prev = p0 - t0.delta.ReadValue();
        Vector2 p1Prev = p1 - t1.delta.ReadValue();

        float prevDist = Vector2.Distance(p0Prev, p1Prev);
        float currDist = Vector2.Distance(p0, p1);

        float diff = currDist - prevDist;

        cam.orthographicSize -= diff * pinchZoomSpeed;
        ClampZoom();

        pressStartedOnEmpty = false;
        isPanning = false;
        panVelocity = Vector3.zero;
    }

    void HandleCameraPan()
    {
        if (PointerIsOverUI())
            return;

        Vector2 pointerScreenPos;
        Vector2 pointerWorldPos;
        bool pressedThisFrame;
        bool releasedThisFrame;
        bool isPressed;

        if (!GetPointerState(out pointerScreenPos, out pointerWorldPos, out pressedThisFrame, out releasedThisFrame, out isPressed))
            return;

        if (pressedThisFrame)
        {
            pressStartedOnEmpty = IsEmptySpace(pointerWorldPos);
            isPanning = false;
            panVelocity = Vector3.zero;

            if (pressStartedOnEmpty)
            {
                panStartPointerWorldPos = pointerWorldPos;
                panStartCameraPos = transform.position;
            }
        }

        if (pressStartedOnEmpty && isPressed)
        {
            float moved = Vector2.Distance(pointerWorldPos, panStartPointerWorldPos);

            if (!isPanning && moved >= panThreshold)
            {
                isPanning = true;
            }

            if (isPanning)
            {
                Vector2 pointerOffset = pointerWorldPos - panStartPointerWorldPos;
                Vector3 targetPos = panStartCameraPos - new Vector3(pointerOffset.x, pointerOffset.y, 0f);

                targetPos = GetClampedCameraPosition(targetPos);

                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    targetPos,
                    ref panVelocity,
                    panSmoothTime
                );
            }
        }

        if (releasedThisFrame)
        {
            pressStartedOnEmpty = false;
            isPanning = false;
            panVelocity = Vector3.zero;
        }
    }

    bool PointerIsOverUI()
    {
        if (EventSystem.current == null) return false;

        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.isPressed || touch.press.wasPressedThisFrame || touch.press.wasReleasedThisFrame)
            {
                int touchId = touch.touchId.ReadValue();
                return EventSystem.current.IsPointerOverGameObject(touchId);
            }
        }

        if (Mouse.current != null)
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        return false;
    }

    bool IsEmptySpace(Vector2 worldPos)
    {
        int toolboxLayer = LayerMask.NameToLayer(toolboxLayerName);
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null) continue;

            if (toolboxLayer >= 0 && hits[i].gameObject.layer == toolboxLayer)
                return false;

            if (hits[i].GetComponent<DraggableObject>() != null)
                return false;

            if (hits[i].GetComponent<GateSocket>() != null)
                return false;
        }

        GateSocket[] sockets = FindObjectsByType<GateSocket>(FindObjectsSortMode.None);
        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i] == null) continue;

            float dist = Vector2.Distance(worldPos, sockets[i].transform.position);
            if (dist <= socketBlockRadius)
                return false;
        }

        return true;
    }

    bool GetPointerState(
        out Vector2 pointerScreenPos,
        out Vector2 pointerWorldPos,
        out bool pressedThisFrame,
        out bool releasedThisFrame,
        out bool isPressed)
    {
        pointerScreenPos = Vector2.zero;
        pointerWorldPos = Vector2.zero;
        pressedThisFrame = false;
        releasedThisFrame = false;
        isPressed = false;

        if (Touchscreen.current != null)
        {
            var touches = Touchscreen.current.touches;

            int pressedCount = 0;
            int firstPressedIndex = -1;

            for (int i = 0; i < touches.Count; i++)
            {
                if (touches[i].press.isPressed ||
                    touches[i].press.wasPressedThisFrame ||
                    touches[i].press.wasReleasedThisFrame)
                {
                    pressedCount++;
                    if (firstPressedIndex == -1)
                        firstPressedIndex = i;
                }
            }

            if (pressedCount >= 2)
                return false;

            if (firstPressedIndex != -1)
            {
                var touch = touches[firstPressedIndex];

                pointerScreenPos = touch.position.ReadValue();
                pressedThisFrame = touch.press.wasPressedThisFrame;
                releasedThisFrame = touch.press.wasReleasedThisFrame;
                isPressed = touch.press.isPressed;

                Vector3 world = cam.ScreenToWorldPoint(new Vector3(pointerScreenPos.x, pointerScreenPos.y, 0f));
                pointerWorldPos = new Vector2(world.x, world.y);
                return true;
            }
        }

        if (Mouse.current != null)
        {
            pointerScreenPos = Mouse.current.position.ReadValue();
            pressedThisFrame = Mouse.current.leftButton.wasPressedThisFrame;
            releasedThisFrame = Mouse.current.leftButton.wasReleasedThisFrame;
            isPressed = Mouse.current.leftButton.isPressed;

            Vector3 world = cam.ScreenToWorldPoint(new Vector3(pointerScreenPos.x, pointerScreenPos.y, 0f));
            pointerWorldPos = new Vector2(world.x, world.y);
            return true;
        }

        return false;
    }

    void ClampZoom()
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    void ClampCameraToBounds()
    {
        transform.position = GetClampedCameraPosition(transform.position);
    }

    Vector3 GetClampedCameraPosition(Vector3 targetPos)
    {
        BoxCollider2D boundsCol = GetBoundsCollider();
        if (boundsCol == null) return targetPos;

        Bounds bounds = boundsCol.bounds;

        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

        float minX = bounds.min.x + camHalfWidth;
        float maxX = bounds.max.x - camHalfWidth;
        float minY = bounds.min.y + camHalfHeight;
        float maxY = bounds.max.y - camHalfHeight;

        // kapag mas maliit ang board kaysa camera view, center lang
        if (minX > maxX)
        {
            float centerX = (bounds.min.x + bounds.max.x) * 0.5f;
            targetPos.x = centerX;
        }
        else
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        }

        if (minY > maxY)
        {
            float centerY = (bounds.min.y + bounds.max.y) * 0.5f;
            targetPos.y = centerY;
        }
        else
        {
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
        }

        return new Vector3(targetPos.x, targetPos.y, transform.position.z);
    }

    BoxCollider2D GetBoundsCollider()
    {
        if (!useBoardAreaBounds)
            return customBounds;

        if (BoardArea.Instance != null)
            return BoardArea.Instance.GetComponent<BoxCollider2D>();

        return customBounds;
    }
}