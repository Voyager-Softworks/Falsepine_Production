using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPickup : Interactable
{
    [Header("Money Pickup")]
    public int moneyAmount = 10;

    public override void DoInteract()
    {
        base.DoInteract();

        EconomyManager.instance?.AddMoney(moneyAmount);
    }
}
