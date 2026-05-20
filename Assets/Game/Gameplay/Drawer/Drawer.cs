using System.Collections;
using UnityEngine;

public class Drawer : MonoBehaviour, IInteractable
{
    [SerializeField] private Interactable interactable;
    [SerializeField] private TransformAnimator transformAnimator;

    void Start()
    {
        interactable.contexts.Add(this);
    }

    public bool CanInteract(PlayerInteractionHandler interactionHandler)
    {
        if (transformAnimator.Animating)
        {
            return false;
        }
        return true;
    }

    public void OnInteract(PlayerInteractionHandler interactionHandler)
    {
        transformAnimator.StartAnim();
    }

    public void OnFocus(PlayerInteractionHandler interactionHandler)
    {
    }

    public void OnUnFocus(PlayerInteractionHandler interactionHandler)
    {
    }
}
