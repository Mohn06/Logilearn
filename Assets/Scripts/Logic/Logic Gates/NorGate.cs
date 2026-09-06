using UnityEngine;
using UnityEngine.Events;

public class NorGate : MonoBehaviour, ILogicInteract
{
    [Header("Gate Visuals")]
    public Sprite Zero;
    public Sprite One;

    [Header("Output Indicator")]
    public LogicOutputDisplay outputDisplay;

    public UnityEvent<bool> onValueChange = new UnityEvent<bool>();

    private SpriteRenderer spriteRenderer;

    private bool[] inputs = new bool[2];
    private bool[] inputSet = new bool[2];

    public bool Output { get; private set; }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetOutput(false, false); // 👈 no signal on start
    }

    public void SetInput(bool value, int inputIndex)
    {
        if (inputIndex < 0 || inputIndex >= inputs.Length)
        {
            Debug.LogWarning("[NOR] Invalid input index");
            return;
        }

        inputs[inputIndex] = value;
        inputSet[inputIndex] = true;

        Evaluate();
    }

    void Evaluate()
    {
        // ❗ If any input is missing → NO SIGNAL
        if (!inputSet[0] || !inputSet[1])
        {
            SetOutput(false, false); // 👈 update visual only, no signal
            return;
        }

        bool result = !(inputs[0] || inputs[1]);
        SetOutput(result, true); // 👈 valid signal
    }

    void SetOutput(bool value, bool sendSignal)
    {
        if (Output == value && sendSignal) return;

        Output = value;

        UpdateVisual(Output);

        // ❗ Only send signal if allowed
        if (sendSignal)
        {
            if (outputDisplay != null)
                outputDisplay.SetValue(Output);

            onValueChange.Invoke(Output);

            Debug.Log($"[NOR] A={(inputs[0] ? 1 : 0)} B={(inputs[1] ? 1 : 0)} → Output={(Output ? 1 : 0)}");
        }
        else
        {
            Debug.Log("[NOR] No signal (disconnected input)");
        }
    }

    void UpdateVisual(bool on)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = on ? One : Zero;
    }

    // 🔥 Call this when a wire disconnects
    public void ClearInput(int inputIndex)
    {
        if (inputIndex < 0 || inputIndex >= inputSet.Length)
            return;

        inputSet[inputIndex] = false;

        Evaluate(); // 👈 immediately stop signal
    }
}