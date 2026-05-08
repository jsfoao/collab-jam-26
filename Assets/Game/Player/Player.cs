using Input;
using UnityEngine;

public class Player : MonoBehaviour
{   
    [SerializeField] private new Camera camera;
    public Camera Camera => camera;

    public PlayerItemHandler ItemHandler { get; private set; }
    public PlayerInteractionHandler InteractionHandler { get; private set; }

    [SerializeField] private PlayerCursorUI cursorPrefab;
    public PlayerCursorUI Cursor { get; private set; }

    void Awake()
    {
        ItemHandler = GetComponent<PlayerItemHandler>();
        InteractionHandler = GetComponent<PlayerInteractionHandler>();
        Camera.SetupCurrent(camera);
    }

    void Start()
    {
        // Spawn cursor
        if (cursorPrefab)
        {
            GameManager gameManager = GameManager.Instance;
            Cursor = Instantiate(cursorPrefab, transform);
            Cursor.transform.SetParent(gameManager.Canvas.transform, false);
        }

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
