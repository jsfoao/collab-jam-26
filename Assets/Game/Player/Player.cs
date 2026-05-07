using Input;
using UnityEngine;

public class Player : MonoBehaviour
{   
    public PlayerItemHandler ItemHandler { get; private set; }
    public PlayerInteractionHandler InteractionHandler { get; private set; }

    void Awake()
    {
        ItemHandler = GetComponent<PlayerItemHandler>();
        InteractionHandler = GetComponent<PlayerInteractionHandler>();
    }

    void Start()
    {
        SetupInput();
    }

    private void SetupInput()
    {
        InputRegistry.Player.Interact.action.performed += ctx => OnInteractInputPerformed();
        InputRegistry.Player.Interact.action.canceled += ctx => OnInteractInputCanceled();
    }

    private void OnInteractInputPerformed()
    {
        InteractionHandler.TryInteract();
    }

    private void OnInteractInputCanceled()
    {
        ItemHandler.TryLink();
        ItemHandler.Drop();    
    }
}
