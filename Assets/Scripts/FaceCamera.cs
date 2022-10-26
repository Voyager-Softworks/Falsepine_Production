using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes an object look at the camera (TextMeshPro usually)
/// </summary>
public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    public bool flipX = false;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(mainCamera.transform);
        transform.rotation = Quaternion.LookRotation(-mainCamera.transform.forward, Vector3.up);
        if (flipX)
        {
            transform.Rotate(0, 180, 0);
        }
    }
}
