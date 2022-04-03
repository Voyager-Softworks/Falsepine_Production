using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class HotbarUIList : MonoBehaviour
{
    public GameObject hotbarPanel;

    public List<GameObject> hotbarSlots;

    public GameObject GetHotbarSlot(int index)
    {
        if (index < 0 || index >= hotbarSlots.Count) return null;
        if (hotbarSlots[index] == null) return null;
        return hotbarSlots[index];
    }

    public Image GetHotbarSlotBackground(int index)
    {
        if (GetHotbarSlot(index) == null) return null;
        return GetHotbarSlot(index).GetComponent<Image>();
    }

    public Image GetHotbarSlotIcon(int index)
    {
        if (GetHotbarSlot(index) == null) return null;
        Image[] imgs = GetHotbarSlot(index).GetComponentsInChildren<Image>();
        if (imgs == null || imgs.Length <= 1) return null;
        return imgs[1];
    }

    public TextMeshProUGUI GetHotbarSlotHotkeyText(int index)
    {
        if (GetHotbarSlot(index) == null) return null;
        TextMeshProUGUI[] texts = GetHotbarSlot(index).GetComponentsInChildren<TextMeshProUGUI>();
        if (texts == null || texts.Length <= 0) return null;
        return texts[0];
    }

    public TextMeshProUGUI GetHotbarSlotCountText(int index)
    {
        if (GetHotbarSlot(index) == null) return null;
        TextMeshProUGUI[] texts = GetHotbarSlot(index).GetComponentsInChildren<TextMeshProUGUI>();
        if (texts == null || texts.Length <= 1) return null;
        return texts[1];
    }

    public void ClearSlots(){
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            if (hotbarSlots[i] == null || GetHotbarSlotIcon(i) == null || GetHotbarSlotHotkeyText(i) == null || GetHotbarSlotCountText(i) == null) continue;
            GetHotbarSlotIcon(i).enabled = false;
            //GetHotbarSlotHotkeyText(i).text = "";
            GetHotbarSlotCountText(i).text = "";
        }
    }
}
