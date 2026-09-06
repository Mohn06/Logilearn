using UnityEngine;
using UnityEngine.Events;

public class NandGate : MonoBehaviour, ILogicInteract
{
    [Header("Gate Visuals")]
    public Sprite Zero;
    public Sprite One;

    [Header("Output Indicator")]
    public LogicOutputDisplay outputDisplay;

    public UnityEvent<bool> onValueChange = new UnityEvent<bool>();

    private SpriteRenderer spriteRenderer;

    private bool inputA;
    private bool inputB;

    // Initialize so first SetOutput() always updates visuals
    public bool Output { get; private set; } = true;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetOutput(false);
    }

    public void SetInput(bool value, int inputIndex)
    {
        switch (inputIndex)
        {
            case 0:
                inputA = value;
                break;
            case 1:
                inputB = value;
                break;
            default:
                Debug.LogWarning("[NAND] Invalid input index");
                return;
        }

        Evaluate();
    }

    void Evaluate()
    {
        // NAND logic: NOT (A AND B)
        bool result = !(inputA && inputB);
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

        Debug.Log($"[NAND] A={(inputA ? 1 : 0)} B={(inputB ? 1 : 0)} → Output={(Output ? 1 : 0)}");
    }

    void UpdateVisual(bool on)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = on ? One : Zero;
    }
}
