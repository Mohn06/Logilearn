using UnityEngine;
using UnityEngine.Events;

public class AndGate : MonoBehaviour, ILogicInteract
{
    [Header("Gate Visuals")]
    public Sprite Zero;
    public Sprite One;

    [Header("Output Indicator")]
    public LogicOutputDisplay outputDisplay;

    public UnityEvent<bool> onValueChange = new UnityEvent<bool>();

    private SpriteRenderer spriteRenderer;

    private bool[] inputs = new bool[2];
    private bool[] inputSet = new bool[2]; // 👈 NEW

    public bool Output { get; private set; }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetOutput(false);
    }

    public void SetInput(bool value, int inputIndex = 0)
    {
        if (inputIndex < 0 || inputIndex >= inputs.Length)
        {
            Debug.LogWarning("[AND] Invalid input index");
            return;
        }

        inputs[inputIndex] = value;
        inputSet[inputIndex] = true; // 👈 mark as connected

        Evaluate();
    }

    void Evaluate()
    {
        // 👇 Don't evaluate unless ALL inputs are set
        if (!inputSet[0] || !inputSet[1])
            return;

        bool result = inputs[0] && inputs[1];
        SetOutput(result);
    }

    void SetOutput(bool value)
    {
        if (Output == value) return;

        Output = value;

        UpdateVisual(Output);

        if (outputDisplay != null)
            outputDisplay.SetValue(Output);

        onValueChange.Invoke(Output);

        Debug.Log($"[AND] A={(inputs[0] ? 1 : 0)} B={(inputs[1] ? 1 : 0)} → Output={(Output ? 1 : 0)}");
    }

    void UpdateVisual(bool on)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = on ? One : Zero;
        }
    }

    // 🔥 OPTIONAL: call this when a wire disconnects
    public void ClearInput(int inputIndex)
    {
        if (inputIndex < 0 || inputIndex >= inputSet.Length)
            return;

        inputSet[inputIndex] = false;
    }
}