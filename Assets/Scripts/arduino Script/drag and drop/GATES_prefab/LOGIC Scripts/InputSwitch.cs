using UnityEngine;
using UnityEngine.InputSystem;

public class InputSwitch : MonoBehaviour, ILogicValue
{
    public bool currentValue = false;

    public SpriteRenderer sr;

    [Header("Sprites (NEW)")]
    public Sprite offSprite;
    public Sprite onSprite;

    [Header("Optional Colors")]
    public Color offColor = Color.red;
    public Color onColor = Color.green;

    [Header("Tap Settings")]
    public float tapMaxMoveDistance = 0.2f;

    private Camera mainCam;
    private Collider2D myCol;
    private DraggableObject dragObj;

    private bool pressStartedOnThis = false;
    private Vector2 pressStartWorldPos;

    void Awake()
    {
        mainCam = Camera.main;
        myCol = GetComponent<Collider2D>();
        dragObj = GetComponent<DraggableObject>();
    }

    void Start()
    {
        UpdateVisual();
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

        if (pressedThisFrame)
        {
            pressStartedOnThis = PointerHitsThisBody(pointerWorldPos);
            pressStartWorldPos = pointerWorldPos;
        }

        if (releasedThisFrame)
        {
            if (!pressStartedOnThis)
                return;

            pressStartedOnThis = false;

            float movedDistance = Vector2.Distance(pressStartWorldPos, pointerWorldPos);
            bool dragged = dragObj != null && dragObj.IsDragging;
            bool releasedOnThis = PointerHitsThisBody(pointerWorldPos);

            if (!dragged && releasedOnThis && movedDistance <= tapMaxMoveDistance)
            {
                currentValue = !currentValue;

                UpdateVisual();

                Debug.Log("[SWITCH] " + gameObject.name + " = " + currentValue);
            }
        }
    }

    bool PointerHitsThisBody(Vector2 worldPos)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == myCol)
                return true;
        }

        return false;
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

    void UpdateVisual()
    {
        if (sr == null) return;

        // 🔥 Sprite priority (like your XOR gate)
        if (currentValue)
        {
            if (onSprite != null)
                sr.sprite = onSprite;

            sr.color = onColor;
        }
        else
        {
            if (offSprite != null)
                sr.sprite = offSprite;

            sr.color = offColor;
        }
    }

    public bool GetValue()
    {
        return currentValue;
    }
}