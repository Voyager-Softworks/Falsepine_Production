using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.Log("Steam not initialized");
            return;
        }
        else
        {
            Debug.Log("Steam initialized");
            string name = SteamFriends.GetPersonaName();
            Debug.Log("Name: " + name);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
