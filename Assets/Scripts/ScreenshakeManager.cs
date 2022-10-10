using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenshakeManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    [ReadOnly, SerializeField] private float shakeTimer;

    [ReadOnly, SerializeField] private Vector3 currentShakeAmplitude;
    [ReadOnly, SerializeField] private float currentShakeFrequency;

    [SerializeField] private AnimationCurve shakeAmplitudeOverTime;
    [SerializeField] private AnimationCurve shakeFrequencyOverTime;

    [SerializeField] private float shakeDecreaseFactor = 1f; // How fast the shake decreases
    // Start is called before the first frame update
    void Start()
    {
        virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        if (virtualCameraNoise == null)
        {
            virtualCameraNoise = virtualCamera.AddCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            currentShakeAmplitude = shakeAmplitudeOverTime.Evaluate(1 - shakeTimer) * Vector3.one;
            currentShakeFrequency = shakeFrequencyOverTime.Evaluate(1 - shakeTimer);
        }
        else
        {
            currentShakeAmplitude = Vector3.Lerp(currentShakeAmplitude, Vector3.zero, shakeDecreaseFactor * Time.deltaTime);
            currentShakeFrequency = Mathf.Lerp(currentShakeFrequency, 0, shakeDecreaseFactor * Time.deltaTime);
        }

        virtualCameraNoise.m_AmplitudeGain = currentShakeAmplitude.x;
        virtualCameraNoise.m_FrequencyGain = currentShakeFrequency;
    }

    public void AddShakeImpulse(float duration, Vector3 amplitude, float frequency)
    {
        shakeTimer = Mathf.Max(shakeTimer, duration);
        currentShakeAmplitude = Vector3.Max(currentShakeAmplitude, amplitude);
        currentShakeFrequency = Mathf.Max(currentShakeFrequency, frequency);
    }
}
