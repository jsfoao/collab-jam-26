using Input;
using UnityEditor;
using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;

    private Interactable focusedInteractable;
    public Interactable FocusedInteractable => focusedInteractable;

    public Player Player { get; private set; }

    void Awake()
    {
        Player = GetComponent<Player>();
    }

    void Update()
    {
        UpdateFocus();
    }

    private void UpdateFocus()
    {
        Camera playerCamera = Player.Camera;
        if (!playerCamera)
        {
            UnFocus();
            return;
        }

        Vector3 mousePosition = InputRegistry.Player.Mouse.action.ReadValue<Vector2>();
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);

        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

        Interactable interactableToFocus = null;
        foreach (RaycastHit hit in hits)
        {
            // Check if it's valid interactable
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (!interactable)
            {
                continue;
            }
            if (!CanInteract(interactable))
            {
                continue;
            }
            
            // Check for occlusion
            RaycastHit occlusionHit;
            Physics.Raycast(ray, out occlusionHit, Mathf.Infinity, layerMask);
            Interactable occlusionInteractable = occlusionHit.collider.GetComponent<Interactable>();
            if (occlusionHit.collider)
            {
                if (!occlusionInteractable)
                {
                    if (occlusionHit.collider != hit.collider)
                    {
                        continue;
                    }
                }
            }
            
            interactableToFocus = interactable;
            break;
        }

        if (interactableToFocus)
        {
            Focus(interactableToFocus);
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

        if (Player.ItemHandler.IsGrabbing())
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

    void OnDrawGizmos()
    {
        if (focusedInteractable)
        {
            Gizmos.color = Color.yellow;
            Collider collider = focusedInteractable.GetComponent<Collider>();
            if (collider)
            {
                string text = focusedInteractable.name;
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.yellow;
                style.fontSize = 14;
                style.alignment = TextAnchor.MiddleCenter;
                Handles.Label(focusedInteractable.transform.position + Vector3.up * 0.2f, text, style);
                Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size + Vector3.one * 0.2f);
            }
        }
    }
}
