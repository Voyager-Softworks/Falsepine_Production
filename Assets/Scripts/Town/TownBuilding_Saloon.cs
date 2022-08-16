using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Town building for the saloon.
/// </summary>
public class TownBuilding_Saloon : TownBuilding
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //if UI is acvitve, update the UI
        if (UI.activeSelf)
        {
            UpdateUI();
        }
    }

    // public void SaveSaloon(){
    //     //save the saloon current page
    //     BinaryFormatter bf = new BinaryFormatter();
    //     FileStream file = File.Create(Application.persistentDataPath + "/saloon.dat");
    //     SaloonData data = new SaloonData();
    //     data.currentPage = currentPage;
    //     bf.Serialize(file, data);
    //     file.Close();
    // }

    // public void LoadSaloon(){
    //     if (File.Exists(Application.persistentDataPath + "/saloon.dat"))
    //     {
    //         BinaryFormatter bf = new BinaryFormatter();
    //         FileStream file = File.Open(Application.persistentDataPath + "/saloon.dat", FileMode.Open);
    //         SaloonData data = (SaloonData)bf.Deserialize(file);
    //         file.Close();

    //         currentPage = data.currentPage;
    //     }
    // }

    private void UpdateUI()
    {

    }
}
