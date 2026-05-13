using UnityEngine;

public class GameplayID : MonoBehaviour
{
    [SerializeField] public GameplayObjectID ID;


    void Start()
    {
        if (!ID)
        {
            Debug.LogWarning($"GameplayID on {gameObject.name} does not have an ID assigned.");
        }
    }
}