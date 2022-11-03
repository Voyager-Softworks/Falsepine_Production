using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrimaevalDialogue : MonoBehaviour
{
    public TMP_Text text;
    public List<string> dialogueOptions;

    private int dialogueIndex = 0;
    public bool dialogueActive = false;
    private string dialogue;

    public float charDelay = 0.1f;
    float timer = 0;
    float fadeTimer = 0;

    public float showDialogueAfterCompleteTime = 1f;
    Color textCol;

    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        textCol = text.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueActive)
        {
            if (dialogueIndex < dialogue.Length)
            {
                timer += Time.deltaTime;
                if (timer > charDelay)
                {
                    text.text += dialogue[dialogueIndex];
                    dialogueIndex++;
                    timer = 0;
                }
            }
            else
            {
                timer += Time.deltaTime;
                if (timer > showDialogueAfterCompleteTime)
                {
                    if (fadeTimer < 1.0f)
                    {
                        fadeTimer += Time.deltaTime;
                        text.color = Color.Lerp(textCol, Color.clear, fadeTimer / 1.0f);
                    }
                    else
                    {
                        dialogueActive = false;
                        text.text = "";
                        timer = 0;
                        dialogueIndex = 0;
                    }
                }
            }
        }
    }

    public void BeginDialogue()
    {
        dialogueActive = true;
        dialogue = dialogueOptions[Random.Range(0, dialogueOptions.Count)];
        audioSource.Play();
    }
}
