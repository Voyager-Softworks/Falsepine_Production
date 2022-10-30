using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalUpdater_Talismans : JournalContentUpdater
{
    public TextMeshProUGUI talismansText;
    public TextMeshProUGUI drinksText;
    public TextMeshProUGUI statsText;

    public override void UpdateContent()
    {
        // refresh the content list:

        // get JournalManager instance
        JournalManager journalManager = JournalManager.instance;
        if (journalManager == null)
        {
            Debug.LogError("JournalManager instance not found!");
            return;
        }

        // get all talismans
        List<StatsManager.Talisman> talismans = StatsManager.instance.m_activeTalismans;

        // clear current content
        contentList.Clear();

        // Talisman content:
        talismansText.text = "";
        if (talismans.Count > 0)
        {
            talismansText.text += talismans.Count;
        }
        else
        {
            talismansText.text += "No talismans yet!";
        }

        // get all drink mods
        List<Drink> drinks = StatsManager.activeDrinks;

        // Drinks text;
        drinksText.text = "";
        if (drinks.Count > 0)
        {
            int num = 0;
            foreach (Drink drink in drinks)
            {
                drinksText.text += (num > 0 ? ", " : "") + drink.m_displayName;

                num++;
            }
        }
        else
        {
            drinksText.text += "No drinks yet!";
        }

        // Stats:
        // get list of all stat mods
        List<StatsManager.StatMod> allStatMods = new List<StatsManager.StatMod>(StatsManager.GetAllStatMods());

        // combine all stat mods
        List<StatsManager.StatMod> combinedStatMods = new List<StatsManager.StatMod>();
        foreach (StatsManager.StatMod statMod in allStatMods)
        {
            bool found = false;
            foreach (StatsManager.StatMod combinedStatMod in combinedStatMods)
            {
                if (combinedStatMod.statType == statMod.statType &&  combinedStatMod.modType == statMod.modType)
                {
                    // if mod is additive
                    if (combinedStatMod.modType == StatsManager.ModType.Additive)
                    {
                        combinedStatMod.value += statMod.value;
                    }
                    // if mod is multiplier
                    else if (combinedStatMod.modType == StatsManager.ModType.Multiplier)
                    {
                        combinedStatMod.value *= statMod.value;
                    }
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                StatsManager.StatMod newStatMod = new StatsManager.StatMod();
                newStatMod.statType = statMod.statType;
                newStatMod.modType = statMod.modType;
                newStatMod.value = statMod.value;
                combinedStatMods.Add(newStatMod);
            }
        }

        // sort alphabetically by stat type
        combinedStatMods.Sort((x, y) => x.statType.CompareTo(y.statType));

        // add stat mods to content list
        int listNum = 0;
        statsText.text = "";
        foreach (StatsManager.StatMod statMod in combinedStatMods)
        {
            statsText.text += (listNum > 0 ? "\n" : "") + "- " + statMod.ToText();
            listNum++;
        }
        if (listNum == 0)
        {
            statsText.text += "No stat mods!";
        }

        // rebuild content
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());

        base.UpdateContent();
    }
}
