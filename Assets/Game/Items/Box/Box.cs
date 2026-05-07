using UnityEngine;

[RequireComponent(typeof(Item))]
public class Box : MonoBehaviour, IInteractable, ILinkable
{
    void Start()
    {
        Item item = GetComponent<Item>();
        item.Interactable.contexts.Add(this);
        item.Link.contexts.Add(this);
    }

    // --- IInteractable ---
    
    public bool CanInteract(PlayerInteractionHandler interactionHandler)
    {
        return true;
    }

    public void OnFocus(PlayerInteractionHandler interactionHandler)
    {
        
    }

    public void OnInteract(PlayerInteractionHandler interactionHandler)
    {
    }

    public void OnUnFocus(PlayerInteractionHandler interactionHandler)
    {
    }

    // --- ILinkable ---

    public bool CanLink(Link otherLink)
    {
        return true;
    }

    public void OnLink(Link otherLink)
    {
    }

    public void OnUnLink(Link otherLink)
    {
    }
}
