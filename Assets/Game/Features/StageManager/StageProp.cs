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
        foreach (MonoBehaviour component in components)
        {
            if (component)
            {
                component.enabled = false;
            }
        }
    }
}