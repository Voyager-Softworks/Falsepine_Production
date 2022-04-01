using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class FadeScript : MonoBehaviour
{
    public Image fadeImage;
    public Color startColor = Color.clear;
    private Color currentColor = Color.clear;
    public Color targetColor = Color.clear;

    public float fadeTime = 0.5f;
    private float fadeTimer = 0f;

    public float fadeDelay = 0.5f;
    private float fadeDelayTimer = 0f;

    bool isStartFade = false;
    bool isEndFade = false;

    public UnityEvent OnStartFadeDone;
    public UnityEvent OnEndFadeDone;

    // Start is called before the first frame update
    void Start()
    {
        fadeImage = GetComponent<Image>();
        if (fadeImage) fadeImage.color = startColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeImage == null) return;

        if (fadeDelayTimer < fadeDelay)
        {
            fadeDelayTimer += Time.deltaTime;
        }
        else
        {
            if (currentColor != targetColor)
            {
                fadeTimer += Time.deltaTime;
                if (fadeTimer >= fadeTime)
                {
                    currentColor = targetColor;
                    fadeTimer = 0f;

                    if (isStartFade)
                    {
                        isStartFade = false;
                        isEndFade = false;
                        OnStartFadeDone.Invoke();
                    }
                    else if (isEndFade)
                    {
                        isStartFade = false;
                        isEndFade = false;
                        OnEndFadeDone.Invoke();
                    }
                }
                else
                {
                    currentColor = Color.Lerp(startColor, targetColor, fadeTimer / fadeTime);
                }
                fadeImage.color = currentColor;
            }
        }
    }

    public void FadeFromAToB(Color _a, Color _b, float _time = 1.0f, float _delay = 0.0f)
    {
        startColor = _a;
        currentColor = _a;
        targetColor = _b;
        fadeTime = _time;
        fadeDelay = _delay;
        fadeTimer = 0f;
        fadeDelayTimer = 0f;
    }

    public void FadeFromBlackToClear(float _fadeTime = 1.0f, float _fadeDelay = 0.0f)
    {
        FadeFromAToB(Color.black, Color.clear, _fadeTime, _fadeDelay);
    }

    public void FadeFromClearToBlack(float _fadeTime = 1.0f, float _fadeDelay = 0.0f)
    {
        FadeFromAToB(Color.clear, Color.black, _fadeTime, _fadeDelay);
    }

    public void StartScreen(){
        isStartFade = true;
        FadeFromBlackToClear(3.0f, 2.0f);
    }

    public void EndScreen(){
        isEndFade = true;
        FadeFromClearToBlack(3.0f, 2.0f);
    }
}
