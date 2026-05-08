using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, ILinkable, IInteractable
{
    [Header("References")]
    [SerializeField] private Link link;
    [SerializeField] private Interactable interactable;

    [Header("Door")]
    [SerializeField] private GameObject hingePivot;
    [SerializeField] private AnimationCurve openAnimCurve;
    [SerializeField] private float openAnimDuration = 1f;
    [SerializeField] private bool openReverse = false;
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

    // --- Open ---

    private bool CanOpen()
    {
        if (IsLocked)
        {
            return false;
        }
        if (!enabled)
        {
            return false;
        }
        return true;
    }


    public void Open()
    {
        if (IsOpen)
        {
            return;
        }
        if (!CanOpen())
        {
            return;
        }
        if (!hingePivot)
        {
            return;            
        }
        
        IsOpen = true;
        StartCoroutine(StartOpenAnim());
    }

    private IEnumerator StartOpenAnim()
    {
        float time = 0f;
        while (time < openAnimDuration)
        {
            float angle = openAnimCurve.Evaluate(time / openAnimDuration);
            if (openReverse)
            {
                angle = -angle;
            }
            hingePivot.transform.localRotation = Quaternion.Euler(0f, angle, 0f);
            time += Time.deltaTime;
            yield return null;
        }

        FinishOpenAnim();
    }

    private void FinishOpenAnim()
    {
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
        Open();
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