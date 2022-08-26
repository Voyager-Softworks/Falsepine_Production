using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// A class that will replace the "GridItem" and "InventoryCell" classes, which will display the items in the inventory.
/// @todo finish this class
/// </summary>
public class ItemDisplay : MonoBehaviour
{
    [Header("Inventory")]
    public string m_linkedInventoryID = "";
    public int m_slotNumber = 0;

    #if UNITY_EDITOR
    [ReadOnly]
    #endif
    public Inventory m_linkedInventory;

    #if UNITY_EDITOR
    [ReadOnly]
    #endif
    public Item m_linkedItem = null;

    [Header("UI")]
    public bool m_showPrice = true;

    public Image m_backgroundImage;
    public Image m_itemIcon;

    [Header("UI - Footer")]
    public GameObject m_footer;
    public Image m_modifierIcon;
    public TextMeshProUGUI m_itemName;

    [Header("UI - Cost")]
    public GameObject m_costPanel;
    public Image m_costIcon;
    public TextMeshProUGUI m_costAmount;

    [Header("UI - Type")]
    public GameObject m_typePanel;
    public Image m_typeIcon;
    public TextMeshProUGUI m_typeName;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if button is hovered over, show the footer
        Button b = GetComponentInChildren<Button>();
        if (b != null)
        {
            if (b.navigation.())
            {
                m_footer.SetActive(true);
            }
            else
            {
                m_footer.SetActive(false);
            }
        }
    }

    private void OnEnable() {
        // bind the button to the click event
        RebindButton();
    }

    private void OnDisable() {
        // unbind the button from the click event
        GetComponentInChildren<Button>().onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Removes, then adds a listener to the button to call the ClickItem method.
    /// </summary>
    public void RebindButton() {
        // unbind the button from the click event
        GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        // rebind the button to the click event
        GetComponentInChildren<Button>().onClick.AddListener(() => ClickItem());
    }

    /// <summary>
    /// Updates the linked values (inventory and item) from the ID and slot number.
    /// </summary>
    public void UpdateLinkedValues(){
        m_linkedInventory = InventoryManager.instance.GetInventory(m_linkedInventoryID);
        if (m_linkedInventory == null) return;

        Inventory.InventorySlot slot = m_linkedInventory.GetSlot(m_slotNumber);
        m_linkedItem = slot?.item;

        UpdateUI();
    }

    /// <summary>
    /// Updates the visual appearance of the item display.
    /// </summary>
    public void UpdateUI(){
        //if none, say so
        if (m_linkedItem == null) {
            m_itemIcon.enabled = false;
            m_modifierIcon.enabled = false;
            m_itemName.text = "Empty";
            m_typePanel.SetActive(false);
            m_costPanel.SetActive(false);

            return;
        }

        // otherwise update with relevant values
        m_itemIcon.enabled = true;
        m_itemIcon.sprite = m_linkedItem.m_icon;
        m_itemName.text = m_linkedItem.m_displayName;
        if (m_linkedItem.GetStatMods().Count > 0) {
            m_modifierIcon.enabled = true;
        } else {
            m_modifierIcon.enabled = false;
        }
        if (m_showPrice) {
            m_typePanel.SetActive(false);
            m_costPanel.SetActive(true);
            m_costAmount.text = m_linkedItem.m_price.ToString();
        } else {
            m_typePanel.SetActive(true);
            m_costPanel.SetActive(false);
            System.Type type = m_linkedItem.GetType();
            bool isWeapon = type.IsSubclassOf(typeof(RangedWeapon));
            Debug.Log("isWeapon: " + isWeapon);
            if (isWeapon) { m_typeName.text = "Ranged"; }
            else if (m_linkedItem.GetType().IsSubclassOf(typeof(Equipment))) { m_typeName.text = "Tool"; }
            else m_typeName.text = "Item";
        }
    }

    /// <summary>
    /// Called when this item is clicked
    /// </summary>
    public void ClickItem()
    {
        //TryTransferToOpenInventory();

        InventoryPannel parentPannel = GetComponentInParent<InventoryPannel>();
        if (parentPannel == null) return;
        parentPannel.ItemClicked(this);
    }

    // custom editor
    #if UNITY_EDITOR
    [CustomEditor(typeof(ItemDisplay))]
    public class ItemDisplayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ItemDisplay script = (ItemDisplay)target;
            if (GUILayout.Button("Try Link Slot"))
            {
                script.UpdateLinkedValues();
            }
        }
    }
    #endif
}
