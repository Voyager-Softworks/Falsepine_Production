using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BankUI : MonoBehaviour
{
    public TextMeshProUGUI m_bankLevelText;
    public TextMeshProUGUI m_percentageText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_bankLevelText.text = EconomyManager.instance.m_bankLevel + "/10";

        m_percentageText.text = (EconomyManager.instance.GetSilverRetainPercentage() * 100.0f).ToString("0") + "% gold saved on death";
    }
}
