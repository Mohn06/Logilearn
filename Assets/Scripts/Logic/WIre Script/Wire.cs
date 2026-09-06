using UnityEngine;
using UnityEngine.Rendering;

public class Wire2D : MonoBehaviour
{
    [Header("Logic Connection")]
    public MonoBehaviour[] sources;
    public MonoBehaviour target;
    public int targetInputIndex;

    [Header("Wire Shape (Editable)")]
    public Transform[] points;

    [Header("Colors")]
    public Color activeColor = Color.cyan;
    [Tooltip("Color to use when the combined source output is false/0")]
    public Color falseColor = Color.red;

    [Header("Bloom / HDR Settings")]
    [Tooltip("HDR emission multiplier (controls how bright the emission is for bloom)")]
    public float bloomIntensity = 2f;

    [Header("Brightness")]
    [Tooltip("Base brightness multiplier applied to visible color even when bloom is not active")]
    [Range(1f, 4f)]
    public float baseBrightness = 1.5f;

    private ILogicInteract[] sourceNodes;
    private ILogicInteract targetNode;
    private LineRenderer line;
    private Material wireMaterial;

    void Awake()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.widthMultiplier = 0.05f;
        line.useWorldSpace = true;

        // Create material that supports emission (HDR) so post-processing bloom can pick it up
        wireMaterial = new Material(Shader.Find("Sprites/Default"));
        wireMaterial.EnableKeyword("_EMISSION");
        wireMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        line.material = wireMaterial;

        // initial display / emission so bloom (if enabled) has something to pick up
        Color initialDisplay = falseColor * baseBrightness;
        Color initialEmission = falseColor * bloomIntensity * baseBrightness * 0.25f;

        if (wireMaterial != null && wireMaterial.HasProperty("_Color"))
            wireMaterial.SetColor("_Color", initialDisplay);
        if (wireMaterial != null && wireMaterial.HasProperty("_EmissionColor"))
            wireMaterial.SetColor("_EmissionColor", initialEmission);

        line.colorGradient = CreateSolidGradient(initialDisplay);

        // deterministic start state
        SetFalseVisual();
    }

    void Start()
    {
        targetNode = target as ILogicInteract;
        if (targetNode == null)
        {
            Debug.LogError("Wire: Target does NOT implement ILogicInteract!");
            return;
        }

        sourceNodes = new ILogicInteract[sources.Length];
        for (int i = 0; i < sources.Length; i++)
        {
            ILogicInteract node = sources[i] as ILogicInteract;
            if (node == null)
            {
                Debug.LogError($"Wire: Source {sources[i].name} does NOT implement ILogicInteract!");
                continue;
            }

            sourceNodes[i] = node;

            if (node is Switch sw)
                sw.onValueChange.AddListener(UpdateWire);
            else if (node is Switch_On swon)
                swon.onValueChange.AddListener(UpdateWire);
            else if (node is AndGate ag)
                ag.onValueChange.AddListener(UpdateWire);
            else if (node is OrGate og)
                og.onValueChange.AddListener(UpdateWire);
            else if (node is BufferGate bg)
                bg.onValueChange.AddListener(UpdateWire);
            else if (node is NotGate ng)
                ng.onValueChange.AddListener(UpdateWire);
            else if (node is XorGate XorG)
                XorG.onValueChange.AddListener(UpdateWire);
            else if (node is XnorGate XnorG)
                XnorG.onValueChange.AddListener(UpdateWire);
            else if (node is NandGate Nang)
                Nang.onValueChange.AddListener(UpdateWire);
            else if (node is NorGate Norg)
                Norg.onValueChange.AddListener(UpdateWire);
            else
                Debug.LogWarning($"Wire: Source {sources[i].name} has no onValueChange event!");
        }

        // Ensure visuals & target input are correct at start based on current source outputs
        UpdateWire(false);
    }

    void Update()
    {
        if (points == null || points.Length < 2)
            return;

        line.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] != null)
                line.SetPosition(i, points[i].position);
        }
    }

    void UpdateWire(bool _)
    {
        bool combined = false;
        foreach (ILogicInteract node in sourceNodes)
        {
            if (node != null)
                combined |= node.Output;
        }

        if (targetNode != null)
            targetNode.SetInput(combined, targetInputIndex);

        if (combined)
            SetActiveVisual();
        else
            SetFalseVisual();
    }

    void SetActiveVisual()
    {
        Color display = activeColor * baseBrightness;
        Color hdr = activeColor * bloomIntensity * baseBrightness;

        // solid active color (progressive visual removed)
        line.colorGradient = CreateSolidGradient(display);
        line.startColor = display;
        line.endColor = display;

        if (wireMaterial != null)
        {
            if (wireMaterial.HasProperty("_Color"))
                wireMaterial.SetColor("_Color", display);
            if (wireMaterial.HasProperty("_EmissionColor"))
                wireMaterial.SetColor("_EmissionColor", hdr);
        }
    }

    void SetFalseVisual()
    {
        // Increased brightness / neon-like false visual.
        // Use a brighter visible color and stronger emission so bloom produces a neon look.
        float falseBrightnessMultiplier = 1.3f; // tuned multiplier to increase visible brightness
        Color display = falseColor * baseBrightness * falseBrightnessMultiplier;
        Color hdr = falseColor * bloomIntensity * baseBrightness * falseBrightnessMultiplier;

        // Keep a solid gradient that matches the displayed color
        line.colorGradient = CreateSolidGradient(display);

        line.startColor = display;
        line.endColor = display;

        if (wireMaterial != null)
        {
            // Make the material slightly darker than the UTF display so emission stands out,
            // but not too dark to avoid washing out the color when bloom is off.
            if (wireMaterial.HasProperty("_Color"))
                wireMaterial.SetColor("_Color", display * 0.65f);

            if (wireMaterial.HasProperty("_EmissionColor"))
            {
                wireMaterial.EnableKeyword("_EMISSION");
                // Stronger emission to emphasize neon/bloom effect
                wireMaterial.SetColor("_EmissionColor", hdr * 0.5f);
            }
        }
    }

    // Helper: create a solid Gradient so the LineRenderer's vertex-colors align with material color.
    Gradient CreateSolidGradient(Color c)
    {
        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(c, 0f), new GradientColorKey(c, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );
        return g;
    }
}
