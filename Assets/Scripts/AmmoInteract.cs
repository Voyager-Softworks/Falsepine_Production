using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fills the player's ammo completely.
/// </summary>
public class AmmoInteract : Interactable
{
    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }

    override public void DoInteract()
    {
        base.DoInteract();

        // rey refill the player ammo
        Inventory playerInv = InventoryManager.instance?.GetInventory("player");
        if (playerInv != null)
        {
            playerInv.FillAmmo();
        }
    }
}
