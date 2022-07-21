using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class BagUIList : MonoBehaviour  /// @todo Comment
{
    public GameObject bagPanel;
    public List<GameObject> bagSlots;

    public Sprite emptySlot;
    public Sprite fullSlot;

    public GameObject GetBagslot(int index)
    {
        if (index < 0 || index >= bagSlots.Count) return null;
        if (bagSlots[index] == null) return null;
        return bagSlots[index];
    }

    public Image GetBagSlotBackground(int index)
    {
        if (GetBagslot(index) == null) return null;
        return GetBagslot(index).GetComponent<Image>();
    }

    public void SwitchBagSlotBackground(int index, bool isFull)
    {
        if (GetBagSlotBackground(index) == null) return;
        if (isFull) GetBagSlotBackground(index).sprite = fullSlot;
        else GetBagSlotBackground(index).sprite = emptySlot;
    }

    public Image GetBagSlotIcon(int index)
    {
        if (GetBagslot(index) == null) return null;
        Image[] imgs = GetBagslot(index).GetComponentsInChildren<Image>();
        if (imgs == null || imgs.Length <= 1) return null;
        return imgs[1];
    }

    public TextMeshProUGUI GetBagSlotText(int index)
    {
        if (GetBagslot(index) == null) return null;
        return GetBagslot(index).GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ClearSlots(){
        for (int i = 0; i < bagSlots.Count; i++)
        {
            if (bagSlots[i] == null || GetBagSlotIcon(i) == null || GetBagSlotText(i) == null) continue;
            GetBagSlotBackground(i).sprite = emptySlot;
            GetBagSlotIcon(i).enabled = false;
            GetBagSlotText(i).text = "";
        }
    }
}
