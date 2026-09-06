using UnityEngine;
using UnityEngine.InputSystem;
using InputTouchPhase = UnityEngine.InputSystem.TouchPhase;

public class WirePath : MonoBehaviour
{
    [Header("Logic Connection")]
    [SerializeField] private MonoBehaviour[] sources;
    [SerializeField] private MonoBehaviour target;
    public int targetInputIndex;

    [Header("Wire Path (Editable)")]
    public Transform[] points;

    [Header("Path Interaction")]
    public float touchRadius = 0.25f;

    [Header("Colors")]
    public Color inactiveColor = new Color(0.3f, 0.3f, 0.3f);
    public Color activeColor = Color.cyan;
    [Tooltip("Color to use when the combined source output is false/0")]
    public Color falseColor = Color.red;

    [Header("Bloom Settings")]
    [Tooltip("HDR intensity multiplier for bloom glow")]
    public float bloomIntensity = 2f;

    [Header("Brightness")]
    [Tooltip("Base brightness multiplier applied to visible color even when bloom is not active")]
    [Range(1f, 4f)]
    public float baseBrightness = 1.5f;

    [Header("Visual")]
    [Range(0f, 1f)]
    public float visualSpread = 0.2f;

    [Header("Continuous Drag")]
    [Range(1f, 10f)]
    public float driftToleranceMultiplier = 3f;

    [Header("Progressive Visual")]
    [Range(0f, 1f)]
    public float currentProgress = 0f;

    [Header("Magnifier")]
    public TouchMagnifier magnifier;

    private ILogicInteract[] sourceNodes;
    private ILogicInteract targetNode;

    private LineRenderer line;
    private Camera cam;
    private Material wireMaterial;

    private bool connected = false;
    private bool isTracing = false;

    // ---------------- UNITY ----------------

    void Awake()
    {
        cam = Camera.main;

        line = gameObject.AddComponent<LineRenderer>();
        line.widthMultiplier = 0.05f;
        line.useWorldSpace = true;

        // BLOOM-READY MATERIAL (HDR)
        wireMaterial = new Material(Shader.Find("Sprites/Default"));
        wireMaterial.EnableKeyword("_EMISSION");
        line.material = wireMaterial;

        if (wireMaterial != null && wireMaterial.HasProperty("_Color"))
            wireMaterial.SetColor("_Color", inactiveColor * baseBrightness);

        if (wireMaterial != null && wireMaterial.HasProperty("_EmissionColor"))
            wireMaterial.SetColor("_EmissionColor", inactiveColor * bloomIntensity * baseBrightness * 0.25f);
    }

    void Start()
    {
        targetNode = target as ILogicInteract;
        if (targetNode == null)
        {
            Debug.LogError("WirePath: Target does NOT implement ILogicInteract!");
            return;
        }

        sourceNodes = new ILogicInteract[sources.Length];
        for (int i = 0; i < sources.Length; i++)
        {
            sourceNodes[i] = sources[i] as ILogicInteract;
            if (sourceNodes[i] == null) continue;

            if (sources[i] is Switch sw) sw.onValueChange.AddListener(UpdateWire);
            else if (sources[i] is Switch_On so) so.onValueChange.AddListener(UpdateWire);
            else if (sources[i] is AndGate ag) ag.onValueChange.AddListener(UpdateWire);
            else if (sources[i] is OrGate og) og.onValueChange.AddListener(UpdateWire);
            else if (sources[i] is BufferGate bg) bg.onValueChange.AddListener(UpdateWire);
            else if (sources[i] is NotGate ng) ng.onValueChange.AddListener(UpdateWire);
            else if (sources[i] is XorGate xg) xg.onValueChange.AddListener(UpdateWire);
            else if (sources[i] is XnorGate xng) xng.onValueChange.AddListener(UpdateWire);
            else if (sources[i] is NandGate nang) nang.onValueChange.AddListener(UpdateWire);
            else if (sources[i] is NorGate norg) norg.onValueChange.AddListener(UpdateWire);
        }

        UpdateLinePositions();
        SetInactiveVisual();

        if (magnifier != null)
            magnifier.Hide();
    }

    void Update()
    {
        UpdateLinePositions();

        if (!connected)
        {
            HandleInput();
            HandleTracing();
        }
        else
        {
            UpdateWire(false);
        }
    }

    // ---------------- INPUT ----------------

    void HandleInput()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Mouse mouse = Mouse.current;
        Touchscreen touchscreen = Touchscreen.current;

        // TOUCH
        if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
        {
            Vector2 screenPos = touchscreen.primaryTouch.position.ReadValue();
            Vector3 world = cam.ScreenToWorldPoint(screenPos);
            world.z = 0f;

            if (!isTracing)
            {
                if (Vector2.Distance(world, points[0].position) <= touchRadius)
                {
                    isTracing = true;

                    if (magnifier != null)
                    {
                        magnifier.Show(screenPos);
                        magnifier.Move(screenPos);
                    }
                }
            }
            else
            {
                if (magnifier != null)
                    magnifier.Move(screenPos);
            }

            return;
        }
        else if (touchscreen != null && !touchscreen.primaryTouch.press.isPressed)
        {
            ResetTracing();
            return;
        }

        // MOUSE
        if (mouse == null) return;

        if (mouse.leftButton.isPressed)
        {
            Vector2 screenPos = mouse.position.ReadValue();
            Vector3 world = cam.ScreenToWorldPoint(screenPos);
            world.z = 0f;

            if (!isTracing)
            {
                if (Vector2.Distance(world, points[0].position) <= touchRadius)
                {
                    isTracing = true;

                    if (magnifier != null)
                    {
                        magnifier.Show(screenPos);
                        magnifier.Move(screenPos);
                    }
                }
            }
            else
            {
                if (magnifier != null)
                    magnifier.Move(screenPos);
            }
        }
        else
        {
            ResetTracing();
        }
    }

    // ---------------- TRACING ----------------

    void HandleTracing()
    {
        if (!isTracing) return;

        Vector3 world;
        Vector2 screenPos;

        Touchscreen touchscreen = Touchscreen.current;
        Mouse mouse = Mouse.current;

        if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
        {
            screenPos = touchscreen.primaryTouch.position.ReadValue();
            world = cam.ScreenToWorldPoint(screenPos);

            if (magnifier != null)
            {
                magnifier.Move(screenPos);
                magnifier.Show(screenPos);
            }
        }
        else if (mouse != null && mouse.leftButton.isPressed)
        {
            screenPos = mouse.position.ReadValue();
            world = cam.ScreenToWorldPoint(screenPos);

            if (magnifier != null)
            {
                magnifier.Move(screenPos);
                magnifier.Show(screenPos);
            }
        }
        else
        {
            ResetTracing();
            return;
        }

        world.z = 0f;

        float minDist = float.MaxValue;
        for (int i = 0; i < points.Length - 1; i++)
        {
            float d = DistancePointToSegment(world, points[i].position, points[i + 1].position);
            minDist = Mathf.Min(minDist, d);
        }

        if (minDist > touchRadius * driftToleranceMultiplier)
        {
            ResetTracing();
            return;
        }

        float newProgress = GetProgressAlongPath(world);
        if (newProgress >= currentProgress)
        {
            currentProgress = newProgress;
            UpdateVisualProgressive();

            if (currentProgress >= 0.995f)
                CompleteConnection();
        }
    }

    // ---------------- CONNECTION ----------------

    void CompleteConnection()
    {
        connected = true;
        isTracing = false;

        if (magnifier != null)
            magnifier.Hide();

        UpdateWire(false);
        Debug.Log("WirePath CONNECTED");
    }

    void UpdateWire(bool _)
    {
        if (!connected) return;

        bool combined = false;
        foreach (ILogicInteract node in sourceNodes)
            if (node != null) combined |= node.Output;

        targetNode.SetInput(combined, targetInputIndex);

        if (combined)
            SetActiveVisual();
        else
            SetFalseVisual();
    }

    // ---------------- VISUAL ----------------

    void UpdateVisualProgressive()
    {
        float end = Mathf.Clamp01(currentProgress + visualSpread);

        Color bright = activeColor * baseBrightness;
        Color brightHdr = activeColor * bloomIntensity * baseBrightness;

        Gradient g = new Gradient();
        g.SetKeys(
            new[]
            {
                new GradientColorKey(brightHdr, 0f),
                new GradientColorKey(brightHdr, currentProgress),
                new GradientColorKey(inactiveColor * baseBrightness, end),
                new GradientColorKey(inactiveColor * baseBrightness, 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );

        line.colorGradient = g;

        if (wireMaterial != null)
        {
            if (wireMaterial.HasProperty("_Color"))
                wireMaterial.SetColor("_Color", inactiveColor * baseBrightness);

            if (wireMaterial.HasProperty("_EmissionColor"))
                wireMaterial.SetColor("_EmissionColor", brightHdr * 0.5f);
        }
    }

    void SetInactiveVisual()
    {
        Color display = inactiveColor * baseBrightness;

        if (wireMaterial != null && wireMaterial.HasProperty("_Color"))
            wireMaterial.SetColor("_Color", display);

        if (wireMaterial != null && wireMaterial.HasProperty("_EmissionColor"))
            wireMaterial.SetColor("_EmissionColor", inactiveColor * bloomIntensity * baseBrightness * 0.25f);

        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(display, 0f), new GradientColorKey(display, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );
        line.colorGradient = g;

        line.startColor = display;
        line.endColor = display;
    }

    void SetActiveVisual()
    {
        Color display = activeColor * baseBrightness;
        Color hdr = activeColor * bloomIntensity * baseBrightness;

        if (wireMaterial != null && wireMaterial.HasProperty("_Color"))
            wireMaterial.SetColor("_Color", display);

        if (wireMaterial != null && wireMaterial.HasProperty("_EmissionColor"))
            wireMaterial.SetColor("_EmissionColor", hdr);

        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(display, 0f), new GradientColorKey(display, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );
        line.colorGradient = g;

        line.startColor = display;
        line.endColor = display;
    }

    void SetFalseVisual()
    {
        Color display = falseColor * baseBrightness;
        Color hdr = falseColor * bloomIntensity * baseBrightness;

        if (wireMaterial != null && wireMaterial.HasProperty("_Color"))
            wireMaterial.SetColor("_Color", display);

        if (wireMaterial != null && wireMaterial.HasProperty("_EmissionColor"))
            wireMaterial.SetColor("_EmissionColor", hdr * 0.5f);

        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(display, 0f), new GradientColorKey(display, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );
        line.colorGradient = g;

        line.startColor = display;
        line.endColor = display;
    }

    void UpdateLinePositions()
    {
        if (points == null || points.Length < 2) return;

        line.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
            line.SetPosition(i, points[i].position);
    }

    void ResetTracing()
    {
        if (connected) return;

        isTracing = false;
        currentProgress = 0f;
        SetInactiveVisual();

        if (magnifier != null)
            magnifier.Hide();
    }

    // ---------------- HELPERS ----------------

    float GetProgressAlongPath(Vector2 worldPos)
    {
        float totalLength = 0f;
        float closestDist = float.MaxValue;
        float lengthAtClosest = 0f;

        for (int i = 0; i < points.Length - 1; i++)
            totalLength += Vector2.Distance(points[i].position, points[i + 1].position);

        float accumulated = 0f;

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 a = points[i].position;
            Vector2 b = points[i + 1].position;
            Vector2 ab = b - a;

            float t = Mathf.Clamp01(Vector2.Dot(worldPos - a, ab) / ab.sqrMagnitude);
            Vector2 proj = a + ab * t;

            float d = Vector2.Distance(worldPos, proj);
            if (d < closestDist)
            {
                closestDist = d;
                lengthAtClosest = accumulated + Vector2.Distance(a, proj);
            }

            accumulated += Vector2.Distance(a, b);
        }

        return Mathf.Clamp01(lengthAtClosest / totalLength);
    }

    static float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float t = Vector2.Dot(p - a, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        return Vector2.Distance(p, a + ab * t);
    }
}