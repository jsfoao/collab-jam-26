using Input;
using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;

    private Interactable focusedInteractable;
    public Interactable FocusedInteractable => focusedInteractable;

    public Player Player { get; private set; }

    void Update()
    {
        Camera mainCamera = Camera.main;
        if (!mainCamera)
        {
            UnFocus();
            return;
        }

        Vector3 mousePosition = InputRegistry.Player.Mouse.action.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (!interactable)
            {
                UnFocus();
                return;
            }
            if (!CanInteract(interactable))
            {
                UnFocus();
                return;
            }
            Focus(interactable);
        }
        else
        {
            UnFocus();
        }
    }

    public bool TryInteract()
    {
        if (focusedInteractable != null)
        {
            Interact(focusedInteractable);
            return true;
        }
        return false;
    }

    public virtual bool CanInteract(Interactable interactable)
    {
        if (!interactable)
        {
            return false;
        }
        if (!interactable.CanInteract(this))
        {
            return false;
        }
        return enabled;
    }

    public void Interact(Interactable interactable)
    {
        if (!interactable)
        {
            return;
        }
        interactable.OnInteract(this);
    }

    public void Focus(Interactable interactable)
    {
        if (!interactable)
        {
            return;
        }
        if (!interactable.CanInteract(this))
        {
            return;
        }
        if (interactable == focusedInteractable)
        {
            return;
        }
        if (focusedInteractable)
        {
            focusedInteractable.OnUnFocus(this);
        }
        focusedInteractable = interactable;
        focusedInteractable.OnFocus(this);
    }

    public void UnFocus()
    {
        if (focusedInteractable != null)
        {
            focusedInteractable.OnUnFocus(this);
            focusedInteractable = null;
        }
    }
}
