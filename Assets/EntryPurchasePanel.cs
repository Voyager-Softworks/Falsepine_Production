using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// This pannel manages the purchasing of entries from the saloon for the journal.
/// </summary>
public class EntryPurchasePanel : MonoBehaviour
{
    [System.Serializable]
    public class EntryUIPurchaseUI{
        public TextMeshProUGUI m_description;
        public TextMeshProUGUI m_price;
        public Button m_purchaseButton;
        public Image m_lockedIcon;
        public TextMeshProUGUI m_lockedText;
    }

    [Header("UI")]
    public List<EntryUIPurchaseUI> m_entryUIs = new List<EntryUIPurchaseUI>();
    public Sprite m_lockedSprite;
    public Sprite m_unlockedSprite;

    [Header("Entries")]
    public int m_price = 10;
    public int m_entryAmount = 3;
    public List<JounralEntry> m_journalEntries = new List<JounralEntry>();

    // Start is called before the first frame update
    void Start()
    {
        // get all valid entires
        List<JounralEntry> validEntries = JournalManager.instance.GetUndiscoveredEntries(_monsterType: MonsterInfo.MonsterType.Minion);

        // add amount of entries to the list (if there are enough)
        for (int i = 0; i < m_entryAmount && i < m_entryUIs.Count && validEntries.Count > 0; i++){
            int randomIndex = Random.Range(0, validEntries.Count);
            // add random entry to the list
            m_journalEntries.Add(validEntries[randomIndex]);
            // remove entry from the list
            validEntries.RemoveAt(randomIndex);
        }

        // link all buttons to the purchase function
        for (int i = 0; i < m_entryUIs.Count; i++){
            int index = i;
            m_entryUIs[i].m_purchaseButton.onClick.AddListener(() =>
            {
                TryBuyEntry(index);
            });
        }
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    /// <summary>
    /// Updates the UI element to show the clues.
    /// </summary>
    private void UpdateUI()
    {
        // for each journalEntry, update the corresponding UI
        for (int i = 0; i < m_entryUIs.Count; i++)
        {
            // get the UI
            EntryUIPurchaseUI entryUI = m_entryUIs[i];

            if (i > m_journalEntries.Count - 1){
                entryUI.m_description.text = "No purchase available";
                entryUI.m_purchaseButton.gameObject.SetActive(false);
                entryUI.m_lockedIcon.sprite = m_lockedSprite;
                entryUI.m_lockedText.text = "CLUE NOT AVALIABLE";

                continue;
            }

            // get the entry
            JounralEntry entry = m_journalEntries[i];

            // check if this clue is already discovered (aka purchased)
            bool beenDiscovered = JournalManager.instance.m_discoveredEntries.Contains(entry);

            if (beenDiscovered)
            {
                entryUI.m_description.text = "Check the journal for more details \n" + entry.entryContent.text;
                entryUI.m_purchaseButton.gameObject.SetActive(false);
                entryUI.m_lockedIcon.sprite = m_unlockedSprite;
                entryUI.m_lockedText.text = "CLUE DISCOVERED";
            }
            else
            {
                entryUI.m_description.text = entry.m_linkedMonster.m_name + " lore";
                entryUI.m_price.text = m_price.ToString();
                entryUI.m_purchaseButton.gameObject.SetActive(true);
                entryUI.m_lockedIcon.sprite = m_lockedSprite;
                entryUI.m_lockedText.text = "CLUE LOCKED";
            }
        }
    }

    /// <summary>
    /// Tries to purchase an entry, if so, discovers it in the journal.
    /// </summary>
    /// <param name="entryIndex"></param>
    public void TryBuyEntry(int entryIndex){
        JounralEntry entry = m_journalEntries[entryIndex];
        if (entry == null) return;

        if (EconomyManager.instance.CanAfford(m_price)){
            EconomyManager.instance.Spend(m_price);
            JournalManager.instance.DiscoverEntry(entry);
            UpdateUI();
        }
    }
}
