using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton donotdestroy script that is used to manage the scene transitions.
public class LevelController : MonoBehaviour
{
    public static LevelController instance;

    // void Awake() {
    //     if (instance == null) {
    //         instance = this;
    //         //do not destroy this object
    //         DontDestroyOnLoad(this);
    //     } else {
    //         Destroy(this);
    //         Destroy(gameObject);
    //     }
    // }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //DEBUG
        //if pressed 0,1,2, go to that scene
        if (Keyboard.current.numpad0Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene(0);
        }
        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene(1);
        }
        if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene(2);
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadTown(){
        SceneManager.LoadScene(2);
    }

    public void Quit(){
        Application.Quit();
    }

    public void LoadScene(int _index){
        SceneManager.LoadScene(_index);
    }

    // public void LoadWin()
    // {
    //     SceneManager.LoadScene(2);
    // }
}
