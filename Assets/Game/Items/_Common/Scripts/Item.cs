using UnityEngine;
using UnityEngine.VFX;

public class Item : MonoBehaviour, IInteractable, ILinkable
{
    [Header("Components")]
    [SerializeField] public Interactable Interactable;
    [SerializeField] public Collider Collider;
    [SerializeField] public MeshRenderer meshRenderer;

    [Header("VFX")]
    [SerializeField] private VisualEffect hitVFX;

    private PlayerItemHandler currentHandler;
    public PlayerItemHandler CurrentHandler => currentHandler;

    public Link Link { get; private set; }


    void Awake()
    {
        Interactable.contexts.Add(this);

        Link = GetComponent<Link>();
        Link.contexts.Add(this);
    }


    void Update()
    {
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
        Link.enabled = true;
    }

    public virtual void OnDrop(PlayerItemHandler itemHandler)
    {
        currentHandler = null;
        Interactable.enabled = true;
        Link.enabled = false;

        // GameManager gameManager = GameManager.Instance;
        // StageManager stageManager = gameManager.StageManager;
        // Stage currentStage = stageManager.CurrentStage;
        // if (currentStage.gameObject.name != "Stage_Workbench")
        // {
        //     gameObject.SetActive(false);
        // }
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
        return true;
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

    void OnDestroy()
    {
        // Remove from inventory
        GameManager gameManager = GameManager.Instance;
        Player player = gameManager.Player;
        PlayerInventory playerInventory = player.Inventory;
        playerInventory.RemoveItemFromInventory(this);
    }
}