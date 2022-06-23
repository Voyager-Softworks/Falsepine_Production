using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalPickupInteract : Interactable 
{
    public JournalManager.InfoType pickupType = JournalManager.InfoType.Lore;

    public string monsterName = "";

    public string infoName = "";


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
        if (JournalManager.instance){
            JournalManager.instance.AddInfo(pickupType, monsterName, infoName);
        }

        base.DoInteract();
    }
}
