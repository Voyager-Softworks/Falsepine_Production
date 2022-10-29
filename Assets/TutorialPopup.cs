using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class TutorialPopup : MonoBehaviour
{
    public TextMeshProUGUI m_title;
    public TextMeshProUGUI m_text;
    public Button acceptButton;
    public Button declineButton;

    private bool m_lastChance = false;

    // Start is called before the first frame update
    void Start()
    {
        if (JournalManager.instance != null && !JournalManager.instance.m_showTutorial)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable() {
        acceptButton.onClick.AddListener(OnAcceptTutorial);
        declineButton.onClick.AddListener(OnDeclineTutorial);
    }

    private void OnDisable() {
        acceptButton.onClick.RemoveListener(OnAcceptTutorial);
        declineButton.onClick.RemoveListener(OnDeclineTutorial);
    }

    // Update is called once per frame
    void Update()
    {
        // if escape is pressed, close window
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnDeclineTutorial();
        }
    }

    /// <summary>
    /// Loads the player into the tutorial level.
    /// </summary>
    public void OnAcceptTutorial()
    {
        gameObject.SetActive(false);

        JournalManager.instance.m_showTutorial = false;

        LevelController.LoadTutorial();
    }

    /// <summary>
    /// Gives the player the items they would have gotten from the tutorial.
    /// </summary>
    public void OnDeclineTutorial()
    {
        // double check
        if (m_lastChance == false){
            m_lastChance = true;
            m_title.text = "Comfirm Skip";
            m_text.text = "This popup will appear again after dying";
            return;
        }

        Debug.Log("Tutorial Declined");

        JournalManager.instance.m_showTutorial = false;

        gameObject.SetActive(false);
    }
}
