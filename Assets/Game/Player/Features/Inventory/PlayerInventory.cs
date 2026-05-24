using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] public int maxInventorySlots = 10;
    [SerializeField] private List<Item> inventoryItems = new List<Item>();

    [SerializeField] private GameObject inventoryUIPrefab;
    private InventoryUI inventoryUI;

    void Start()
    {
        // Spawn inventory UI
        if (inventoryUIPrefab)
        {
            GameManager gameManager = GameManager.Instance;
            if (!gameManager)
            {
                return;
            }
            inventoryUI = Instantiate(inventoryUIPrefab, gameManager.Canvas.transform).GetComponent<InventoryUI>();
            inventoryUI.playerInventory = this;
            inventoryUI.Initialize();
        }
    }

    public void AddItemToInventory(Item item) 
    {
        if(item == null) 
        {
            return;
        }
        if(inventoryItems.Count == maxInventorySlots) 
        {
            Debug.Log("Inventory full");
            return;
        }
        inventoryItems.Add(item); 

        // Disable item in world
        item.gameObject.SetActive(false);
        
        // Update UI
        InventoryEntryUI entryUI = inventoryUI.entries[inventoryItems.Count - 1];
        entryUI.SetItem(item);
    }

    public void RemoveItemFromInventory(Item item) 
    {
        if(item == null) 
        {
            return;
        }
        if (!inventoryItems.Contains(item)) 
        {
            return;
        }

        // Update UI
        int index = inventoryItems.IndexOf(item);
        bool removed = inventoryItems.Remove(item);
        if (!removed) 
        {
            return;
        }
        InventoryEntryUI entryUI = inventoryUI.entries[index];
        entryUI.ClearItem();
    }

    public bool HasItem(Item item) 
    {
        return inventoryItems.Contains(item);
    }
}
