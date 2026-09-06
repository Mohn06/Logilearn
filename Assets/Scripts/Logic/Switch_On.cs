using UnityEngine;
using UnityEngine.Events;

public class Switch_On : MonoBehaviour, IInteractable, ILogicInteract
{
    public bool Output { get; private set; }
    public UnityEvent<bool> onValueChange;

    [Header("Switch Visuals")]
    public Sprite redSprite;
    public Sprite greenSprite;

    [Header("Output Indicator")]
    public LogicOutputDisplay outputDisplay;

    [Header("Audio")]
    public AudioClip switchSFX; // 👈 added

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        SetState(true);
    }

    public void Interact()
    {
        Toggle();
    }

    public void Toggle()
    {
        SetState(!Output);

        // 👇 added audio (no logic changed)
        if (AudioManager.Instance != null && switchSFX != null)
            AudioManager.Instance.PlaySFX(switchSFX);
    }

    void SetState(bool value)
    {
        if (Output == value) return;

        Output = value;
        onValueChange?.Invoke(Output);

        if (spriteRenderer != null)
            spriteRenderer.sprite = Output ? greenSprite : redSprite;

        if (outputDisplay != null)
            outputDisplay.SetValue(Output);

        Debug.Log($"[SWITCH_ON] Output = {(Output ? 1 : 0)}");
    }

    public void SetInput(bool value, int inputIndex = 0) { }
}