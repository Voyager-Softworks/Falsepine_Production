using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BankUI : MonoBehaviour
{
    public TextMeshProUGUI m_bankLevelText;
    public TextMeshProUGUI m_percentageText;
    public Button m_upgradeButton;
    public TextMeshProUGUI m_upgradeText;
    public TextMeshProUGUI m_upgradeCostText;
    public Image m_silverImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable() {
        BindButtons();
    }

    private void OnDisable() {
        UnbindButtons();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        m_bankLevelText.text = EconomyManager.instance.m_bankLevel + "/10";

        m_percentageText.text = (EconomyManager.instance.GetSilverRetainPercentage() * 100.0f).ToString("0") + "% gold saved on death";

        if (EconomyManager.instance.m_bankLevel >= EconomyManager.instance.m_maxBankLevel)
        {
            m_upgradeButton.interactable = false;
            // update text
            m_upgradeText.text = "MAX LEVEL";
            // disable cost text and image
            m_upgradeCostText.gameObject.SetActive(false);
            m_silverImage.gameObject.SetActive(false);
        }
        else
        {
            m_upgradeButton.interactable = true;
            // update text
            m_upgradeText.text = "UPGRADE";
            m_upgradeCostText.text = EconomyManager.instance.m_bankUpgradeCost.ToString();
            // enable cost text and image
            m_upgradeCostText.gameObject.SetActive(true);
            m_silverImage.gameObject.SetActive(true);
        }
    }

    private void BindButtons()
    {
        m_upgradeButton.onClick.AddListener(UpgradeButtonPressed);
    }

    private void UnbindButtons()
    {
        m_upgradeButton.onClick.RemoveAllListeners();
    }

    public void UpgradeButtonPressed()
    {
        if (EconomyManager.instance.TryUpgradeBank()){
            UIAudioManager.instance?.buySound.Play();
        }
        else{
            UIAudioManager.instance?.errorSound.Play();
        }
    }
}
