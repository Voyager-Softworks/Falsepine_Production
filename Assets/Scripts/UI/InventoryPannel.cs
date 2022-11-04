using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

/// <summary>
/// Class used to display a specific inventory UI, as well as manage the rules around its interaction.
/// </summary>
public class InventoryPannel : MonoBehaviour
{
    public string inventoryID;
#if UNITY_EDITOR
    [ReadOnly]
#endif
    [SerializeField] public Inventory linkedInventory;
    [SerializeField] public InventoryPannel otherPannel;

    [Header("Transferring")]
    [SerializeField] public bool sendingAllowed = true;
    [SerializeField] public bool receivingAllowed = true;
    [SerializeField] public bool removeSpaces = false;

    [Header("UI")]
    public bool m_autoLink = true;
    public bool m_showPrice = false;
    public bool m_showEmpty = false;
    public List<ItemDisplay> m_itemDisplayInstances = new List<ItemDisplay>();
    public GameObject m_contentPanel;

    public GameObject m_itemDisplayPrefab;

    [System.Serializable]
    public class SortButton
    {
        public Button button;
        public string sortType;
    }

    public List<SortButton> m_sortButtons = new List<SortButton>();
    public SortButton m_selectedSortButton = null;

    public Sprite m_selectedSprite;
    public Sprite m_unselectedSprite;

    

    [Header("Double CLick")]
    public float m_clickTime = 0.5f;
    public float m_clickTimer = 0f;


    // Start is called before the first frame update
    public virtual void Start()
    {
        // get inventory
        linkedInventory = InventoryManager.instance?.GetInventory(inventoryID);

        // if cells empty, get them
        // if (m_itemDisplayInstances.Count == 0)
        // {
        //     m_itemDisplayInstances = GetComponentsInChildren<InventoryCell>().ToList();
        // }

        UpdateItemDisplays();
    }

    protected virtual void OnEnable() {
        // bind sort buttons
        foreach (var button in m_sortButtons)
        {
            button.button.onClick.AddListener(() => SortButtonClicked(button));
        }
    }

    protected virtual void OnDisable() {
        // unbind sort buttons
        foreach (var button in m_sortButtons)
        {
            button.button.onClick.RemoveAllListeners();
        }
    }

    protected virtual void SortButtonClicked(SortButton button)
    {
        if (m_selectedSortButton != null && m_selectedSortButton.button != null)
        {
            // update sprite
            m_selectedSortButton.button.image.sprite = m_unselectedSprite;
        }

        if (m_selectedSortButton == button){
            m_selectedSortButton = null;
        }
        else{
            m_selectedSortButton = button;
            // update sprite
            m_selectedSortButton.button.image.sprite = m_selectedSprite;
        }

        UpdateItemDisplays();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        // every 20 frames, try remove empty spaces
        if (Time.frameCount % 20 == 0)
        {
            if (removeSpaces)
            {
                TryRemoveSpaces(linkedInventory);
            }
        }

        // every 5 frames, update the UI (after removing spaces)
        if (Time.frameCount % 5 == 0)
        {
            UpdateItemDisplays();
        }
    }

    /// <summary>
    /// Updates all the cells in the UI
    /// </summary>
    public virtual void UpdateUI()
    {
        // update each cell
        // foreach (InventoryCell cell in m_itemDisplayInstances)
        // {
        //     cell.UpdateUI();
        // }
    }

    /// <summary>
    /// Links the item Displays to the linked inventory.
    /// </summary>
    protected virtual void UpdateItemDisplays()
    {
        // auto link the item displays
        if (m_autoLink){
            ReLinkItemDisplays();
        }

        // update the item displays
        foreach (ItemDisplay itemDisplay in m_itemDisplayInstances)
        {
            itemDisplay.UpdateLinkedValues();
        }
    }

    /// <summary>
    /// Creates/Removes item displays as needed, and links them to the linked inventory.
    /// </summary>
    private void ReLinkItemDisplays()
    {
        // clear the linked ID of each item display
        foreach (ItemDisplay itemDisplay in m_itemDisplayInstances)
        {
            itemDisplay.m_linkedInventoryID = "";
        }

        List<Inventory.InventorySlot> slots = new List<Inventory.InventorySlot>(linkedInventory.slots);

        // if selected sort button, filter the slots
        if (m_selectedSortButton != null && m_selectedSortButton.button != null)
        {
            for (int i = slots.Count - 1; i >= 0; i--)
            {
                Item item = slots[i].item;
                if (item == null)
                {
                    slots.RemoveAt(i);
                    continue;
                }

                // only keep those that matche the sort type
                if (item.GetType().Name.ToLower().Contains(m_selectedSortButton.sortType.ToLower()))
                {
                    continue;
                }
                else {
                    slots.RemoveAt(i);
                }
            }
        }

        for (int i = 0; i < slots.Count(); i++)
        {
            Inventory.InventorySlot slot = slots[i];
            if (!m_showEmpty && slot.item == null) continue;
            // get the item display at i if it exists, otherwise create a new one
            ItemDisplay itemDisplay = null;
            if (i < m_itemDisplayInstances.Count)
            {
                itemDisplay = m_itemDisplayInstances[i];
            }
            else
            {
                itemDisplay = Instantiate(m_itemDisplayPrefab, m_contentPanel.transform).GetComponent<ItemDisplay>();
                m_itemDisplayInstances.Add(itemDisplay);
            }

            itemDisplay.GetComponent<ItemDisplay>().m_linkedInventoryID = linkedInventory.id;
            itemDisplay.GetComponent<ItemDisplay>().m_slotNumber = linkedInventory.GetItemIndex(slot.item);
            itemDisplay.GetComponent<ItemDisplay>().m_showPrice = m_showPrice;
        }

        // if item display doesnt have a linked inventory, remove it
        for (int i = m_itemDisplayInstances.Count - 1; i >= 0; i--)
        {
            if (m_itemDisplayInstances[i].m_linkedInventoryID == "")
            {
                Destroy(m_itemDisplayInstances[i].gameObject);
                m_itemDisplayInstances.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Called when an item is clicked in the inventory.
    /// </summary>
    /// <param name="_itemDisplay"></param>
    public virtual void ItemClicked(ItemDisplay _itemDisplay)
    {
        if (_itemDisplay == null) return;

        if (!sendingAllowed) return;

        Inventory sourceInventory = _itemDisplay.m_linkedInventory;
        if (sourceInventory == null) return;

        //get index of slot in source inventory
        int sourceIndex = _itemDisplay.m_slotNumber;
        if (sourceIndex == -1) return;

        Inventory targetInventory = otherPannel?.linkedInventory;
        if (targetInventory == null) return;

        if (!otherPannel.receivingAllowed) return;

        PerformClickAction(_itemDisplay, sourceInventory, targetInventory, sourceIndex);

        if (removeSpaces)
        {
            TryRemoveSpaces(sourceInventory);
        }

        // auto link the item displays
        if (m_autoLink){
            ReLinkItemDisplays();
        }
    }

    /// <summary>
    /// What should happen when an item is clicked
    /// </summary>
    /// <param name="_itemDisplay"></param>
    /// <param name="_sourceInventory"></param>
    /// <param name="_targetInventory"></param>
    /// <param name="_sourceIndex"></param>
    protected virtual void PerformClickAction(ItemDisplay _itemDisplay, Inventory _sourceInventory, Inventory _targetInventory, int _sourceIndex)
    {
        if (InventoryManager.instance.TryMoveItem(_sourceInventory, _targetInventory, _sourceIndex)){
            // sound
            UIAudioManager.instance?.equipSound.Play();
        }
    }

    /// <summary>
    /// Tries to remove empty spaces in the inventory.
    /// </summary>
    /// <param name="_sourceInventory"></param>
    protected virtual void TryRemoveSpaces(Inventory _sourceInventory)
    {
        // get inventory, and a list of slots in it
        Inventory inventory = InventoryManager.instance?.GetInventory(inventoryID);
        List<Inventory.InventorySlot> slots = new List<Inventory.InventorySlot>();
        if (inventory != null)
        {
            slots = new List<Inventory.InventorySlot>(inventory.slots);
        }

        // loop through each slot in the inventory, try move the item to an earlier slot
        foreach (Inventory.InventorySlot slot in slots)
        {
            Item item = slot.item;
            if (item == null) continue;

            //get index of slot in source inventory
            int sourceIndex = _sourceInventory.GetItemIndex(item);
            if (sourceIndex == -1) return;

            _sourceInventory.RemoveItemFromInventory(sourceIndex);

            Item leftover = _sourceInventory.TryAddItemToInventory(item);
        }

        UpdateItemDisplays();
    }
}
