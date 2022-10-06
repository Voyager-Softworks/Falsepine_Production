using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalUpdater_Talismans : JournalContentUpdater
{
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

        // add new content
        foreach (StatsManager.Talisman talisman in talismans)
        {
            JournalContent content = new JournalContent();
            content.text = talisman.m_statMod.ToText();
            content.image = talisman.m_icon;
            contentList.Add(content);
        }

        // get all drink mods
        List<Drink> drinks = StatsManager.activeDrinks;

        // add new content
        foreach (Drink drink in drinks)
        {
            JournalContent content = new JournalContent();
            content.text = drink.m_displayName.ToString();
            foreach (StatsManager.StatMod mod in drink.GetStatMods()){
                content.text += "\n" + mod.ToText();
            }
            content.image = drink.m_icon;
            contentList.Add(content);
        }

        base.UpdateContent();
    }
}
