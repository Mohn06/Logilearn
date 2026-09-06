using UnityEngine;
using System.Collections;

public class TruthTableDoor : MonoBehaviour
{
    [Header("Door Animation")]
    public string openParameter = "IsOpen";
    public string closeTrigger = "Close";
    public bool disableColliderWhenOpen = true;

    [Header("Audio")]
    public AudioClip doorOpen;
    public float startupSilentTime = 0.15f;

    private Animator animator;
    private Collider2D doorCollider;
    private bool isOpen = false;
    private bool canPlayOpenSfx = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider2D>();

        SetInitialClosedState();
    }

    void Start()
    {
        StartCoroutine(EnableOpenSfxAfterStartup());
    }

    void SetInitialClosedState()
    {
        isOpen = false;

        if (animator != null)
        {
            animator.SetBool(openParameter, false);
        }

        if (doorCollider != null && disableColliderWhenOpen)
        {
            doorCollider.enabled = true;
        }

        Debug.Log("[TRUTH TABLE DOOR] Initialized closed");
    }

    IEnumerator EnableOpenSfxAfterStartup()
    {
        yield return new WaitForSeconds(startupSilentTime);
        canPlayOpenSfx = true;
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;

        if (animator != null)
        {
            animator.SetBool(openParameter, true);
        }

        if (doorCollider != null && disableColliderWhenOpen)
        {
            doorCollider.enabled = false;
        }

        if (canPlayOpenSfx)
        {
            PlayDoorSFX();
        }

        Debug.Log("[TRUTH TABLE DOOR] Opening");
    }

    public void CloseDoor()
    {
        if (!isOpen) return;

        isOpen = false;

        if (animator != null)
        {
            animator.SetBool(openParameter, false);
            animator.SetTrigger(closeTrigger);
        }

        if (doorCollider != null && disableColliderWhenOpen)
        {
            doorCollider.enabled = true;
        }

        Debug.Log("[TRUTH TABLE DOOR] Closing");
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    void PlayDoorSFX()
    {
        if (AudioManager.Instance != null && doorOpen != null)
        {
            AudioManager.Instance.PlaySFX(doorOpen);
        }
    }
}