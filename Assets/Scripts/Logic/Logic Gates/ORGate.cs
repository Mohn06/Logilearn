    using UnityEngine;
using UnityEngine.Events;

public class OrGate : MonoBehaviour, ILogicInteract
{
    [Header("Gate Visuals")]
    public Sprite Zero;
    public Sprite One;

    [Header("Output Indicator")]
    public LogicOutputDisplay outputDisplay;

    private SpriteRenderer spriteRenderer;
    private bool[] inputs = new bool[2];

    public bool Output { get; private set; }

   
    public UnityEvent<bool> onValueChange = new UnityEvent<bool>();

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual(false);
    }

    public void SetInput(bool value, int inputIndex = 0)
    {
        if (inputIndex < 0 || inputIndex >= inputs.Length)
            return;

        inputs[inputIndex] = value;
        Evaluate();
    }

    void Evaluate()
    {
        bool result = inputs[0] || inputs[1];
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

        Debug.Log($"[OR] Output = {(Output ? 1 : 0)}");
    }

    void UpdateVisual(bool on)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = on ? One : Zero;
    }
}
