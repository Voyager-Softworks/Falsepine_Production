using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldPanel : MonoBehaviour
{
    public TextMeshProUGUI m_goldText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (EconomyManager.instance == null) return;

        m_goldText.text = EconomyManager.instance.m_playerSilver.ToString();
    }
}