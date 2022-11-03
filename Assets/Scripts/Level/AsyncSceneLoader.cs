using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class AsyncSceneLoader : MonoBehaviour
{
    bool isSceneLoading = false;
    public Image loadingIcon;
    public GameObject loadingScreen;


    // Start is called before the first frame update
    void Start()
    {
        isSceneLoading = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSceneLoading)
        {
            loadingIcon.color = new Color(1, 1, 1, (((Mathf.Sin(Time.time * 3) + 1f) / 2f) * 0.75f) + 0.25f);
        }
    }

    public void LoadScene(string sceneName, bool save = false)
    {
        if (!isSceneLoading)
        {
            loadingScreen.SetActive(true);
            isSceneLoading = true;
            StartCoroutine(LoadSceneAsync(sceneName));

            // close all toggle windows
            ToggleableWindow.CloseAllWindows();

            // ensure time is 1s
            Time.timeScale = 1f;
        }
    }

    IEnumerator LoadSceneAsync(string sceneName, bool save = false)
    {
        yield return new WaitForSeconds(1);
        //if (save) SaveManager.SaveAll(SaveManager.currentSaveSlot);
        if (save)
        {
            var tcs = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                Debug.Log("Saving Asynchronously...");
                SaveManager.SaveAll(SaveManager.currentSaveSlot);
                Debug.Log("Saved!");
                tcs.SetResult(true);
            });
            while (!tcs.Task.IsCompleted)
            {
                loadingIcon.color = new Color(1, 1, 1, (((Mathf.Sin(Time.time * 3) + 1f) / 2f) * 0.75f) + 0.25f);
                yield return null;
            }
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            loadingIcon.color = new Color(1, 1, 1, (((Mathf.Sin(Time.time * 3) + 1f) / 2f) * 0.75f) + 0.25f);
            yield return null;
        }
        loadingScreen.SetActive(false);
        isSceneLoading = false;
    }
}
