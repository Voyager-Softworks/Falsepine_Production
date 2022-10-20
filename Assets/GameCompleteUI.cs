using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCompleteUI : MonoBehaviour
{
    public Button loopButton;
    public Button endButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        RebindButtons();
    }

    private void RebindButtons()
    {
        // remove all listeners
        loopButton.onClick.RemoveAllListeners();
        endButton.onClick.RemoveAllListeners();

        // bind buttons
        loopButton.onClick.AddListener(LoopPressed);
        endButton.onClick.AddListener(EndPressed);
    }

    private void OnDisable() {
        // remove all listeners
        loopButton.onClick.RemoveAllListeners();
        endButton.onClick.RemoveAllListeners();
    }

    public void LoopPressed()
    {
        MissionManager.instance.GoToNextZone();
        LevelController.LoadTown();
    }

    public void EndPressed()
    {
        // complete save
        SaveManager.CompleteSave(SaveManager.currentSaveSlot);

        LevelController.DestroyManagers();

        LevelController.LoadMenu(false);
    }
}
