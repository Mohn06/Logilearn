using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class ToolboxUIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject gatePrefab;
    public float spawnGridSize = 1f;
    public float dragSpawnThreshold = 15f; // screen pixels

    [Header("Spawn Limit")]
    public int maxSpawnCount = 0; // 0 = unlimited

    [Header("UI")]
    public TextMeshProUGUI countText; // assign mo yung TMP dito

    private int spawnedCount = 0;

    private bool pointerDownOnButton = false;
    private bool hasSpawned = false;
    private Vector2 startScreenPos;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Start()
    {
        UpdateCountUI();
    }

    void Update()
    {
        if (!pointerDownOnButton || hasSpawned) return;
        if (mainCam == null) mainCam = Camera.main;

        Vector2 currentScreenPos;
        bool isPressed;

        if (!GetPointer(out currentScreenPos, out isPressed)) return;
        if (!isPressed) return;

        float moved = Vector2.Distance(startScreenPos, currentScreenPos);
        if (moved >= dragSpawnThreshold)
        {
            if (CanSpawn())
            {
                SpawnAndStartDragging(currentScreenPos);
                hasSpawned = true;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownOnButton = true;
        hasSpawned = false;
        startScreenPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDownOnButton = false;
        hasSpawned = false;
    }

    bool GetPointer(out Vector2 screenPos, out bool isPressed)
    {
        screenPos = Vector2.zero;
        isPressed = false;

        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            bool active = touch.press.isPressed || touch.press.wasPressedThisFrame || touch.press.wasReleasedThisFrame;
            if (active)
            {
                screenPos = touch.position.ReadValue();
                isPressed = touch.press.isPressed;
                return true;
            }
        }

        if (Mouse.current != null)
        {
            screenPos = Mouse.current.position.ReadValue();
            isPressed = Mouse.current.leftButton.isPressed;
            return true;
        }

        return false;
    }

    bool CanSpawn()
    {
        if (maxSpawnCount == 0) return true; // unlimited
        return spawnedCount < maxSpawnCount;
    }

    void SpawnAndStartDragging(Vector2 screenPos)
    {
        if (gatePrefab == null || mainCam == null) return;
        if (!CanSpawn()) return;

        Vector3 world = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        world.z = 0f;

        Vector3 spawnPos = GetSnappedPosition(world);

        GameObject newGate = Instantiate(gatePrefab, spawnPos, Quaternion.identity);

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

        spawnedCount++;
        UpdateCountUI();
    }

    Vector3 GetSnappedPosition(Vector3 pos)
    {
        float snappedX = Mathf.Round(pos.x / spawnGridSize) * spawnGridSize;
        float snappedY = Mathf.Round(pos.y / spawnGridSize) * spawnGridSize;
        return new Vector3(snappedX, snappedY, 0f);
    }

    void UpdateCountUI()
    {
        if (countText == null) return;

        // unlimited = hide text
        if (maxSpawnCount == 0)
        {
            countText.gameObject.SetActive(false);
            return;
        }

        countText.gameObject.SetActive(true);

        int remaining = maxSpawnCount - spawnedCount;
        if (remaining < 0) remaining = 0;

        countText.text = remaining.ToString();
    }

    public void ResetSpawnCount()
    {
        spawnedCount = 0;
        UpdateCountUI();
    }

    public int GetSpawnedCount()
    {
        return spawnedCount;
    }

    public int GetRemainingCount()
    {
        if (maxSpawnCount == 0) return -1; // unlimited
        return Mathf.Max(0, maxSpawnCount - spawnedCount);
    }
}