using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaloonUIManager : MonoBehaviour
{
    public Sprite m_selectedSprite;
    public Sprite m_unselectedSprite;

    [Header("UI Elements")]
    [Header("Tabs")]
    public Button m_drinksButton;
    public Button m_cluesButton;
    public GameObject m_drinksPanel;
    public GameObject m_cluesPanel;

    // Start is called before the first frame update
    void Start()
    {
        OpenDrinks();
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
        m_drinksButton.onClick.AddListener(DrinksButtonPressed);
        m_cluesButton.onClick.AddListener(CluesButtonPressed);
    }

    public void UnbindButtons()
    {
        m_drinksButton.onClick.RemoveAllListeners();
        m_cluesButton.onClick.RemoveAllListeners();
    }

    public void DrinksButtonPressed()
    {
        OpenDrinks();
        UIAudioManager.instance?.buttonSound.Play();
    }

    private void OpenDrinks()
    {
        m_drinksPanel.SetActive(true);
        m_cluesPanel.SetActive(false);
        m_drinksButton.image.sprite = m_selectedSprite;
        m_cluesButton.image.sprite = m_unselectedSprite;
    }

    public void CluesButtonPressed()
    {
        OpenClues();
        UIAudioManager.instance?.buttonSound.Play();
    }

    private void OpenClues()
    {
        m_drinksPanel.SetActive(false);
        m_cluesPanel.SetActive(true);
        m_drinksButton.image.sprite = m_unselectedSprite;
        m_cluesButton.image.sprite = m_selectedSprite;
    }
}
