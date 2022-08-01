using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enables the cursor on the scene.
/// </summary>
public class EnableCursor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
