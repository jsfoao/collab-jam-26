using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool CanInteract(PlayerInteractionHandler interactionHandler);
    public void OnInteract(PlayerInteractionHandler interactionHandler);
    public void OnFocus(PlayerInteractionHandler interactionHandler);
    public void OnUnFocus(PlayerInteractionHandler interactionHandler);
}

public class Interactable : MonoBehaviour, IInteractable
{
    public List<IInteractable> contexts = new List<IInteractable>();

    public virtual bool CanInteract(PlayerInteractionHandler interactionHandler)
    {
        if (!enabled)
        {
            return false;
        }
        if (contexts.Count > 0)
        {
            foreach (IInteractable context in contexts)
            {
                if (!context.CanInteract(interactionHandler))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public virtual void OnInteract(PlayerInteractionHandler interactionHandler)
    {
        foreach (IInteractable context in contexts)
        {
            if (context.CanInteract(interactionHandler))
            {
                context.OnInteract(interactionHandler);
            }
        }
        Debug.Log($"Interacted with {gameObject.name}");
    }

    public virtual void OnFocus(PlayerInteractionHandler interactionHandler)
    {
        foreach (IInteractable context in contexts)
        {
            context.OnFocus(interactionHandler);
        }

        Debug.Log($"Focused on {gameObject.name}");
    }

    public virtual void OnUnFocus(PlayerInteractionHandler interactionHandler)
    {
        foreach (IInteractable context in contexts)
        {
            context.OnUnFocus(interactionHandler);
        }

        Debug.Log($"Unfocused from {gameObject.name}");
    }
}