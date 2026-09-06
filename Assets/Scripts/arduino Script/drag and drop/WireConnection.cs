using UnityEngine;

public class WireConnection : MonoBehaviour
{
    [Header("Sockets")]
    public GateSocket fromSocket;
    public GateSocket toSocket;

    [Header("Line")]
    public LineRenderer line;
    public float lineWidth = 0.08f;

    [Header("Visual")]
    public Color offColor = Color.black;
    public Color onColor = Color.cyan;
    [Range(1f, 5f)]
    public float onBrightness = 2f;

    private bool isPreview = false;
    private Vector3 previewEndPos;
    private bool lastSignalState = false;

    void Awake()
    {
        if (line == null)
            line = GetComponent<LineRenderer>();

        if (line == null)
            line = gameObject.AddComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.positionCount = 2;

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        line.numCapVertices = 6;
        line.numCornerVertices = 6;

        ApplyVisual(false);
    }

    void Update()
    {
        UpdateLinePositions();

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        if (isPreview)
        {
            ApplyVisual(false);
            return;
        }

        bool signal = GetCurrentSignal();
        if (signal != lastSignalState)
            lastSignalState = signal;

        ApplyVisual(signal);
    }

    public void SetupPreview(GateSocket outputSocket)
    {
        fromSocket = outputSocket;
        toSocket = null;
        isPreview = true;
        lastSignalState = false;
        ApplyVisual(false);
    }

    public void SetPreviewEnd(Vector3 endPos)
    {
        previewEndPos = endPos;
    }

    public void FinalizeWire(GateSocket inputSocket)
    {
        toSocket = inputSocket;
        isPreview = false;
        UpdateLinePositions();
        lastSignalState = GetCurrentSignal();
        ApplyVisual(lastSignalState);
    }

    void UpdateLinePositions()
    {
        if (line == null || fromSocket == null)
            return;

        line.positionCount = 2;
        line.SetPosition(0, fromSocket.transform.position);

        if (isPreview || toSocket == null)
            line.SetPosition(1, previewEndPos);
        else
            line.SetPosition(1, toSocket.transform.position);
    }

    bool GetCurrentSignal()
    {
        if (fromSocket == null)
            return false;

        return fromSocket.GetSignalValue();
    }

    void ApplyVisual(bool isOn)
    {
        if (line == null)
            return;

        Color display = isOn ? onColor * onBrightness : offColor;

        line.startColor = display;
        line.endColor = display;
        line.colorGradient = CreateSolidGradient(display);
    }

    Gradient CreateSolidGradient(Color c)
    {
        Gradient g = new Gradient();
        g.SetKeys(
            new[] {
                new GradientColorKey(c, 0f),
                new GradientColorKey(c, 1f)
            },
            new[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );
        return g;
    }

    public void DisconnectAndDestroy()
    {
        if (fromSocket != null)
        {
            fromSocket.connectedSocket = null;
            fromSocket.currentWire = null;
        }

        if (toSocket != null)
        {
            toSocket.connectedSocket = null;
            toSocket.currentWire = null;
        }

        Destroy(gameObject);
    }
}