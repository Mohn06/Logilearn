using UnityEngine;
using UnityEngine.InputSystem;

public class DraggableObject : MonoBehaviour
{
    private static DraggableObject activeDraggedObject = null;

    private Camera mainCam;
    private bool isDragging = false;
    private bool pressStartedOnThis = false;

    private Vector3 offset;
    private Collider2D myCol;
    private Vector3 lastValidPosition;
    private Vector2 pressStartWorldPos;

    public bool IsDragging => isDragging;

    [Header("Grid Snap")]
    public float gridSize = 1f;

    [Header("Overlap Check")]
    public float sameCellTolerance = 0.05f;

    [Header("Nearest Free Cell Search")]
    public int searchRadius = 5;

    [Header("Layer Names")]
    public string toolboxLayerName = "Toolbox";

    [Header("Socket Block Settings")]
    public float socketBlockRadius = 0.16f;

    [Header("Drag Settings")]
    public float dragStartThreshold = 0.18f;

    void Awake()
    {
        mainCam = Camera.main;
        myCol = GetComponent<Collider2D>();
        lastValidPosition = transform.position;
    }

    void OnDisable()
    {
        if (activeDraggedObject == this)
            activeDraggedObject = null;
    }

    void OnDestroy()
    {
        if (activeDraggedObject == this)
            activeDraggedObject = null;
    }

    void Update()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (myCol == null) myCol = GetComponent<Collider2D>();

        Vector2 pointerScreenPos;
        Vector2 pointerWorldPos;
        bool pressedThisFrame;
        bool releasedThisFrame;
        bool isPressed;

        if (!GetPointerState(out pointerScreenPos, out pointerWorldPos, out pressedThisFrame, out releasedThisFrame, out isPressed))
            return;

        Vector3 pointerWorld3 = new Vector3(pointerWorldPos.x, pointerWorldPos.y, 0f);

        if (pressedThisFrame)
        {
            pressStartedOnThis = false;

            // Kung may ibang active drag, ignore
            if (activeDraggedObject != null && activeDraggedObject != this)
                return;

            // Huwag mag-start drag kapag toolbox o socket ang pinindot
            if (PointerIsOnToolbox(pointerWorldPos))
                return;

            if (PointerIsOnThisGateSocketGrabZone(pointerWorldPos))
                return;

            // Isang draggable lang ang primary under pointer
            if (ThisIsPrimaryDraggableUnderPointer(pointerWorldPos))
            {
                pressStartedOnThis = true;
                pressStartWorldPos = pointerWorldPos;
                lastValidPosition = transform.position;
                offset = transform.position - pointerWorld3;
            }
        }

        // Start actual drag only after movement threshold
        if (!isDragging && pressStartedOnThis && isPressed)
        {
            if (activeDraggedObject != null && activeDraggedObject != this)
            {
                pressStartedOnThis = false;
                return;
            }

            float moved = Vector2.Distance(pointerWorldPos, pressStartWorldPos);
            if (moved >= dragStartThreshold)
            {
                isDragging = true;
                activeDraggedObject = this;
            }
        }

        // While dragging
        if (isDragging)
        {
            if (activeDraggedObject != this)
            {
                isDragging = false;
                pressStartedOnThis = false;
                return;
            }

            transform.position = new Vector3(pointerWorld3.x + offset.x, pointerWorld3.y + offset.y, 0f);
        }

        // Release
        if (releasedThisFrame)
        {
            if (isDragging && activeDraggedObject == this)
            {
                isDragging = false;
                activeDraggedObject = null;

                // Delete if released over Trash UI in Canvas
                if (TrashUIArea.Instance != null && TrashUIArea.Instance.IsPointerInside(pointerScreenPos))
                {
                    DestroyAllConnectedWires();
                    Destroy(gameObject);
                    pressStartedOnThis = false;
                    return;
                }

                SnapClampAndFindNearestFreeCell();
            }

            if (pressStartedOnThis && activeDraggedObject == this)
            {
                activeDraggedObject = null;
            }

            pressStartedOnThis = false;
        }
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

        // Touch first for mobile
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            bool touchActive =
                touch.press.isPressed ||
                touch.press.wasPressedThisFrame ||
                touch.press.wasReleasedThisFrame;

            if (touchActive)
            {
                pointerScreenPos = touch.position.ReadValue();
                pressedThisFrame = touch.press.wasPressedThisFrame;
                releasedThisFrame = touch.press.wasReleasedThisFrame;
                isPressed = touch.press.isPressed;

                Vector3 world = mainCam.ScreenToWorldPoint(new Vector3(pointerScreenPos.x, pointerScreenPos.y, 0f));
                pointerWorldPos = new Vector2(world.x, world.y);
                return true;
            }
        }

        // Mouse fallback for editor / PC
        if (Mouse.current != null)
        {
            pointerScreenPos = Mouse.current.position.ReadValue();
            pressedThisFrame = Mouse.current.leftButton.wasPressedThisFrame;
            releasedThisFrame = Mouse.current.leftButton.wasReleasedThisFrame;
            isPressed = Mouse.current.leftButton.isPressed;

            Vector3 world = mainCam.ScreenToWorldPoint(new Vector3(pointerScreenPos.x, pointerScreenPos.y, 0f));
            pointerWorldPos = new Vector2(world.x, world.y);
            return true;
        }

        return false;
    }

    public void StartDragging()
    {
        if (mainCam == null) mainCam = Camera.main;

        Vector2 pointerScreenPos;
        Vector2 pointerWorldPos;
        bool pressedThisFrame;
        bool releasedThisFrame;
        bool isPressed;

        if (!GetPointerState(out pointerScreenPos, out pointerWorldPos, out pressedThisFrame, out releasedThisFrame, out isPressed))
            return;

        if (activeDraggedObject != null && activeDraggedObject != this)
            return;

        lastValidPosition = transform.position;
        offset = transform.position - new Vector3(pointerWorldPos.x, pointerWorldPos.y, 0f);
        isDragging = true;
        pressStartedOnThis = false;
        activeDraggedObject = this;
    }

    bool ThisIsPrimaryDraggableUnderPointer(Vector2 worldPos)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

        if (hits == null || hits.Length == 0)
            return false;

        DraggableObject bestDraggable = null;
        int bestSortingOrder = int.MinValue;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null) continue;

            DraggableObject dragObj = hits[i].GetComponent<DraggableObject>();
            if (dragObj == null) continue;

            Collider2D dragMainCol = dragObj.GetComponent<Collider2D>();
            if (hits[i] != dragMainCol)
                continue;

            SpriteRenderer sr = dragObj.GetComponent<SpriteRenderer>();
            int sortingOrder = sr != null ? sr.sortingOrder : 0;

            if (bestDraggable == null || sortingOrder > bestSortingOrder)
            {
                bestDraggable = dragObj;
                bestSortingOrder = sortingOrder;
            }
        }

        return bestDraggable == this;
    }

    bool PointerIsOnThisGateSocketGrabZone(Vector2 worldPos)
    {
        GateSocket[] sockets = GetComponentsInChildren<GateSocket>();

        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i] == null) continue;

            float dist = Vector2.Distance(worldPos, sockets[i].transform.position);
            if (dist <= socketBlockRadius)
            {
                return true;
            }
        }

        return false;
    }

    bool PointerIsOnToolbox(Vector2 worldPos)
    {
        int toolboxLayer = LayerMask.NameToLayer(toolboxLayerName);
        if (toolboxLayer < 0) return false;

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null) continue;

            if (hits[i].gameObject.layer == toolboxLayer)
            {
                return true;
            }
        }

        return false;
    }

    void DestroyAllConnectedWires()
    {
        GateSocket[] sockets = GetComponentsInChildren<GateSocket>(true);

        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i] == null) continue;

            if (sockets[i].currentWire != null)
            {
                WireConnection wire = sockets[i].currentWire;
                sockets[i].currentWire = null;

                if (wire != null)
                    wire.DisconnectAndDestroy();
            }

            sockets[i].connectedSocket = null;
        }
    }

    void SnapClampAndFindNearestFreeCell()
    {
        Vector3 targetPos = GetSnappedPosition(transform.position);

        if (BoardArea.Instance != null && myCol != null)
        {
            targetPos = BoardArea.Instance.ClampToBoard(targetPos, myCol.bounds);
            targetPos = GetSnappedPosition(targetPos);
        }

        if (!IsCellOccupiedByOther(targetPos))
        {
            transform.position = targetPos;
            lastValidPosition = transform.position;
            return;
        }

        Vector3 nearestFreePos;
        bool foundFree = FindNearestFreeCell(targetPos, out nearestFreePos);

        if (foundFree)
        {
            transform.position = nearestFreePos;
            lastValidPosition = nearestFreePos;
        }
        else
        {
            transform.position = lastValidPosition;
        }
    }

    Vector3 GetSnappedPosition(Vector3 pos)
    {
        float snappedX = Mathf.Round(pos.x / gridSize) * gridSize;
        float snappedY = Mathf.Round(pos.y / gridSize) * gridSize;
        return new Vector3(snappedX, snappedY, 0f);
    }

    bool IsCellOccupiedByOther(Vector3 targetPos)
    {
        GameObject[] allPlaceables = GameObject.FindGameObjectsWithTag("Placeable");

        foreach (GameObject obj in allPlaceables)
        {
            if (obj == gameObject) continue;

            Vector3 otherPos = obj.transform.position;

            if (Mathf.Abs(otherPos.x - targetPos.x) <= sameCellTolerance &&
                Mathf.Abs(otherPos.y - targetPos.y) <= sameCellTolerance)
            {
                return true;
            }
        }

        return false;
    }

    bool FindNearestFreeCell(Vector3 centerPos, out Vector3 freePos)
    {
        freePos = centerPos;

        for (int r = 1; r <= searchRadius; r++)
        {
            for (int x = -r; x <= r; x++)
            {
                for (int y = -r; y <= r; y++)
                {
                    if (Mathf.Abs(x) != r && Mathf.Abs(y) != r)
                        continue;

                    Vector3 candidate = new Vector3(
                        centerPos.x + (x * gridSize),
                        centerPos.y + (y * gridSize),
                        0f
                    );

                    if (BoardArea.Instance != null && myCol != null)
                    {
                        candidate = BoardArea.Instance.ClampToBoard(candidate, myCol.bounds);
                        candidate = GetSnappedPosition(candidate);
                    }

                    if (!IsCellOccupiedByOther(candidate))
                    {
                        freePos = candidate;
                        return true;
                    }
                }
            }
        }

        return false;
    }
}