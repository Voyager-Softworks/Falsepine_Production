using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedkitEquipment : MonoBehaviour, Equipment.Useable
{
    public float healAmount = 100f;

    public bool TryUse(Item _item) {
        // find player health
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth.currentHealth < playerHealth.calcedMaxHealth) {
            // heal player
            playerHealth.Heal(healAmount);
            return true;
        }

        return false;
    }
}
