using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoTypeEquipment : MonoBehaviour, Equipment.Useable
{
    public bool TryUse(Item _item) {
        // find player inventory interface
        PlayerInventoryInterface inventoryInterface = FindObjectOfType<PlayerInventoryInterface>();

        // get current selected weapon
        Item weapon = inventoryInterface.selectedWeapon;

        // cast as RangedWeapon
        RangedWeapon rangedWeapon = weapon as RangedWeapon;

        // fill the ammo and set temp ammo stats
        if (rangedWeapon != null){
            rangedWeapon.m_clipAmmo = rangedWeapon.m_clipSize;
            
            // set temp ammo stats
            rangedWeapon.m_tempAmmoStats = new List<StatsManager.StatType>(_item.GetStatTypes());

            return true;
        }

        return false;
    }
}
