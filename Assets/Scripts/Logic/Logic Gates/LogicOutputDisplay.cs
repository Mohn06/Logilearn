using UnityEngine;
using UnityEngine.Events;

public class SignalDisplayNode : MonoBehaviour, IInteractable, ILogicInteract
{
    [Header("Popup Script")]
    public TruthTablePopup truthTablePopup;

    [Header("Sprites")]
    public Sprite zeroSprite;
    public Sprite oneSprite;

    [Header("Optional Sound")]
    public AudioClip interactSFX;
    public AudioClip changeSFX;

    public bool Output { get; private set; }
    public UnityEvent<bool> onValueChange;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Output = false;
        UpdateVisual();
    }

    public void Interact()
    {
        OpenConsole();
    }

    public void OpenConsole()
    {
        if (interactSFX != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(interactSFX);

        if (truthTablePopup != null)
            truthTablePopup.OpenPopup();
    }

    public void SetInput(bool value, int inputIndex = 0)
    {
        SetState(value);
    }

    void SetState(bool value)
    {
        if (Output == value) return;

        Output = value;

        UpdateVisual();

        if (changeSFX != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(changeSFX);

        onValueChange?.Invoke(Output);
    }

    void UpdateVisual()
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = Output ? oneSprite : zeroSprite;
    }
}