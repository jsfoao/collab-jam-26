using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int maxInventorySlots = 10;
    [SerializeField] private List<Item> inventoryItems = new List<Item>();
    public void AddItemToInventory(Item item) {
        if(item == null) return;
        if(inventoryItems.Count == maxInventorySlots) {
            Debug.Log("Inventory full");
            return;
        }
        inventoryItems.Add(item); 
        item.gameObject.SetActive(false);

    }
}
