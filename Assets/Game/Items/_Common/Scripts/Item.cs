using UnityEngine;

public class Item : MonoBehaviour, IInteractable, ILinkable
{
    [SerializeField] private ItemData itemData;
    public ItemData ItemData => itemData;
    
    private PlayerItemHandler currentHandler;
    public PlayerItemHandler CurrentHandler => currentHandler;

    public Interactable Interactable { get; private set; }
    public Link Link { get; private set; }

    void Awake()
    {
        Interactable = GetComponent<Interactable>();
        Interactable.contexts.Add(this);

        Link = GetComponent<Link>();
        Link.contexts.Add(this);
    }


    void Update()
    {
        foreach (Link link in Link.links)
        {
            Debug.DrawLine(transform.position, link.transform.position, Color.green, 0f, false);
        }
    }

    // --- Item ---

    public virtual bool CanGrab(PlayerItemHandler itemHandler)
    {
        return enabled;
    }

    public virtual void OnGrab(PlayerItemHandler itemHandler)
    {
        currentHandler = itemHandler;
        Interactable.enabled = false;
    }

    public virtual void OnDrop(PlayerItemHandler itemHandler)
    {
        currentHandler = null;
        Interactable.enabled = true;
    }

    // --- ILinkable ---

    public virtual bool CanLink(Link otherLink)
    {
        return enabled;
    }

    public virtual void OnLink(Link otherLink)
    {
    }

    public void OnUnLink(Link otherLink)
    {
    }

    // --- IInteractable ---

    public bool CanInteract(PlayerInteractionHandler interactionHandler)
    {
        return enabled;
    }

    public void OnInteract(PlayerInteractionHandler interactionHandler)
    {
        // Debug.Log("Add to inventory");
        // PlayerInventory playerInventory = interactionHandler.GetComponent<PlayerInventory>();
        // playerInventory.AddItemToInventory(this);

        PlayerItemHandler itemHandler = interactionHandler.Player.ItemHandler;
        itemHandler.Grab(this);
    }

    public void OnFocus(PlayerInteractionHandler interactionHandler)
    {
    }

    public void OnUnFocus(PlayerInteractionHandler interactionHandler)
    {
    }
}