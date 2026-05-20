using UnityEngine;
using UnityEngine.VFX;

public class Item : MonoBehaviour, IInteractable, ILinkable
{
    private PlayerItemHandler currentHandler;
    public PlayerItemHandler CurrentHandler => currentHandler;

    public Interactable Interactable { get; private set; }
    public Link Link { get; private set; }

    [SerializeField] public MeshRenderer meshRenderer;

    [SerializeField] bool displayItem = false;

    [Header("VFX")]
    [SerializeField] private VisualEffect hitVFX;

    void Awake()
    {
        Interactable = GetComponent<Interactable>();
        Interactable.contexts.Add(this);

        Link = GetComponent<Link>();
        Link.contexts.Add(this);
    }


    void Update()
    {
        if (displayItem)
        {
            transform.Rotate(Vector3.up, 40f * Time.deltaTime);
        }
        UpdateGlowMaterial();

        foreach (Link link in Link.links)
        {
            if (!link)
            {
                continue;
            }
            Debug.DrawLine(transform.position, link.transform.position, Color.green, 0f, false);
        }
    }

    // Glow if not in inventory
    private void UpdateGlowMaterial()
    {
        GameManager gameManager = GameManager.Instance;
        if (!gameManager)
        {
            return;
        }
        Player player = gameManager.Player;
        if (!player)
        {
            return;
        }
        PlayerInventory playerInventory = player.Inventory;
        if (!playerInventory)
        {
            return;
        }
        if (!meshRenderer)
        {
            return;
        }
        Material material = meshRenderer.material;
        if (!material)
        {
            return;
        }
        if (displayItem)
        {
            material.SetFloat("_Glow", 0.0f);
            return;
        }

        bool hasItem = playerInventory.HasItem(this);
        float glowValue = hasItem ? 0.0f : 1.0f;
        material.SetFloat("_Glow", glowValue);

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

    void OnCollisionEnter(Collision collision)
    {
        if (!hitVFX)
        {
            return;
        }
        Quaternion rotation = Quaternion.LookRotation(collision.contacts[0].normal);
        Vector3 position = collision.contacts[0].point;
        VisualEffect hitVFXInstance = Instantiate(hitVFX, position, rotation);
        Destroy(hitVFXInstance.gameObject, 0.4f);
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
        Debug.Log("Add to inventory");
        PlayerInventory playerInventory = interactionHandler.GetComponent<PlayerInventory>();
        if (!playerInventory.HasItem(this))
        {
            playerInventory.AddItemToInventory(this);
            return;
        }
        
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