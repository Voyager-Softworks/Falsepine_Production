using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class MenuUI : MonoBehaviour
{
    public Button playButton;
    public Button deleteSaveButton;
    public Button quitButton;

    
    private bool m_deleteCheck = false;
    private float m_newGameResetTime = 2.0f;
    private float m_newGameTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable() {
        playButton.onClick.AddListener(PlayGame);
        deleteSaveButton.onClick.AddListener(DeleteSave);
        quitButton.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {
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
