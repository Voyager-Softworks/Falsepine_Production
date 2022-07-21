using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

public class JournalPickupLimiter : MonoBehaviour  /// @todo Comment
{
    public int loreAmount = 1;
    public int clueAmount = 1;

    public List<JournalPickupInteract> enabledLore = new List<JournalPickupInteract>(); 
    public List<JournalPickupInteract> enabledClues = new List<JournalPickupInteract>();
    public List<JournalPickupInteract> all = new List<JournalPickupInteract>();

    int frameCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //limit how often
        frameCount++;
        if (frameCount < 120)
        {
            return;
        }

        // if (frameCount > 600 && enabledClues.Count == clueAmount && enabledLore.Count == loreAmount)
        // {
        //     return;
        // }

        // find all pickups in the scene
        all = FindObjectsOfType<JournalPickupInteract>(false).ToList();

        enabledLore = all.Where(x => x.pickupType == JournalManager.InfoType.Lore).ToList();
        enabledClues = all.Where(x => x.pickupType == JournalManager.InfoType.Clue).ToList();
        
        // randomly disable some lore until we have the correct amount
        if (enabledLore.Count > loreAmount)
        {
            int index = UnityEngine.Random.Range(0, enabledLore.Count);
            enabledLore[index].gameObject.SetActive(false);
        }

        // randomly disable some clues until we have the correct amount
        if (enabledClues.Count > clueAmount)
        {
            int index = UnityEngine.Random.Range(0, enabledClues.Count);
            enabledClues[index].gameObject.SetActive(false);
        }
    }
}
