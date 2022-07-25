using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[Serializable]
public class InventoryGridItem : MonoBehaviour  /// @todo Comment
{
    public Image icon;
    public Image modIcon;
    public TextMeshProUGUI text;
    public Button button;

    public Inventory.InventorySlot linkedSlot;
    public Item itemInSlot {
        get {
            return linkedSlot?.item;
        }
    }

    public UnityEvent OnClick;

    private Camera uiCamera = null;
    private InfoBox ib = null;

    private void Start() {
        // find UI camera.
        // itterate through parents and check if they have a child with camera component.
        // if so, set it as the ui camera.
        Transform parent = transform.parent;
        while (parent != null) {
            if (parent.GetComponentInChildren<Camera>() != null) {
                uiCamera = parent.GetComponentInChildren<Camera>();
                break;
            }
            parent = parent.parent;
        }

        // find info box.
        ib = FindObjectOfType<InfoBox>();
    }

    private void Update() {
        // check if mouse is over this grid item
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        // get corners
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
        Vector2 bottomLeft = uiCamera.WorldToScreenPoint(corners[0]);
        Vector2 topLeft = uiCamera.WorldToScreenPoint(corners[1]);
        Vector2 topRight = uiCamera.WorldToScreenPoint(corners[2]);
        Vector2 bottomRight = uiCamera.WorldToScreenPoint(corners[3]);

        // check if mouse is within the grid item's screen space
        if (mouseScreenPos.x >= topLeft.x && mouseScreenPos.x <= topRight.x &&
            mouseScreenPos.y <= topLeft.y && mouseScreenPos.y >= bottomLeft.y)
        {
            // display info box
            if (ib) ib.Display(itemInSlot, 0.1f, 0.1f);
        }
    }

    public void UpdateUI(){
        //check
        //if (itemInSlot == null) return;

        // update visuals
        icon.sprite = itemInSlot?.m_icon;
        text.text = itemInSlot?.m_displayName;

        if (itemInSlot?.GetStatMods().Count > 0) {
            modIcon.gameObject.SetActive(true);
        } else {
            modIcon.gameObject.SetActive(false);
        }
    }

    public void ClickItem(){
        //TryTransferToOpenInventory();

        InventoryPannel parentPannel = GetComponentInParent<InventoryPannel>();
        if (parentPannel == null) return;
        parentPannel.ItemClicked(this);

        OnClick.Invoke();
    }

    public void TryTransferToOpenInventory(){
        if (linkedSlot == null) return;
        if (itemInSlot == null) return;

        Inventory sourceInventory = linkedSlot.ownerInventory;
        if (sourceInventory == null) return;

        InventoryPannel parentPannel = GetComponentInParent<InventoryPannel>();
        if (parentPannel == null) return;

        InventoryPannel targetPannel = parentPannel.otherPannel;
        if (targetPannel == null) return;

        Inventory targetInventory = targetPannel.linkedInventory;
        if (targetInventory == null) return;

        //get index of slot in source inventory
        int sourceIndex = sourceInventory.GetItemIndex(itemInSlot);
        if (sourceIndex == -1) return;

        //transfer
        InventoryManager.instance.TryMoveItem(sourceInventory, targetInventory, sourceIndex);
    }
}
