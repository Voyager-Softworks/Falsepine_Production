using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

/// <summary>
/// Fades the screen out, then transitions to another scene.
/// </summary>
public class FadeScript : MonoBehaviour
{
    private bool m_trueCompleteFalseOver = true;

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

        // fade delay
        if (fadeDelayTimer < fadeDelay)
        {
            fadeDelayTimer += Time.deltaTime;
        }
        else
        {
            // fade
            if (currentColor != targetColor)
            {
                fadeTimer += Time.deltaTime;
                if (fadeTimer >= fadeTime)
                {
                    currentColor = targetColor;
                }
                else
                {
                    currentColor = Color.Lerp(startColor, targetColor, fadeTimer / fadeTime);
                }
                fadeImage.color = currentColor;
            }
        }

        // ensure done event is called
        if (fadeDelayTimer >= fadeDelay && fadeTimer >= fadeTime)
        {
            if (isStartFade)
            {
                isStartFade = false;
                OnStartFadeDone.Invoke();
            }
            else if (isEndFade)
            {
                isEndFade = false;
                OnEndFadeDone.Invoke();
            }
        }
    }

    /// <summary>
    /// Tries to load the destination scene
    /// </summary>
    public void GotoDestination()
    {
        if (m_trueCompleteFalseOver)
        {
            LevelController.LoadComplete();
        }
        else
        {
            LevelController.LoadGameOver();
        }
    }

    /// <summary>
    /// Fades between two colours over time
    /// </summary>
    /// <param name="_a"></param>
    /// <param name="_b"></param>
    /// <param name="_time"></param>
    /// <param name="_delay"></param>
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

    /// <summary>
    /// Fades from black to clear
    /// </summary>
    /// <param name="_fadeTime"></param>
    /// <param name="_fadeDelay"></param>
    public void FadeFromBlackToClear(float _fadeTime = 1.0f, float _fadeDelay = 0.0f)
    {
        FadeFromAToB(Color.black, Color.clear, _fadeTime, _fadeDelay);
    }

    /// <summary>
    /// Fades from clear to black
    /// </summary>
    /// <param name="_fadeTime"></param>
    /// <param name="_fadeDelay"></param>
    public void FadeFromClearToBlack(float _fadeTime = 1.0f, float _fadeDelay = 0.0f)
    {
        FadeFromAToB(Color.clear, Color.black, _fadeTime, _fadeDelay);
    }

    /// <summary>
    /// Fades from black to clear (used when starting the scene)
    /// </summary>
    public void StartScreen()
    {
        isStartFade = true;
        FadeFromBlackToClear(3.0f, 2.0f);
    }

    /// <summary>
    /// Fades from clear to black (used when ending the scene)
    /// </summary>
    /// <param name="_trueCompleteFalseOver"></param>
    public void EndScreen(bool _trueCompleteFalseOver = true, float _delay = 0f)
    {
        isEndFade = true;
        FadeFromClearToBlack(3.0f, 2.0f + _delay);
        m_trueCompleteFalseOver = _trueCompleteFalseOver;
    }
}
