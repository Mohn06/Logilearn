using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public float interactRadius = 1.5f;
    public LayerMask interactableLayer;

    public void Interact()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            interactRadius,
            interactableLayer
        );

        if (hit != null)
        {

            // Generic interactable
            if (hit.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
