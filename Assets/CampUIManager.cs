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
    
    [Header("Unlock")]
    public Button unlockButton;
    public GameObject iconLocked;
    public GameObject iconUnlocked;


    // Start is called before the first frame update
    void Start()
    {
        OpenLoadout();
    }

    // Update is called once per frame
    void Update()
    {
        // if scene is last m_finalScene, disable embark button
        if (m_finalScene.EqualsPath(SceneManager.GetActiveScene()))
        {
            m_embarkButton.interactable = false;
        }
        else
        {
            m_embarkButton.interactable = true;
        }

        // update unlock button
        bool thisZoneBossKilled = MissionManager.instance.HasZoneBossDied();
        if (!thisZoneBossKilled) {
            iconLocked.SetActive(true);
            iconUnlocked.SetActive(false);
            unlockButton.GetComponentInChildren<Button>().interactable = false;
            unlockButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
            unlockButton.GetComponentInChildren<TextMeshProUGUI>().text = "Kill Boss First";
        }
        else {
            iconUnlocked.SetActive(true);
            iconLocked.SetActive(false);
            unlockButton.GetComponentInChildren<Button>().interactable = true;
            unlockButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            unlockButton.GetComponentInChildren<TextMeshProUGUI>().text = "Go to Next Zone";
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
        unlockButton.onClick.AddListener(UnlockButtonPressed);
    }

    public void UnbindButtons()
    {
        m_loadoutButton.onClick.RemoveAllListeners();
        m_missionsButton.onClick.RemoveAllListeners();
        m_embarkButton.onClick.RemoveAllListeners();
        unlockButton.onClick.RemoveAllListeners();
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
            if (MissionManager.instance.GetCurrentMission() != null && MissionManager.instance.GetCurrentMission().GetState() == MissionCondition.ConditionState.COMPLETE){
                // try return mission
                MissionManager.instance.TryReturnMission();
            }

            EmbarkPopup popup = FindObjectOfType<EmbarkPopup>();
            if (popup != null)
            {
                popup.OpenWindow();
            }
        }
    }

    public void UnlockButtonPressed(){
        if (MissionManager.instance == null) return;
        if (MissionManager.instance.GetCurrentZone() == null) return;
        if (MissionManager.instance.GetCurrentZone().m_zoneMonsters == null) return;

        bool thisZoneBossKilled = MissionManager.instance.HasZoneBossDied();

        if (thisZoneBossKilled){
            MissionManager.instance.GoToNextZone();
            // sound
            UIAudioManager.instance?.unlockZoneSound.Play();
        }

        ZoneNameUI[] zoneNameUIs = FindObjectsOfType<ZoneNameUI>();
        for (int i = 0; i < zoneNameUIs.Length; i++)
        {
            zoneNameUIs[i].UpdateText();
        }
    }
}
