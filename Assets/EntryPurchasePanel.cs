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
    [Header("UI")]
    public GameObject m_cluePurchaseUIPrefab;
    public Transform m_contentParent;
    public List<CluePurchaseUI> m_clueUIs = new List<CluePurchaseUI>();
    public CluePurchaseUI m_selectedClue;
    public Button m_purchaseButton;
    public TextMeshProUGUI m_purchaseButtonText;
    public TextMeshProUGUI m_purchaseButtonCostText;
    public Image m_purchaseButtonSilverIcon;

    [Header("Entries")]
    public int m_minPrice = 10;
    public int m_maxPrice = 20;
    public int m_entryAmount = 3;
    public List<JounralEntry> m_journalEntries = new List<JounralEntry>();

    // Start is called before the first frame update
    void Start()
    {
        // get all valid entires
        List<JounralEntry> validEntries = JournalManager.instance.GetUndiscoveredEntries(_monsterType: MonsterInfo.MonsterType.Minion);

        // add amount of entries to the list (if there are enough)
        for (int i = 0; i < m_entryAmount && validEntries.Count > 0; i++){
            int randomIndex = Random.Range(0, validEntries.Count);
            JounralEntry entry = validEntries[randomIndex];
            // add random entry to the list
            m_journalEntries.Add(entry);
            // remove entry from the list
            validEntries.RemoveAt(randomIndex);

            // create clue ui
            CluePurchaseUI clueUI = Instantiate(m_cluePurchaseUIPrefab, m_contentParent).GetComponent<CluePurchaseUI>();
            clueUI.m_linkedJournalEntry = entry;
            clueUI.m_primaryText.text = entry.m_entryType.ToString();

            // assign price
            clueUI.m_price = Random.Range(m_minPrice, m_maxPrice);

            // add clue ui to the list
            m_clueUIs.Add(clueUI);

            // listen for click
            clueUI.OnClick += () => {
                // deselect all
                foreach (CluePurchaseUI ui in m_clueUIs){
                    ui.m_isSelected = false;
                }

                // select this one
                clueUI.m_isSelected = true;
                m_selectedClue = clueUI;
            };
        }
    }

    private void OnEnable()
    {
        // bind purchase button
        m_purchaseButton.onClick.AddListener(TryBuySelected);

        
        UpdateUI();
    }

    private void OnDisable() {
        // unbind purchase button
        m_purchaseButton.onClick.RemoveListener(TryBuySelected);
    }

    private void Update() {
        UpdateUI();
    }

    /// <summary>
    /// Updates the UI element to show the clues.
    /// </summary>
    private void UpdateUI()
    {
        // for each journalEntry, update the UI
        for (int i = 0; i < m_clueUIs.Count; i++)
        {
            CluePurchaseUI clueUI = m_clueUIs[i];
            clueUI.UpdateUI();
        }

        if (m_selectedClue != null){
            // say Purchase
            m_purchaseButtonText.text = "PURCHASE";

            // set the price on the button
            m_purchaseButtonCostText.text = m_selectedClue.m_price.ToString();

            // enable the silver icon
            m_purchaseButtonSilverIcon.gameObject.SetActive(true);
        }
        else{
            // say Select Entry
            m_purchaseButtonText.text = "SELECT ENTRY";

            // set the price
            m_purchaseButtonCostText.text = "0";

            // disable the silver icon
            m_purchaseButtonSilverIcon.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Tries to purchase an entry, if so, discovers it in the journal.
    /// </summary>
    /// <param name="entryIndex"></param>
    public void TryBuySelected(){
        if (m_selectedClue == null) return;
        if (m_selectedClue.m_linkedJournalEntry == null) return;

        if (EconomyManager.instance.CanAfford(m_selectedClue.m_price)){
            EconomyManager.instance.SpendMoney(m_selectedClue.m_price);
            JournalManager.instance.DiscoverEntry(m_selectedClue.m_linkedJournalEntry);
            UpdateUI();
        }
    }
}
