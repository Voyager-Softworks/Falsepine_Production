using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Inventory pannel for the shop specifically.
/// </summary>
public class InventoryPannel_Shop : InventoryPannel
{

    public ItemDisplay itemToBuy = null;
    public Button buyButton;
    public TextMeshProUGUI buyButtonText;
    public TextMeshProUGUI buyButtonPriceText;
    public Image buyButtonIcon;

    public Sprite selectedSprite;
    public Sprite unselectedSprite;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    private void OnEnable() {
        // unbind buy button
        buyButton.onClick.RemoveAllListeners();

        UpdateBuyButton();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
    }

    protected override void UpdateItemDisplays()
    {
        base.UpdateItemDisplays();
    }

    public override void ItemClicked(ItemDisplay gridItem)
    {
        base.ItemClicked(gridItem);
    }

    /// <summary>
    /// Tries to buy the item.
    /// @todo Make this work again with new UI.
    /// </summary>
    /// <param name="gridItem"></param>
    /// <param name="sourceInventory"></param>
    /// <param name="targetInventory"></param>
    /// <param name="sourceIndex"></param>
    protected override void PerformClickAction(ItemDisplay _itemDisplay, Inventory _sourceInventory, Inventory _targetInventory, int _sourceIndex)
    {
        Item item = _itemDisplay.m_linkedItem;
        if (item == null) return;

        // unselected sprite
        if (itemToBuy != null)
        {
            itemToBuy.m_backgroundImage.sprite = unselectedSprite;
        }

        itemToBuy = _itemDisplay;

        // selected sprite
        itemToBuy.m_backgroundImage.sprite = selectedSprite;

        // unbind buy button
        buyButton.onClick.RemoveAllListeners();
        // bind buy button
        buyButton.onClick.AddListener(() => TryBuyItem(_sourceInventory, _targetInventory, _sourceIndex));

        UpdateBuyButton();
    }

    public void TryBuyItem(Inventory _sourceInventory, Inventory _targetInventory, int _sourceIndex){
        if (itemToBuy == null) return;

        EconomyManager economyManager = EconomyManager.instance;
        if (economyManager == null) return;

        int price = itemToBuy.m_linkedItem.GetPrice();
        if (economyManager.CanAfford(price)){
            //transfer
            if (InventoryManager.instance.CanMoveItem(_sourceInventory, _targetInventory, _sourceIndex)){
                economyManager.SpendMoney(price);
                InventoryManager.instance.TryMoveItem(_sourceInventory, _targetInventory, _sourceIndex);

                // message
                if (MessageManager.instance) {
                    MessageManager.instance.AddMessage("You bought a " + itemToBuy.m_linkedItem.m_displayName, "bag", true);
                }
                // notify
                NotificationManager.instance?.AddIconAtPlayer("bag");

                // sound
                UIAudioManager.instance?.buySound.Play();
            }

            // unbind buy button
            buyButton.onClick.RemoveAllListeners();
            // unselect item
            itemToBuy.m_backgroundImage.sprite = unselectedSprite;
            itemToBuy = null;
            UpdateBuyButton();
        }
        else{
            // sound
            UIAudioManager.instance?.errorSound.Play();
        }
    }

    public void UpdateBuyButton(){
        if (itemToBuy == null){
            buyButton.interactable = false;
            buyButtonText.text = "NO ITEM";
            buyButtonPriceText.text = "0";
        } else {
            buyButton.interactable = true;
            buyButtonText.text = "PURCHASE";
            buyButtonPriceText.text = itemToBuy.m_linkedItem.GetPrice().ToString();
        }
    }
}
