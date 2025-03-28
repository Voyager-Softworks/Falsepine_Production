using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CluePurchaseUI : MonoBehaviour
{
    [Header("References")]
    public Button m_button;
    public Image m_background;
    public GameObject m_lockedIcon;
    public GameObject m_unlockedIcon;
    public Button m_jounralButton;
    public TextMeshProUGUI m_primaryText;
    public TextMeshProUGUI m_secondaryText;
    public GameObject m_silverIcon;

    [Header("Journal Entry")]
    [ReadOnly] public JounralEntry m_linkedJournalEntry;
    [ReadOnly] public int m_price = 10;

    [Header("Assets")]
    public Sprite m_selectedSprite;
    public Sprite m_unselectedSprite;


    public System.Action OnClick;
    public bool m_isSelected = false;


    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable() {
        // bind button to the click function
        m_button.onClick.AddListener(() =>
        {
            OnClick?.Invoke();
        });

        // bind journal button to JournalButtonPressed
        m_jounralButton.onClick.AddListener(JournalButtonPressed);
    }

    private void OnDisable() {
        // remove all listeners
        m_button.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI()
    {
        // set background if m_isSelected
        m_background.sprite = m_isSelected ? m_selectedSprite : m_unselectedSprite;

        // check if this clue is already discovered (aka purchased)
        bool beenDiscovered = JournalManager.instance.m_discoveredEntries.Contains(m_linkedJournalEntry);

        // show correct icons
        m_unlockedIcon.SetActive(true);
        //m_lockedIcon.SetActive(!beenDiscovered);
        // if image exists, set image to monster image
        Sprite monsterImage = m_linkedJournalEntry?.m_linkedMonster?.m_monsterImage;
        if (monsterImage != null)
        {
            Image[] images = m_unlockedIcon.GetComponentsInChildren<Image>();
            foreach (Image image in images)
            {
                image.sprite = monsterImage;
            }
        }

        // show correct text and silver icon (if needed)
        m_primaryText.text = m_linkedJournalEntry.m_linkedMonster.m_name + " " + m_linkedJournalEntry.m_entryType.ToString();
        m_secondaryText.text = beenDiscovered ? "Unlocked" : m_price.ToString();
        m_silverIcon.SetActive(!beenDiscovered);


        // journal button button only if the clue has been discovered
        m_jounralButton.gameObject.SetActive(beenDiscovered);
    }

    public void JournalButtonPressed(){
        // open to entry
        JournalManager.instance?.OpenToEntry(m_linkedJournalEntry);
    }
}
