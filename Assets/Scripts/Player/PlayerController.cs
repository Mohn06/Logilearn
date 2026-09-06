using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    public AudioClip footstep;
    public bool FacingLeft { get { return facingLeft; } set { facingLeft = value; } }

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;

    private bool facingLeft = false;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    // Ensure input action maps are disabled when the component is disabled
    private void OnDisable()
    {
        playerControls?.Disable();
    }

    // Clean up the generated InputAction asset when the object is destroyed
    private void OnDestroy()
    {
        playerControls?.Dispose();
    }

    private void Update()
    {
        
        PlayerInput();
    }

    private void FixedUpdate()
    {

        if (TutorialManager.Instance != null && TutorialManager.Instance.IsJournalOpen)
            return;
        AdjustPlayerFacingDirection();
        Move();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
    }
    public void ForceStop()
    {
        // Clear stored movement input
        movement = Vector2.zero;

        // Stop physics movement (new Unity versions use linearVelocity)
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Stop animations
        if (myAnimator != null)
        {
            myAnimator.SetFloat("moveX", 0f);
            myAnimator.SetFloat("moveY", 0f);
        }
    }
    public void PlayFootstep()
    {
        AudioManager.Instance.PlaySFX(footstep);
    }
    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }
    public void DisableInput()
    {
        playerControls.Movement.Disable();  // disable only movement map
        ForceStop();
    }

    public void EnableInput()
    {
        playerControls.Movement.Enable();
    }
    private void AdjustPlayerFacingDirection()
    {
        if (movement.x > 0) // Moving Right
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            FacingLeft = false;
        }
        else if (movement.x < 0) // Moving Left
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            FacingLeft = true;
        }
    }
}
