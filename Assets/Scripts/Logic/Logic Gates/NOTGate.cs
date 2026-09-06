using UnityEngine;
using UnityEngine.Events;

public class NotGate : MonoBehaviour, ILogicInteract
{
    [Header("Gate Visuals")]
    public Sprite Zero;
    public Sprite One;

    [Header("Output Indicator")]
    public LogicOutputDisplay outputDisplay;

    private SpriteRenderer spriteRenderer;

    private bool input;
    private bool inputSet; // 👈 NEW: track if input is connected

    public bool Output { get; private set; }

    public UnityEvent<bool> onValueChange = new UnityEvent<bool>();

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Optional: start OFF or keep last state
        SetOutput(false);
    }

    public void SetInput(bool value, int inputIndex = 0)
    {
        input = value;
        inputSet = true; // 👈 mark as connected

        Evaluate();
    }

    void Evaluate()
    {
        // 👇 Prevent evaluation if no valid input (same idea as XNOR)
        if (!inputSet)
            return;

        bool result = !input;
        SetOutput(result);
    }

    void SetOutput(bool value)
    {
        if (Output == value) return;

        Output = value;

        Debug.Log($"[NOT] Input={(input ? 1 : 0)} → Output={(Output ? 1 : 0)}");

        if (outputDisplay != null)
            outputDisplay.SetValue(Output);

        onValueChange.Invoke(Output);

        UpdateVisual(Output);
    }

    void UpdateVisual(bool on)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = on ? One : Zero;
    }
}