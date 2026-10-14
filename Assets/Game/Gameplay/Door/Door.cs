using System.Collections;
using FMOD.Studio;
using FMODUnity;
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

    [Header("Audio")]
    [SerializeField] private EventReference unlockSound;

    public bool IsLocked { get; private set; } = true;
    public bool IsOpen { get; private set; } = false;

    EventReference soundToPlay;

    StudioEventEmitter eventEmitter;


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
        RuntimeManager.PlayOneShot(unlockSound);
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
        StartCoroutine(AudioCoroutine());
        interactable.enabled = false;
    }

    IEnumerator AudioCoroutine()
    {
        float lowFreq = 0f;
        float duration = 1f;
        float targetLowFreq = 1f;
        AudioManager audioManager = AudioManager.Instance;
        if (!audioManager)
        {
            yield break;
        }
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = time / duration;
            lowFreq = Mathf.Lerp(0f, targetLowFreq, alpha);
            audioManager.ambienceEmitter.EventInstance.setParameterByName("LowFreq", lowFreq);
            yield return null;
        }
        audioManager.ambienceEmitter.EventInstance.setParameterByName("LowFreq", targetLowFreq);
        yield return null;
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