using UnityEngine;

public class StageProp : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] components;

    void Awake()
    {
        enabled = false;
        OnDisable();
    }

    void OnEnable()
    {
        // Enable interactables
        Interactable[] interactables = GetComponentsInChildren<Interactable>();
        foreach (Interactable interactable in interactables)
        {
            interactable.enabled = true;
        }

        // Enable links
        Link[] links = GetComponentsInChildren<Link>();
        foreach (Link link in links)
        {
            link.enabled = true;
        }

        foreach (MonoBehaviour component in components)
        {
            if (component)
            {
                component.enabled = true;
            }
        }
    }

    void OnDisable()
    {
        // Disable interactables
        Interactable[] interactables = GetComponentsInChildren<Interactable>();
        foreach (Interactable interactable in interactables)
        {
            interactable.enabled = false;
        }

        // Disable links
        Link[] links = GetComponentsInChildren<Link>();
        foreach (Link link in links)
        {
            link.enabled = false;
        }

        foreach (MonoBehaviour component in components)
        {
            if (component)
            {
                component.enabled = false;
            }
        }
    }
}