using UnityEngine;

public class Door : MonoBehaviour, ILinkable, IInteractable
{
    [Header("Components")]
    [SerializeField] private Link link;
    [SerializeField] private Interactable interactable;
    [SerializeField] private TransformAnimator animator;

    [Header("Door")]
    [SerializeField] private GameObject hingePivot;
    [SerializeField] private AnimationCurve openAnimCurve;
    [SerializeField] private bool startLocked = true;

    public bool IsLocked { get; private set; } = true;
    public bool IsOpen { get; private set; } = false;

    void Awake()
    {
        link.contexts.Add(this);
        interactable.contexts.Add(this);

        if (startLocked)
        {
            IsLocked = true;
        }
        else
        {
            IsLocked = false;
        }
    }

    public void UnLock()
    {
        IsLocked = false;
    }

    // --- ILinkable ---

    public bool CanLink(Link otherLink)
    {
        return true;
    }

    public void OnLink(Link otherLink)
    {
        Destroy(otherLink.gameObject);
        UnLock();
    }

    public void OnUnLink(Link otherLink)
    {
    }

    // --- IInteractable ---

    public bool CanInteract(PlayerInteractionHandler interactionHandler)
    {
        return true;
    }

    public void OnInteract(PlayerInteractionHandler interactionHandler)
    {
        if (IsLocked)
        {
            return;
        }
        animator.StartForwardAnim();
        interactable.enabled = false;
    }

    public void OnFocus(PlayerInteractionHandler interactionHandler)
    {
    }

    public void OnUnFocus(PlayerInteractionHandler interactionHandler)
    {
    }

    // --- Editor ---

    void OnDrawGizmosSelected()
    {
        if (hingePivot)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hingePivot.transform.position, 0.1f);
        }
    }
}