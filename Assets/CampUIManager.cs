using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CampUIManager : MonoBehaviour
{
    public Sprite m_selectedSprite;
    public Sprite m_unselectedSprite;

    [Header("UI Elements")]
    [Header("Tabs")]
    public Button m_loadoutButton;
    public Button m_missionsButton;
    public GameObject m_loadoutPanel;
    public GameObject m_missionsPanel;
    [Header("Embark")]
    public Button m_embarkButton;
    public Utilities.SceneField m_finalScene;

    // Start is called before the first frame update
    void Start()
    {
        OpenLoadout();
    }

    // Update is called once per frame
    void Update()
    {
        // if scene is last m_finalScene, disable embark button
        if (m_finalScene.Equals(SceneManager.GetActiveScene()))
        {
            m_embarkButton.interactable = false;
        }
        else
        {
            m_embarkButton.interactable = true;
        }
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
        m_embarkButton.onClick.AddListener(EmbarkButtonPressed);
    }

    public void UnbindButtons()
    {
        m_loadoutButton.onClick.RemoveAllListeners();
        m_missionsButton.onClick.RemoveAllListeners();
        m_embarkButton.onClick.RemoveAllListeners();
    }

    public void LoadoutButtonPressed()
    {
        OpenLoadout();
        UIAudioManager.instance?.buttonSound.Play();
    }

    private void OpenLoadout()
    {
        m_loadoutPanel.SetActive(true);
        m_missionsPanel.SetActive(false);
        m_loadoutButton.image.sprite = m_selectedSprite;
        m_missionsButton.image.sprite = m_unselectedSprite;
    }

    public void MissionsButtonPressed()
    {
        OpenMissions();
        UIAudioManager.instance?.buttonSound.Play();
    }

    private void OpenMissions()
    {
        m_loadoutPanel.SetActive(false);
        m_missionsPanel.SetActive(true);
        m_loadoutButton.image.sprite = m_unselectedSprite;
        m_missionsButton.image.sprite = m_selectedSprite;
    }

    public void EmbarkButtonPressed()
    {
        //load first level
        if (MissionManager.instance != null)
        {
            MissionManager.instance.TryEmbark();
        }
    }
}
