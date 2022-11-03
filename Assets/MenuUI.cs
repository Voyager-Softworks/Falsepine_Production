using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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

        // if mouse is over any buttons, enable the last image
        Vector2 mousePos = Mouse.current.position.ReadValue();
        
        if (playButton.GetComponent<RectTransform>().rect.Contains(playButton.transform.InverseTransformPoint(mousePos))){
            playButtonImage.enabled = true;
        } else {
            playButtonImage.enabled = false;
        }

        if (deleteSaveButton.GetComponent<RectTransform>().rect.Contains(deleteSaveButton.transform.InverseTransformPoint(mousePos))){
            deleteSaveButtonImage.enabled = true;
        } else {
            deleteSaveButtonImage.enabled = false;
        }

        if (quitButton.GetComponent<RectTransform>().rect.Contains(quitButton.transform.InverseTransformPoint(mousePos))){
            quitButtonImage.enabled = true;
        } else {
            quitButtonImage.enabled = false;
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
