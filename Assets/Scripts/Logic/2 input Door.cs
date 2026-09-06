using UnityEngine;

public class twoinputDoor : MonoBehaviour, ILogicInteract
{
    [Header("Door Animation")]
    public string openParameter = "IsOpen";
    public string closeTrigger = "Close";
    public bool disableColliderWhenOpen = true;

    [Header("Logic Pattern")]
    public bool[] requiredInputs = new bool[2]; // Pattern needed to open

    private bool[] currentInputs;

    private Animator animator;
    private Collider2D doorCollider;

    public AudioClip DoorOpen;

    public bool Output { get; private set; }

    void Awake()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider2D>();

        currentInputs = new bool[requiredInputs.Length];

        SetDoor(false);
        Debug.Log("[DOOR] Initialized (Closed)");
    }

    public void SetInput(bool value, int inputIndex = 0)
    {
        if (inputIndex < 0 || inputIndex >= currentInputs.Length)
        {
            Debug.LogWarning("[DOOR] Invalid input index: " + inputIndex);
            return;
        }

        currentInputs[inputIndex] = value;

        Debug.Log($"[DOOR] Input {inputIndex}: {(value ? 1 : 0)}");

        CheckDoorState();
    }

    void CheckDoorState()
    {
        bool match = true;

        for (int i = 0; i < requiredInputs.Length; i++)
        {
            if (currentInputs[i] != requiredInputs[i])
            {
                match = false;
                break;
            }
        }
        Debug.Log("Current Inputs: " + currentInputs[0] + ", " + currentInputs[1]);

        SetDoor(match);
    }

    void SetDoor(bool open)
    {
        if (Output == open) return;

        Output = open;

        if (animator != null)
        {
            if (open)
            {
                animator.SetBool(openParameter, true);
                DoorSFX();
            }
            else
            {
                animator.SetBool(openParameter, false);
                animator.SetTrigger(closeTrigger);
            }
        }

        if (doorCollider != null && disableColliderWhenOpen)
            doorCollider.enabled = !open;

        Debug.Log(open ? "[DOOR] Opening" : "[DOOR] Closing");
    }

    public void DoorSFX()
    {
        AudioManager.Instance.PlaySFX(DoorOpen);
    }
}