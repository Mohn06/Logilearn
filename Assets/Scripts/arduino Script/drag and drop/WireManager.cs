using UnityEngine;
using UnityEngine.InputSystem;

public class WireManager : MonoBehaviour
{
    public static WireManager Instance;

    public GameObject wirePrefab;

    [Header("Magnifier")]
    public TouchMagnifier magnifier;

    [Header("Mobile-Friendly Socket Grab Settings")]
    public float outputGrabRadius = 0.35f;
    public float inputReleaseRadius = 0.45f;

    private Camera mainCam;
    private GateSocket heldOutputSocket;
    private WireConnection previewWire;
    private bool isHoldingWire = false;

    void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
    }

    void Update()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        Vector2 pointerScreenPos;
        Vector2 pointerWorld;
        bool pressedThisFrame;
        bool releasedThisFrame;
        bool isPressed;

        bool hasPointer = GetPointerState(
            out pointerScreenPos,
            out pointerWorld,
            out pressedThisFrame,
            out releasedThisFrame,
            out isPressed
        );

        if (!hasPointer)
        {
            if (isHoldingWire)
                CancelPreview();
            return;
        }

        Vector3 pointerWorld3 = new Vector3(pointerWorld.x, pointerWorld.y, 0f);

        if (isHoldingWire)
        {
            if (previewWire != null)
                previewWire.SetPreviewEnd(pointerWorld3);

            if (magnifier != null)
            {
                magnifier.Show(pointerScreenPos);
                magnifier.Move(pointerScreenPos);
            }
        }

        if (pressedThisFrame)
        {
            if (!isHoldingWire)
            {
                GateSocket clickedInput = GetNearestSocket(
                    pointerWorld,
                    GateSocket.SocketType.Input,
                    inputReleaseRadius
                );

                if (clickedInput != null && clickedInput.currentWire != null)
                {
                    StartReconnectFromInput(clickedInput);

                    if (isHoldingWire && magnifier != null)
                    {
                        magnifier.Show(pointerScreenPos);
                        magnifier.Move(pointerScreenPos);
                    }

                    return;
                }
            }

            GateSocket clickedOutput = GetNearestSocket(
                pointerWorld,
                GateSocket.SocketType.Output,
                outputGrabRadius
            );

            if (clickedOutput != null)
            {
                StartWireHold(clickedOutput);

                if (isHoldingWire && magnifier != null)
                {
                    magnifier.Show(pointerScreenPos);
                    magnifier.Move(pointerScreenPos);
                }

                return;
            }
        }

        if (releasedThisFrame)
        {
            if (!isHoldingWire)
            {
                if (magnifier != null)
                    magnifier.Hide();
                return;
            }

            GateSocket releasedInput = GetNearestSocket(
                pointerWorld,
                GateSocket.SocketType.Input,
                inputReleaseRadius
            );

            if (releasedInput != null)
            {
                if (heldOutputSocket == null)
                {
                    CancelPreview();
                    return;
                }

                if (releasedInput.parentGate == heldOutputSocket.parentGate)
                {
                    CancelPreview();
                    return;
                }

                if (releasedInput.currentWire != null)
                    DisconnectWire(releasedInput.currentWire);

                FinalizePreview(releasedInput);
            }
            else
            {
                CancelPreview();
            }

            if (magnifier != null)
                magnifier.Hide();
        }

        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelPreview();

            if (magnifier != null)
                magnifier.Hide();
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

                Vector3 world = mainCam.ScreenToWorldPoint(
                    new Vector3(pointerScreenPos.x, pointerScreenPos.y, Mathf.Abs(mainCam.transform.position.z))
                );

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

            Vector3 world = mainCam.ScreenToWorldPoint(
                new Vector3(pointerScreenPos.x, pointerScreenPos.y, Mathf.Abs(mainCam.transform.position.z))
            );

            pointerWorldPos = new Vector2(world.x, world.y);
            return true;
        }

        return false;
    }

    GateSocket GetNearestSocket(Vector2 worldPos, GateSocket.SocketType wantedType, float maxRadius)
    {
        GateSocket[] allSockets = FindObjectsByType<GateSocket>(FindObjectsSortMode.None);
        GateSocket nearest = null;
        float bestScore = float.MaxValue;

        for (int i = 0; i < allSockets.Length; i++)
        {
            if (allSockets[i] == null) continue;
            if (allSockets[i].socketType != wantedType) continue;

            Collider2D col = allSockets[i].GetComponent<Collider2D>();
            Vector2 socketCenter = allSockets[i].transform.position;

            float score;

            if (col != null)
            {
                Vector2 closest = col.ClosestPoint(worldPos);
                float distToCollider = Vector2.Distance(worldPos, closest);

                if (distToCollider > maxRadius)
                    continue;

                score = distToCollider;
            }
            else
            {
                float distToCenter = Vector2.Distance(worldPos, socketCenter);

                if (distToCenter > maxRadius)
                    continue;

                score = distToCenter;
            }

            if (score < bestScore)
            {
                bestScore = score;
                nearest = allSockets[i];
            }
        }

        return nearest;
    }

    void StartWireHold(GateSocket outputSocket)
    {
        if (wirePrefab == null)
            return;

        CancelPreview();

        heldOutputSocket = outputSocket;
        isHoldingWire = true;

        GameObject wireObj = Instantiate(wirePrefab, Vector3.zero, Quaternion.identity);
        previewWire = wireObj.GetComponent<WireConnection>();

        if (previewWire != null)
        {
            previewWire.SetupPreview(outputSocket);
        }
        else
        {
            CancelPreview();
            return;
        }
    }

    void StartReconnectFromInput(GateSocket inputSocket)
    {
        if (inputSocket == null || inputSocket.currentWire == null)
            return;

        WireConnection oldWire = inputSocket.currentWire;
        GateSocket sourceOutput = oldWire.fromSocket;

        if (sourceOutput == null)
        {
            DisconnectWire(oldWire);
            return;
        }

        DisconnectWire(oldWire);
        StartWireHold(sourceOutput);
    }

    void FinalizePreview(GateSocket inputSocket)
    {
        if (previewWire == null || heldOutputSocket == null || inputSocket == null)
        {
            CancelPreview();
            return;
        }

        previewWire.FinalizeWire(inputSocket);

        heldOutputSocket.connectedSocket = inputSocket;
        inputSocket.connectedSocket = heldOutputSocket;

        heldOutputSocket.currentWire = previewWire;
        inputSocket.currentWire = previewWire;

        previewWire = null;
        heldOutputSocket = null;
        isHoldingWire = false;
    }

    void DisconnectWire(WireConnection wire)
    {
        if (wire == null) return;
        wire.DisconnectAndDestroy();
    }

    void CancelPreview()
    {
        if (previewWire != null)
        {
            Destroy(previewWire.gameObject);
            previewWire = null;
        }

        heldOutputSocket = null;
        isHoldingWire = false;

        if (magnifier != null)
            magnifier.Hide();
    }
}