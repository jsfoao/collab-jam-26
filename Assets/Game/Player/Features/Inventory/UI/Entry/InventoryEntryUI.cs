using UnityEngine;
using UnityEngine.UI;

public class InventoryEntryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private RawImage icon;
    [SerializeField] private GameObject itemRendererPrefab;

    [Header("Inventory Entry")]
    [SerializeField] private float itemRotationSpeed = 40f;
    [SerializeField] private Vector2Int renderTextureSize = new Vector2Int(80, 80);
    [SerializeField] private float itemDisplayOffsetZ = 0.1f;
    [SerializeField] private float itemDisplayOffsetSpeed = 1f;

    [System.NonSerialized] public InventoryUI inventoryUI;
    [System.NonSerialized] public int index = -1;
    private InventoryEntryRenderer itemRenderer;
    private RenderTexture renderTexture;
    private Item currentItem;
    private GameObject currentItemProxy;
    private Vector3 attachBaseLocalPosition;
    private bool isPointerOver;

    public void Initialize()
    {
        itemRenderer = Instantiate(itemRendererPrefab).GetComponent<InventoryEntryRenderer>();
        itemRenderer.transform.position = Vector3.right * index * 10f;
        itemRenderer.transform.rotation = Quaternion.identity;
        attachBaseLocalPosition = itemRenderer.attachItem.transform.localPosition;

        // Create render texture
        renderTexture = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 16);
        renderTexture.Create();

        itemRenderer.renderCamera.targetTexture = renderTexture;
        itemRenderer.gameObject.SetActive(true);
        icon.texture = renderTexture;
        icon.enabled = true;

        InventoryEntryButton entryButton = button.GetComponent<InventoryEntryButton>();
        entryButton.onPointerDown += OnPointerDown;
        entryButton.onPointerEnter += OnPointerEnter;
        entryButton.onPointerExit += OnPointerExit;

        ClearItem();
    }

    public void SetItem(Item item)
    {
        if (!item)
        {
            ClearItem();
            return;
        }
        currentItem = item;
        
        // Instantiate empty proxy object with mesh renderer for display in inventory
        currentItemProxy = new GameObject("Item Proxy");
        currentItemProxy.transform.SetParent(itemRenderer.attachItem.transform);
        currentItemProxy.transform.localPosition = Vector3.zero;
        currentItemProxy.transform.localRotation = Quaternion.identity;
        currentItemProxy.transform.localScale = item.meshRenderer.transform.localScale;
        currentItemProxy.layer = LayerMask.NameToLayer("Item");
        MeshFilter meshFilter = currentItemProxy.AddComponent<MeshFilter>();
        meshFilter.mesh = item.meshRenderer.GetComponent<MeshFilter>().sharedMesh;
        MeshRenderer meshRenderer = currentItemProxy.AddComponent<MeshRenderer>();
        meshRenderer.materials = item.meshRenderer.sharedMaterials;
        Material material = meshRenderer.material;
        material.SetFloat("_Glow", 0.0f);

        button.interactable = true;
        button.image.enabled = true;
        button.enabled = true;
    }

    public void ClearItem()
    {
        currentItem = null;
        Destroy(currentItemProxy);
        currentItemProxy = null;
        
        button.interactable = false;
        button.image.enabled = false;
        button.enabled = false;
    }

    private void Update()
    {
        if (currentItem)
        {
            itemRenderer.attachItem.transform.localRotation *= Quaternion.Euler(Vector3.up * itemRotationSpeed * Time.deltaTime);
            float localPosZ = isPointerOver ? itemDisplayOffsetZ : attachBaseLocalPosition.z;

            Vector3 targetLocalPos = Vector3.forward * localPosZ;
            Vector3 newLocalPos = Vector3.MoveTowards(itemRenderer.attachItem.transform.localPosition, targetLocalPos, Time.deltaTime * itemDisplayOffsetSpeed);
            itemRenderer.attachItem.transform.localPosition = newLocalPos; 
        }
        else
        {
            itemRenderer.attachItem.transform.localRotation = Quaternion.identity;
            itemRenderer.attachItem.transform.localPosition = attachBaseLocalPosition;
        }
    }

    private void OnPointerEnter()
    {
        isPointerOver = true;
    }

    private void OnPointerExit()
    {
        isPointerOver = false;
    }

    private void OnPointerDown()
    {
        if (!currentItem)
        {
            return;
        }

        Debug.Log($"Down inventory entry {index} with item {currentItem.name}");

        Item item = currentItem;
        GameManager gameManager = GameManager.Instance;

        Player player = gameManager.Player;
        PlayerItemHandler itemHandler = player.ItemHandler;


        itemHandler.Drop();
        
        // Move item to target position
        item.gameObject.transform.position = player.Camera.transform.position - Vector3.up * 0.2f;

        // Attach to current stage
        Stage stage = gameManager.StageManager.CurrentStage;
        if (!stage)
        {
            return;
        }
        item.transform.SetParent(stage.transform);
        
        item.gameObject.SetActive(true);
        
        // Finish and grab item
        itemHandler.Grab(item);
    }
}
