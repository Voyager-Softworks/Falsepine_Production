using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
///  Manages Screenshake for the game
/// </summary> 
public class ScreenshakeManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera; // The virtual camera to shake
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise; // The noise profile for the virtual camera

    [ReadOnly, SerializeField] private float shakeTimer; // The timer for the shake

    [ReadOnly, SerializeField] private Vector3 currentShakeAmplitude; // The current amplitude of the shake
    [ReadOnly, SerializeField] private float currentShakeFrequency; // The current frequency of the shake

    [SerializeField] private AnimationCurve shakeAmplitudeOverTime; // The curve that will be used to determine the amplitude of the shake over time
    [SerializeField] private AnimationCurve shakeFrequencyOverTime; // The curve that will be used to determine the frequency of the shake over time

    [SerializeField] private float shakeDecreaseFactor = 1f; // How fast the shake decreases
    // Start is called before the first frame update
    void Start()
    {
        virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>(); // Get the noise profile for the virtual camera
        if (virtualCameraNoise == null)
        {
            virtualCameraNoise = virtualCamera.AddCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>(); // Add the noise profile to the virtual camera if it doesn't have one
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer > 0)
        {
            // Decrease shake amount over time.
            shakeTimer -= Time.deltaTime; // Decrease the shake timer
            currentShakeAmplitude = shakeAmplitudeOverTime.Evaluate(1 - shakeTimer) * Vector3.one; // Set the current amplitude of the shake
            currentShakeFrequency = shakeFrequencyOverTime.Evaluate(1 - shakeTimer); // Set the current frequency of the shake
        }
        else
        {
            //Make the shake stop
            currentShakeAmplitude = Vector3.Lerp(currentShakeAmplitude, Vector3.zero, shakeDecreaseFactor * Time.deltaTime); // Decrease the current amplitude of the shake
            currentShakeFrequency = Mathf.Lerp(currentShakeFrequency, 0, shakeDecreaseFactor * Time.deltaTime); // Decrease the current frequency of the shake
        }

        virtualCameraNoise.m_AmplitudeGain = currentShakeAmplitude.x; // Set the amplitude of the shake
        virtualCameraNoise.m_FrequencyGain = currentShakeFrequency; // Set the frequency of the shake
    }

    /// <summary>
    ///  Shake the camera
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="amplitude"></param>
    /// <param name="frequency"></param>
    public void AddShakeImpulse(float duration, Vector3 amplitude, float frequency)
    {
        shakeTimer = Mathf.Max(shakeTimer, duration); // Set the shake timer to the duration of the shake, if the shake timer is already greater than the duration, do nothing
        currentShakeAmplitude = Vector3.Max(currentShakeAmplitude, amplitude); // Set the current amplitude of the shake to the amplitude of the shake, if the current amplitude is already greater than the amplitude, do nothing
        currentShakeFrequency = Mathf.Max(currentShakeFrequency, frequency); // Set the current frequency of the shake to the frequency of the shake, if the current frequency is already greater than the frequency, do nothing
    }
}
