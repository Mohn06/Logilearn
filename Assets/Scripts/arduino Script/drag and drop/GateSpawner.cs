using UnityEngine;
using UnityEngine.InputSystem;

public class GateSpawner : MonoBehaviour
{
    public GameObject gatePrefab;
    public float spawnGridSize = 1f;

    [Header("Drag Spawn Settings")]
    public float spawnDragThreshold = 0.18f;

    private Camera mainCam;

    private bool pressStartedOnToolbox = false;
    private bool hasSpawnedThisPress = false;
    private Vector2 pressStartWorldPos;
    private GameObject spawnedGateThisPress;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (mainCam == null) mainCam = Camera.main;

        Vector2 pointerScreenPos;
        Vector2 pointerWorld;
        bool pressedThisFrame;
        bool releasedThisFrame;
        bool isPressed;

        if (!GetPointerState(out pointerScreenPos, out pointerWorld, out pressedThisFrame, out releasedThisFrame, out isPressed))
            return;

        Vector3 pointerWorld3 = new Vector3(pointerWorld.x, pointerWorld.y, 0f);

        if (pressedThisFrame)
        {
            pressStartedOnToolbox = PointerIsOnThisToolbox(pointerWorld);
            hasSpawnedThisPress = false;
            spawnedGateThisPress = null;
            pressStartWorldPos = pointerWorld;
        }

        // Spawn only when dragging enough from toolbox
        if (pressStartedOnToolbox && isPressed && !hasSpawnedThisPress)
        {
            float moved = Vector2.Distance(pointerWorld, pressStartWorldPos);

            if (moved >= spawnDragThreshold)
            {
                SpawnAndStartDragging(pointerWorld3);
                hasSpawnedThisPress = true;
            }
        }

        if (releasedThisFrame)
        {
            pressStartedOnToolbox = false;
            hasSpawnedThisPress = false;
            spawnedGateThisPress = null;
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

    bool PointerIsOnThisToolbox(Vector2 worldPos)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null) continue;

            if (hits[i].transform == transform || hits[i].transform.IsChildOf(transform))
            {
                return true;
            }
        }

        return false;
    }

    void SpawnAndStartDragging(Vector3 pointerWorld)
    {
        if (gatePrefab == null)
        {
            Debug.LogError("Walang naka-assign na gatePrefab!");
            return;
        }

        Vector3 spawnPos = GetSnappedPosition(pointerWorld);

        GameObject newGate = Instantiate(gatePrefab, spawnPos, Quaternion.identity);
        spawnedGateThisPress = newGate;

        Collider2D newCol = newGate.GetComponent<Collider2D>();
        if (BoardArea.Instance != null && newCol != null)
        {
            Vector3 clamped = BoardArea.Instance.ClampToBoard(newGate.transform.position, newCol.bounds);
            clamped = GetSnappedPosition(clamped);
            newGate.transform.position = clamped;
        }

        DraggableObject drag = newGate.GetComponent<DraggableObject>();
        if (drag != null)
        {
            drag.StartDragging();
        }
    }

    Vector3 GetSnappedPosition(Vector3 pos)
    {
        float snappedX = Mathf.Round(pos.x / spawnGridSize) * spawnGridSize;
        float snappedY = Mathf.Round(pos.y / spawnGridSize) * spawnGridSize;
        return new Vector3(snappedX, snappedY, 0f);
    }
}