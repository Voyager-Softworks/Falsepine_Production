using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBoxEquipment : MonoBehaviour, Equipment.Useable
{
    public bool TryUse(Item _item) {
        // find player inventory
        Inventory inventory = InventoryManager.instance.GetInventory("player");
        
        // fill the ammo
        if (inventory != null){
            if (inventory.FillAmmo()){
                return true;
            }
        }

        return false;
    }
}
