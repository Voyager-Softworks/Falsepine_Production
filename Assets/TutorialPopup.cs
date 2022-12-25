using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
        if (Keyboard.current.escapeKey.wasPressedThisFrame || Gamepad.current.buttonEast.wasPressedThisFrame)
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

        // move all items from home inventory to player inventory
        Inventory homeInventory = InventoryManager.instance.GetInventory("home");
        Inventory playerInventory = InventoryManager.instance.GetInventory("player");
        if (homeInventory != null && playerInventory != null)
        {
            for (int i = homeInventory.slots.Count - 1; i >= 0; i--)
            {
                InventoryManager.instance.TryMoveItem(InventoryManager.instance.GetInventory("home"), InventoryManager.instance.GetInventory("player"), i);
            }
        }

        // give player the silver
        EconomyManager.instance.AddMoney(20);

        MessageManager.instance.AddMessage("Tutorial items granted!", "silver", true);

        // sound
        UIAudioManager.instance?.equipSound.Play();


        JournalManager.instance.m_showTutorial = false;

        gameObject.SetActive(false);
    }
}
