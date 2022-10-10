using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZoneNameUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UpdateText();
    }

    private void OnEnable() {
        UpdateText();
    }

    public void UpdateText(){
        if (MissionManager.instance == null) return;
        if (MissionManager.instance.GetCurrentZone() == null) return;

        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        if (text == null) return;

        text.text = MissionManager.instance.GetCurrentZone().m_title;
    }
}
