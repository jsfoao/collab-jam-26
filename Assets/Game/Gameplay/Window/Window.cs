using System;
using UnityEngine;

public class Window : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Interactable interactable;
    [SerializeField] private TransformAnimator animator;

    void Start()
    {
        interactable.contexts.Add(this);
    }

    public bool CanInteract(PlayerInteractionHandler interactionHandler)
    {
        if (animator.Animating)
        {
            return false;
        }
        return true;
    }

    public void OnInteract(PlayerInteractionHandler interactionHandler)
    {
        animator.StartAnim();
    }

    public void OnFocus(PlayerInteractionHandler interactionHandler)
    {
    }

    public void OnUnFocus(PlayerInteractionHandler interactionHandler)
    {
    }
}
