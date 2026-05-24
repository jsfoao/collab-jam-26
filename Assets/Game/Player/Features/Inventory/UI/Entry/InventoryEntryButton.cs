using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryEntryButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action onPointerDown;
    public Action onPointerEnter;
    public Action onPointerExit;

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke();
    }
}
