using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour, IInteractable, ILogicInteract
{

    public bool Output { get; private set; }
    public UnityEvent<bool> onValueChange;

    [Header("Switch Visuals")]
    public Sprite redSprite;  
    public Sprite greenSprite; 

    

    private SpriteRenderer spriteRenderer;
    public AudioClip switchSFX;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        SetState(false); 
    }

    
    public void Interact()
    {
        Toggle();
    }

 
    public void Toggle()
    {
        SetState(!Output);
        ActivateSwitch();

    }

    public void ActivateSwitch()
    {
        // Play sound
        AudioManager.Instance.PlaySFX(switchSFX);

        // Your existing switch logic
        Debug.Log("Switch Activated!");
    }



void SetState(bool value)
    {
        if (Output == value) return;

        Output = value;
        onValueChange?.Invoke(Output);

        if (spriteRenderer != null)
        {
            
            spriteRenderer.sprite = Output ? greenSprite : redSprite;
        }

       

        Debug.Log($"[SWITCH] Output = {(Output ? 1 : 0)}");
    }

 

    public void SetInput(bool value, int inputIndex = 0) { }
}
