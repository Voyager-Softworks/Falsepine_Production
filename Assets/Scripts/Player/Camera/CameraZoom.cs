using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the zooming of the camera.
/// </summary>
public class CameraZoom : MonoBehaviour
{
    public InputAction zoomAction;
    CinemachineVirtualCamera cinemachineCam;
    Camera cam;
    public float currentZoom = 10f;
    public float minZoom = 9.0f;
    public float maxZoom = 9.0f;
    public float zoomAmount = 10.0f;
    public float zoomSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        cinemachineCam = GetComponent<CinemachineVirtualCamera>();
        cam = GetComponent<Camera>();

        zoomAction.Enable();
    }

    private void OnEnable() {
        EnableInput();
    }
    private void OnDisable() {
        DisableInput();
    }

    /// <summary>
    /// Enables the input for the zoom action.
    /// </summary>
    public void EnableInput()
    {
        zoomAction.Enable();
    }
    /// <summary>
    /// Disables the input for the zoom action.
    /// </summary>
    public void DisableInput()
    {
        zoomAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // if any windows are open, don't zoom
        if (ToggleableTownWindow.AnyWindowOpen()) {
            DisableInput();
        }
        else {
            EnableInput();
        }

        Zoom();
    }

    /// <summary>
    /// Updates the zoom of the camera using input and current values.
    /// </summary>
    void Zoom()
    {
        if (cinemachineCam == null && cam == null) return;

        if (zoomAction.ReadValue<float>() > 0.0f)
        {
            currentZoom -= zoomAmount;
        }
        else if (zoomAction.ReadValue<float>() < 0.0f)
        {
            currentZoom += zoomAmount;
        }
        
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        //if ortho set size
        if (cinemachineCam != null){
            cinemachineCam.m_Lens.OrthographicSize = Mathf.Lerp(cinemachineCam.m_Lens.OrthographicSize, currentZoom, Time.deltaTime * zoomSpeed);
        }
        else{
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, currentZoom, Time.deltaTime * zoomSpeed);
        }

        //if perspective set fov
        if (cinemachineCam != null){
            cinemachineCam.m_Lens.FieldOfView = Mathf.Lerp(cinemachineCam.m_Lens.FieldOfView, currentZoom, Time.deltaTime * zoomSpeed);
        }
        else{
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, currentZoom, Time.deltaTime * zoomSpeed);
        }

    }
}
