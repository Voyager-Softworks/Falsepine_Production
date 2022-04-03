using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public InputAction openBagAction;
    public InputAction openJournalAction;
    public InputAction closeAction;

    public InputAction hotkey1Action;
    public InputAction hotkey2Action;
    public InputAction hotkey3Action;

    private UIScript _uiScript;

    //this should be replaced with more complicated system if doing full prod
    [Serializable]
    public struct InventoryItem{
        public string name;
        public int hotkey;
        public int amount;
        public int maxAmount;
        public Sprite icon;
        public GameObject prefab;
    }

    public InventoryItem bearTraps = new InventoryItem();
    public InventoryItem baitMeat = new InventoryItem();
    public InventoryItem baitBird = new InventoryItem();



    // Start is called before the first frame update
    void Start()
    {
        openBagAction.Enable();
        openJournalAction.Enable();
        closeAction.Enable();
        
        hotkey1Action.Enable();
        hotkey2Action.Enable();
        hotkey3Action.Enable();

        openBagAction.performed += ctx => ToggleBag();
        openJournalAction.performed += ctx => ToggleJournal();
        closeAction.performed += ctx => CloseAll();

        hotkey1Action.performed += HotkeyPressed;
        hotkey2Action.performed += HotkeyPressed;
        hotkey3Action.performed += HotkeyPressed;

        if (_uiScript == null) _uiScript = FindObjectOfType<UIScript>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBagUI();
        UpdateHotbarUI();
    }

    private void UpdateBagUI(){
        if (_uiScript == null || _uiScript.bagUIList == null || _uiScript.bagUIList.bagPanel == null || !_uiScript.bagUIList.bagPanel.activeSelf) return;

        //clear slots
        _uiScript.bagUIList.ClearSlots();

        int currentSlot = 0;

        //bear traps
        if (bearTraps.amount > 0){
            _uiScript.bagUIList.SwitchBagSlotBackground(currentSlot, true);
            _uiScript.bagUIList.GetBagSlotIcon(currentSlot).enabled = true;
            _uiScript.bagUIList.GetBagSlotIcon(currentSlot).sprite = bearTraps.icon;
            _uiScript.bagUIList.GetBagSlotText(currentSlot).text = bearTraps.amount.ToString();
            currentSlot++;
        }

        //bait meat
        if (baitMeat.amount > 0){
            _uiScript.bagUIList.SwitchBagSlotBackground(currentSlot, true);
            _uiScript.bagUIList.GetBagSlotIcon(currentSlot).enabled = true;
            _uiScript.bagUIList.GetBagSlotIcon(currentSlot).sprite = baitMeat.icon;
            _uiScript.bagUIList.GetBagSlotText(currentSlot).text = baitMeat.amount.ToString();
            currentSlot++;
        }

        //bait bird
        if (baitBird.amount > 0){
            _uiScript.bagUIList.SwitchBagSlotBackground(currentSlot, true);
            _uiScript.bagUIList.GetBagSlotIcon(currentSlot).enabled = true;
            _uiScript.bagUIList.GetBagSlotIcon(currentSlot).sprite = baitBird.icon;
            _uiScript.bagUIList.GetBagSlotText(currentSlot).text = baitBird.amount.ToString();
            currentSlot++;
        }
    }

    private void UpdateHotbarUI()
    {
        if (_uiScript == null || _uiScript.hotbarUIList == null || _uiScript.hotbarUIList.hotbarPanel == null || !_uiScript.hotbarUIList.hotbarPanel.activeSelf) return;

        //clear slots
        _uiScript.hotbarUIList.ClearSlots();

        int currentSlot = 0;

        //bear traps
        if (bearTraps.amount > 0){
            _uiScript.hotbarUIList.GetHotbarSlotIcon(currentSlot).enabled = true;
            _uiScript.hotbarUIList.GetHotbarSlotIcon(currentSlot).sprite = bearTraps.icon;
            _uiScript.hotbarUIList.GetHotbarSlotCountText(currentSlot).text = bearTraps.amount.ToString();
        }
        currentSlot++;

        //bait meat
        if (baitMeat.amount > 0){
            _uiScript.hotbarUIList.GetHotbarSlotIcon(currentSlot).enabled = true;
            _uiScript.hotbarUIList.GetHotbarSlotIcon(currentSlot).sprite = baitMeat.icon;
            _uiScript.hotbarUIList.GetHotbarSlotCountText(currentSlot).text = baitMeat.amount.ToString();
        }
        currentSlot++;

        //bait bird
        if (baitBird.amount > 0){
            _uiScript.hotbarUIList.GetHotbarSlotIcon(currentSlot).enabled = true;
            _uiScript.hotbarUIList.GetHotbarSlotIcon(currentSlot).sprite = baitBird.icon;
            _uiScript.hotbarUIList.GetHotbarSlotCountText(currentSlot).text = baitBird.amount.ToString();
        }
        currentSlot++;
    }

    public void HotkeyPressed(InputAction.CallbackContext ctx){
        if (ctx.performed)
        {
            if (ctx.action == hotkey1Action)
            {
                UseHotkey(1);
            }
            else if (ctx.action == hotkey2Action)
            {
                UseHotkey(2);
            }
            else if (ctx.action == hotkey3Action)
            {
                UseHotkey(3);
            }
        }
    }

    public void UseHotkey(int hotkey){
        InventoryItem item = GetHotkeyItem(hotkey);

        if (item.amount > 0){
            item.amount--;
            if (item.prefab != null){
                GameObject obj = Instantiate(item.prefab, transform.position + transform.forward, Quaternion.identity);
                if (obj.GetComponent<Rigidbody>()) obj.GetComponent<Rigidbody>().AddForce(transform.forward * 10, ForceMode.Impulse);
            }
        }
    }

    private InventoryItem GetHotkeyItem(int hotkey)
    {
        if (hotkey == bearTraps.hotkey) return bearTraps;
        else if (hotkey == baitMeat.hotkey) return baitMeat;
        else if (hotkey == baitBird.hotkey) return baitBird;
        else return new InventoryItem();
    }

    public void SetHotkey(int slot, int hotkey){
        RemoveBind(hotkey);

        if (slot == 0){
            bearTraps.hotkey = hotkey;
        }
        else if (slot == 1){
            baitMeat.hotkey = hotkey;
        }
        else if (slot == 2){
            baitBird.hotkey = hotkey;
        }
    }

    public void RemoveBind(int hotkey){
        if (bearTraps.hotkey == hotkey){
            bearTraps.hotkey = -1;
        }
        else if (baitMeat.hotkey == hotkey){
            baitMeat.hotkey = -1;
        }
        else if (baitBird.hotkey == hotkey){
            baitBird.hotkey = -1;
        }
    }

    public void AddBearTrap(int amount){
        bearTraps.amount += amount;
        bearTraps.amount = Mathf.Clamp(bearTraps.amount, 0, bearTraps.maxAmount);
        UpdateBagUI();
    }

    public void AddBaitMeat(int amount){
        baitMeat.amount += amount;
        baitMeat.amount = Mathf.Clamp(baitMeat.amount, 0, baitMeat.maxAmount);
        UpdateBagUI();
    }

    public void AddBaitBird(int amount){
        baitBird.amount += amount;
        baitBird.amount = Mathf.Clamp(baitBird.amount, 0, baitBird.maxAmount);
        UpdateBagUI();
    }

#region OpenCloseScripts

    public void OpenJournal()
    {
        if (_uiScript == null || _uiScript.journalUIList == null || _uiScript.journalUIList.journalPanel == null) return;

        CloseAll();

        _uiScript.journalUIList.journalPanel.SetActive(true);
    }
    public void CloseJournal()
    {
        if (_uiScript == null || _uiScript.journalUIList == null || _uiScript.journalUIList.journalPanel == null) return;

        _uiScript.journalUIList.journalPanel.SetActive(false);
    }
    public void ToggleJournal()
    {
        if (_uiScript == null || _uiScript.journalUIList == null || _uiScript.journalUIList.journalPanel == null) return;

        if (_uiScript.journalUIList.journalPanel.activeSelf) CloseJournal();
        else OpenJournal();
    }

    public void OpenBag()
    {
        if (_uiScript == null || _uiScript.bagUIList == null || _uiScript.bagUIList.bagPanel == null) return;

        CloseAll();

        _uiScript.bagUIList.bagPanel.SetActive(true);
    }
    public void CloseBag()
    {
        if (_uiScript == null || _uiScript.bagUIList == null || _uiScript.bagUIList.bagPanel == null) return;

        _uiScript.bagUIList.bagPanel.SetActive(false);
    }
    public void ToggleBag()
    {
        if (_uiScript == null || _uiScript.bagUIList == null || _uiScript.bagUIList.bagPanel == null) return;

        if (_uiScript.bagUIList.bagPanel.activeSelf) CloseBag();
        else OpenBag();
    }

    public void CloseAll()
    {
        CloseJournal();
        CloseBag();
    }

#endregion
}
