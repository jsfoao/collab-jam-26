using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryEntryUI entryPrefab;
    [SerializeField] private HorizontalLayoutGroup layoutGroup;

    [System.NonSerialized] public List<InventoryEntryUI> entries = new List<InventoryEntryUI>();
    [System.NonSerialized] public PlayerInventory playerInventory;

    public void Initialize()
    {

        if (!playerInventory)
        {
            return;
        }
        for (int i = 0; i < playerInventory.maxInventorySlots; i++)
        {
            InventoryEntryUI entry = Instantiate(entryPrefab, layoutGroup.transform);
            entry.index = i;
            entries.Add(entry);
            entry.inventoryUI = this;
            entry.Initialize();
        }
    }
}
