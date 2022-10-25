using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// The primary way to travel and load levels. Communicates with the SaveManager to save and load the state of the game.
/// </summary>
public class LevelController : MonoBehaviour
{
    static public void LoadScene(string sceneName, bool _doSave = true)
    {
        // Disabled here, as the async scene loader is now responsible for saving
        //if (_doSave) SaveManager.SaveAll(SaveManager.currentSaveSlot);

        //SceneManager.LoadScene(sceneName);
        FindObjectOfType<AsyncSceneLoader>().LoadScene(sceneName, _doSave);
    }

    static public void LoadMenu(bool _doSave = true)
    {
        if (_doSave) SaveManager.SaveAll(SaveManager.currentSaveSlot);

        DestroyManagers();

        SceneManager.LoadScene("Menu");
    }

    static public void LoadTown(bool _doSave = true)
    {
        if (_doSave) SaveManager.SaveAll(SaveManager.currentSaveSlot);

        SceneManager.LoadScene("TownScene");
    }

    static public void LoadTutorial(bool _doSave = true)
    {
        if (_doSave) SaveManager.SaveAll(SaveManager.currentSaveSlot);

        SceneManager.LoadScene("TutorialScene");
    }

    static public void LoadSnow(bool _doSave = true)
    {
        if (_doSave) SaveManager.SaveAll(SaveManager.currentSaveSlot);

        SceneManager.LoadScene("SnowLevel");
    }

    static public void LoadSnowBoss(bool _doSave = true)
    {
        if (_doSave) SaveManager.SaveAll(SaveManager.currentSaveSlot);

        SceneManager.LoadScene("BoneStag");
    }

    static public void LoadMissionComplete(bool _doSave = true)
    {
        if (_doSave) SaveManager.SaveAll(SaveManager.currentSaveSlot);

        SceneManager.LoadScene("Scene_MissonComplete");
    }

    static public void LoadGameComplete(bool _doSave = true)
    {
        SceneManager.LoadScene("Scene_GameComplete");
    }

    /// <summary>
    /// Soft deletes progress and loads the game over scene.
    /// </summary>
    static public void LoadGameOver()
    {
        //SaveManager.SoftDeleteAll(SaveManager.currentSaveSlot);
        SaveManager.GameOverRestart(SaveManager.currentSaveSlot);

        DestroyManagers();

        SceneManager.LoadScene("Scene_GameOver");
    }

    static public void DestroyManagers()
    {
        if (MissionManager.instance) Destroy(MissionManager.instance.gameObject);
        if (InventoryManager.instance) Destroy(InventoryManager.instance.gameObject);
        if (JournalManager.instance) Destroy(JournalManager.instance.gameObject);
        if (EconomyManager.instance) Destroy(EconomyManager.instance.gameObject);
        if (StatsManager.instance) Destroy(StatsManager.instance.gameObject);
        if (MessageManager.instance) Destroy(MessageManager.instance.gameObject);
    }

    static public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    static public void Quit(/* bool _doSave = true */)
    {
        // if (_doSave)
        // {
        //     SaveManager.SaveAll(SaveManager.currentSaveSlot);
        // }

        Application.Quit();
    }

    private static Dictionary<MonoBehaviour, bool> m_scriptStates = new Dictionary<MonoBehaviour, bool>();
    private static List<MonoBehaviour> m_requesters = new List<MonoBehaviour>();

    private static bool m_isPaused = false;
    public static bool IsPaused { get { return m_isPaused; } }

    public static System.Action GamePaused;
    public static System.Action GameUnpaused;

    /// <summary>
    /// Adds a script to list of requesters, then pauses the game.
    /// </summary>
    /// <param name="_requester"></param>
    public static void RequestPause(MonoBehaviour _requester)
    {
        if (!m_requesters.Contains(_requester))
        {
            m_requesters.Add(_requester);
        }

        CheckRequesters();

        ForcePause();
    }

    /// <summary>
    /// Removes a requester from the list of requesters, if no requesters are left, the game will be unpaused.
    /// </summary>
    /// <param name="_requester"></param>
    public static void RequestUnpause(MonoBehaviour _requester)
    {
        if (m_requesters.Contains(_requester))
        {
            m_requesters.Remove(_requester);
        }

        CheckRequesters();
    }

    /// <summary>
    /// Ensures that all requesters are still valid, if not, removes them from the list. <br/>
    /// If no requesters are left, the game will be unpaused.
    /// </summary>
    public static void CheckRequesters()
    {
        for (int i = 0; i < m_requesters.Count; i++)
        {
            // if null, disabled, or destroyed, remove from list
            if (m_requesters[i] == null || !m_requesters[i].enabled || m_requesters[i].gameObject == null)
            {
                m_requesters.RemoveAt(i);
            }
        }

        if (m_requesters.Count <= 0)
        {
            ForceUnpause();
        }
    }

    /// <summary>
    /// Sets time scale to 0, and calls GamePaused event.
    /// </summary>
    public static void ForcePause()
    {
        Time.timeScale = 0;

        m_isPaused = true;

        GamePaused?.Invoke();
    }

    /// <summary>
    /// Sets time scale to 1, clears requesters, and calls GameUnpaused event.
    /// </summary>
    public static void ForceUnpause()
    {
        Time.timeScale = 1;

        m_isPaused = false;

        // clear requesters
        m_requesters.Clear();

        GameUnpaused?.Invoke();
    }
}
