using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DrinkPurchasePanel : MonoBehaviour
{
    [Header("UI")]
    public GameObject m_drinkPurchaseUIPrefab;
    public Transform m_contentParent;
    public List<DrinkPurchaseUI> m_drinkUIs = new List<DrinkPurchaseUI>();
    public DrinkPurchaseUI m_selectedDrink;
    public Button m_purchaseButton;
    public TextMeshProUGUI m_purchaseButtonText;
    public TextMeshProUGUI m_purchaseButtonCostText;
    public Image m_purchaseButtonSilverIcon;

    [Header("Entries")]
    public int m_drinkAmount = 3;
    public List<Drink> m_drinksEntries = new List<Drink>();

    [Header("Double CLick")]
    public float m_clickTime = 0.5f;
    public float m_clickTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // get all valid drinks
        List<Drink> validDrinks = new List<Drink>(EconomyManager.instance.m_purchasableDrinks);

        // add amount of entries to the list (if there are enough)
        for (int i = 0; i < m_drinkAmount && validDrinks.Count > 0; i++){
            //int randomIndex = Random.Range(0, validDrinks.Count);
            int randomIndex = 0;
            Drink drink = validDrinks[randomIndex];
            // add random entry to the list
            m_drinksEntries.Add(drink);
            // remove entry from the list
            validDrinks.RemoveAt(randomIndex);

            // create drink ui
            DrinkPurchaseUI drinkUI = Instantiate(m_drinkPurchaseUIPrefab, m_contentParent).GetComponent<DrinkPurchaseUI>();
            drinkUI.m_linkedDrink = drink;
            drinkUI.m_primaryText.text = drink.m_displayName.ToString();

            // assign price
            drinkUI.m_price = drink.GetPrice();

            // add drink ui to the list
            m_drinkUIs.Add(drinkUI);

            // listen for click
            drinkUI.OnClick += () => {
                // if not selected, play sound
                if (!drinkUI.m_isSelected){
                    // sound
                    UIAudioManager.instance?.buttonSound.Play();
                }

                // deselect all
                foreach (DrinkPurchaseUI ui in m_drinkUIs){
                    ui.m_isSelected = false;
                }

                // if already selected, and timer is not 0, double click
                if (m_selectedDrink == drinkUI && m_clickTimer > 0f){
                    // double click
                    TryBuySelected();
                }
                // start timer
                m_clickTimer = m_clickTime;

                // select this one
                drinkUI.m_isSelected = true;
                m_selectedDrink = drinkUI;
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

        // count down time
        m_clickTimer = Mathf.Max(0f, m_clickTimer - Time.deltaTime);
    }

    /// <summary>
    /// Updates the UI element to show the drinks.
    /// </summary>
    private void UpdateUI()
    {
        // for each journalEntry, update the UI
        for (int i = 0; i < m_drinkUIs.Count; i++)
        {
            DrinkPurchaseUI drinkUI = m_drinkUIs[i];
            drinkUI.UpdateUI();
        }

        if (m_selectedDrink != null){
            bool isActive = m_selectedDrink.IsAlreadyActive();
            // count already active drinks
            int activeDrinks = StatsManager.activeDrinks.Count;
            if (activeDrinks >= 1){
                // if there is already an active drink, disable the button
                m_purchaseButton.interactable = false;
                m_purchaseButtonText.text = "Already Drunk!";

                // set the price on the button
                m_purchaseButtonCostText.transform.gameObject.SetActive(false);

                // disable the silver icon
                m_purchaseButtonSilverIcon.gameObject.SetActive(false);
            }
            else if (!isActive){
                m_purchaseButton.interactable = true;

                // say Purchase
                m_purchaseButtonText.text = "PURCHASE";

                // set the price on the button
                m_purchaseButtonCostText.transform.gameObject.SetActive(true);
                m_purchaseButtonCostText.text = m_selectedDrink.m_price.ToString();

                // enable the silver icon
                m_purchaseButtonSilverIcon.gameObject.SetActive(true);
            }
            else{
                m_purchaseButton.interactable = false;

                // say Active
                m_purchaseButtonText.text = "ACTIVE";

                // set the price on the button
                m_purchaseButtonCostText.transform.gameObject.SetActive(false);

                // disable the silver icon
                m_purchaseButtonSilverIcon.gameObject.SetActive(false);
            }
            
        }
        else{
            m_purchaseButton.interactable = false;

            // say Select Entry
            m_purchaseButtonText.text = "SELECT DRINK";

            // set the price
            m_purchaseButtonCostText.transform.gameObject.SetActive(false);

            // disable the silver icon
            m_purchaseButtonSilverIcon.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Tries to purchase an entry, if so, discovers it in the journal.
    /// </summary>
    /// <param name="entryIndex"></param>
    public void TryBuySelected(){
        if (m_selectedDrink == null) return;
        if (m_selectedDrink.m_linkedDrink == null) return;

        if (m_selectedDrink.IsAlreadyActive()){
            // already active
            return;
        }

        if (EconomyManager.instance.CanAfford(m_selectedDrink.m_price)){
            EconomyManager.instance.SpendMoney(m_selectedDrink.m_price);
            m_selectedDrink.m_linkedDrink.Consume();
            UpdateUI();

            // sound
            UIAudioManager.instance?.buySound.Play();
        }
        else{
            // sound
            UIAudioManager.instance?.errorSound.Play();
        }
    }
}
