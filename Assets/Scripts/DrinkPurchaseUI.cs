using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DrinkPurchaseUI : MonoBehaviour
{
    [Header("References")]
    public Button m_button;
    public Image m_background;
    public Image m_icon;
    public TextMeshProUGUI m_primaryText;
    public TextMeshProUGUI m_secondaryText;
    public GameObject m_silverIcon;

    [Header("Journal Entry")]
    [ReadOnly] public Drink m_linkedDrink;
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
    }

    private void OnDisable() {
        // unbind button to the click function
        m_button.onClick.RemoveListener(() =>
        {
            OnClick?.Invoke();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI()
    {
        // set background if m_isSelected
        m_background.sprite = m_isSelected ? m_selectedSprite : m_unselectedSprite;

        // set icon
        m_icon.sprite = m_linkedDrink.m_icon;

        // check if this drink is already active (aka purchased)
        bool allActive = IsAlreadyActive();

        // show correct text and silver icon (if needed)
        m_primaryText.text = m_linkedDrink.m_displayName.ToString();
        m_secondaryText.text = allActive ? "Active" : m_price.ToString();
        m_silverIcon.SetActive(!allActive);
    }

    public bool IsAlreadyActive()
    {
        if (StatsManager.activeDrinks.Contains(m_linkedDrink))
        {
            return true;
        }

        return false;
    }

    public void JournalButtonPressed(){
        // open the journal
        JournalManager.instance.OpenWindow();
        // select the entry
        //JournalManager.instance.
    }
}
