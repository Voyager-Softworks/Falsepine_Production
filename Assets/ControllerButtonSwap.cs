using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControllerButtonSwap : MonoBehaviour
{
    public string keyboard = "<sprite name=\"Btn_J\">";
    public string controller = "<sprite name=\"Btn_DUp\">";

    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = keyboard;
    }

    // Update is called once per frame
    void Update()
    {
        if (CustomInputManager.LastInputWasGamepad){
            text.text = controller;
        } else {
            text.text = keyboard;
        }
    }
}
