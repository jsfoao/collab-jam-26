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
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 cursorSize = new Vector2(32f, 32f);

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
    }

    public virtual void OnFocus(PlayerInteractionHandler interactionHandler)
    {
        // Set cursor texture
        if (cursorTexture)
        {
            Player player = interactionHandler.Player;
            PlayerCursorUI cursor = player.Cursor;
            cursor.SetCursorSize(cursorSize);
            cursor.SetCursorTexture(cursorTexture);
        }

        foreach (IInteractable context in contexts)
        {
            context.OnFocus(interactionHandler);
        }
    }

    public virtual void OnUnFocus(PlayerInteractionHandler interactionHandler)
    {
        // Clear cursor texture
        if (cursorTexture)
        {
            Player player = interactionHandler.Player;
            PlayerCursorUI cursor = player.Cursor;
            cursor.ClearCursorTexture();
        }

        foreach (IInteractable context in contexts)
        {
            context.OnUnFocus(interactionHandler);
        }
    }
}