using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CampUIManager : MonoBehaviour
{
    public Sprite m_selectedSprite;
    public Sprite m_unselectedSprite;

    [Header("UI Elements")]
    public Button m_loadoutButton;
    public Button m_missionsButton;
    public GameObject m_loadoutPanel;
    public GameObject m_missionsPanel;

    // Start is called before the first frame update
    void Start()
    {
        LoadoutButtonPressed();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable() {
        BindButtons();
    }

    private void OnDisable() {
        UnbindButtons();
    }

    public void BindButtons()
    {
        m_loadoutButton.onClick.AddListener(LoadoutButtonPressed);
        m_missionsButton.onClick.AddListener(MissionsButtonPressed);
    }

    public void UnbindButtons()
    {
        m_loadoutButton.onClick.RemoveAllListeners();
        m_missionsButton.onClick.RemoveAllListeners();
    }

    public void LoadoutButtonPressed()
    {
        m_loadoutPanel.SetActive(true);
        m_missionsPanel.SetActive(false);
        m_loadoutButton.image.sprite = m_selectedSprite;
        m_missionsButton.image.sprite = m_unselectedSprite;
    }

    public void MissionsButtonPressed()
    {
        m_loadoutPanel.SetActive(false);
        m_missionsPanel.SetActive(true);
        m_loadoutButton.image.sprite = m_unselectedSprite;
        m_missionsButton.image.sprite = m_selectedSprite;
    }
}
