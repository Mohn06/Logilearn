using UnityEngine;
using UnityEngine.Events;

public class BufferGate : MonoBehaviour, ILogicInteract
{
    [Header("Gate Visuals")]
    public Sprite Zero;   
    public Sprite One;   

    [Header("Output Indicator")]
    public LogicOutputDisplay outputDisplay;

    public UnityEvent<bool> onValueChange = new UnityEvent<bool>();

    private SpriteRenderer spriteRenderer;
    public bool Output { get; private set; }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetOutput(false); 
    }

    public void SetInput(bool value, int inputIndex = 0)
    {
        
        SetOutput(value);
    }

    void SetOutput(bool value)
    {
        if (Output == value) return;

        Output = value;

        
        UpdateVisual(Output);

        
        if (outputDisplay != null)
            outputDisplay.SetValue(Output);

        
        onValueChange.Invoke(Output);

        Debug.Log($"[BUFFER] Output = {(Output ? 1 : 0)}");
    }

    void UpdateVisual(bool on)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = on ? One : Zero;
    }
}
