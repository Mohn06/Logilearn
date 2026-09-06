using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class GateTapHandler : MonoBehaviour
{
    [Header("Tap Settings")]
    [SerializeField] private LayerMask logicGateLayer; // 👈 ADD THIS
    [SerializeField] private Camera mainCamera;        // 👈 RECOMMENDED

    void Awake()
    {
        // Safety fallback
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        // Block taps while popup is open
        if (LogicGatePagedPopup.Instance != null &&
            LogicGatePagedPopup.Instance.panel.activeSelf)
            return;

        Vector2 screenPos = Vector2.zero;
        bool pressed = false;

        // 🖱 PC Mouse
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("🖱 Mouse click detected");

            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            screenPos = Mouse.current.position.ReadValue();
            pressed = true;
        }
        // 📱 Mobile Touch (New Input System)
        else if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                Debug.Log("📱 Touch detected (New Input System)");

                int touchId = touch.touchId.ReadValue();

                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject(touchId))
                    return;

                screenPos = touch.position.ReadValue();
                pressed = true;
            }
        }

        if (!pressed) return;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        // 🎯 FIX: Only hit logic gates, ignore camera confiner
        Collider2D hit = Physics2D.OverlapPoint(worldPos, logicGateLayer);

        if (hit != null)
        {
            Debug.Log("🎯 Collider hit: " + hit.name);
        }

        if (hit != null &&
            hit.TryGetComponent(out LogicGateClickable gate))
        {
            Debug.Log("✅ Logic gate clicked: " + gate.gameObject.name);
            gate.OnClicked();
        }
    }
}
