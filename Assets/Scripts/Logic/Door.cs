using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, ILogicInteract
{
    [Header("Door Animation")]
    public string openParameter = "IsOpen";
    public string closeTrigger = "Close";
    public bool disableColliderWhenOpen = true;

    [Header("Logic Requirement")]
    public bool requiredInput = true; // true = open on 1, false = open on 0

    [Header("Audio")]
    public AudioClip DoorOpen;
    public float startupSilentTime = 0.15f; // ignore open SFX only during startup

    private Animator animator;
    private Collider2D doorCollider;

    public bool Output { get; private set; }

    private bool canPlayOpenSfx = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider2D>();

        // Start closed silently
        Output = false;

        if (animator != null)
        {
            animator.SetBool(openParameter, false);
        }

        if (doorCollider != null && disableColliderWhenOpen)
        {
            doorCollider.enabled = true;
        }

        Debug.Log("[DOOR] Initialized (Closed)");
    }

    void Start()
    {
        StartCoroutine(EnableOpenSfxAfterStartup());
    }

    IEnumerator EnableOpenSfxAfterStartup()
    {
        yield return new WaitForSeconds(startupSilentTime);
        canPlayOpenSfx = true;
    }

    // Called by Wire2D
    public void SetInput(bool value, int inputIndex = 0)
    {
        Debug.Log("[DOOR] Input received: " + (value ? 1 : 0));

        bool shouldOpen = (value == requiredInput);
        SetDoor(shouldOpen);
    }

    void SetDoor(bool open)
    {
        bool wasOpen = Output;

        if (wasOpen == open)
            return;

        Output = open;

        if (animator != null)
        {
            animator.SetBool(openParameter, open);

            if (!open)
            {
                animator.SetTrigger(closeTrigger);
            }
        }

        if (doorCollider != null && disableColliderWhenOpen)
        {
            doorCollider.enabled = !open;
        }

        // Play SFX only on CLOSED -> OPEN, and only after startup phase
        if (canPlayOpenSfx && !wasOpen && open)
        {
            DoorSFX();
        }

        Debug.Log(open ? "[DOOR] Opening" : "[DOOR] Closing");
    }

    public void DoorSFX()
    {
        if (AudioManager.Instance != null && DoorOpen != null)
        {
            AudioManager.Instance.PlaySFX(DoorOpen);
        }
    }
}