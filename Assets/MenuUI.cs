using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MenuUI : MonoBehaviour
{
    public Button playButton;
    public Image playButtonImage;

    public Button deleteSaveButton;
    public Image deleteSaveButtonImage;

    public Button quitButton;
    public Image quitButtonImage;

    
    private bool m_deleteCheck = false;
    private float m_newGameResetTime = 2.0f;
    private float m_newGameTimer = 0.0f;

    public float m_fadeTime = 0.5f;
    private float m_fadeTimer = 0.0f;

    [Header("Close on start")]
    public List<GameObject> closeOnStart;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in closeOnStart) {
            // if AudioControls call Init()
            if (obj.GetComponent<AudioControls>() != null) {
                obj.GetComponent<AudioControls>().Init();
            }
            // if BrightnessControls call Init()
            if (obj.GetComponent<BrightnessControls>() != null) {
                obj.GetComponent<BrightnessControls>().Init();
            }
            // if DisplayModeSetting call Init()
            if (obj.GetComponent<DisplayModeSetting>() != null) {
                obj.GetComponent<DisplayModeSetting>().Init();
            }
            obj.SetActive(false);
        }
    }

    private void OnEnable() {
        playButton.onClick.AddListener(PlayGame);
        deleteSaveButton.onClick.AddListener(DeleteSave);
        quitButton.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {
        // if time scale is not 1, set it to 1
        if (Time.timeScale != 1.0f) {
            Time.timeScale = 1.0f;
        }

        // update timer
        m_newGameTimer = Mathf.Max(0.0f, m_newGameTimer - Time.deltaTime);
        if (m_newGameTimer <= 0.0f) {
            if (m_deleteCheck)
            {
                m_deleteCheck = false;
            }
            deleteSaveButton.GetComponentInChildren<TextMeshProUGUI>().text = "Delete Save";
        }
    }

    void PlayGame() {
        LevelController.LoadTown();
    }

    void DeleteSave() {
        if (m_deleteCheck == false){
            m_deleteCheck = true;
            m_newGameTimer = m_newGameResetTime;

            deleteSaveButton.GetComponentInChildren<TextMeshProUGUI>().text = "Confirm Delete?";

            return;
        }

        SaveManager.DeleteAllSaves();

        deleteSaveButton.GetComponentInChildren<TextMeshProUGUI>().text = "Deleted Save!";
        m_deleteCheck = false;
        m_newGameTimer = m_newGameResetTime;
    }

    void QuitGame() {
        Application.Quit();
    }
}
