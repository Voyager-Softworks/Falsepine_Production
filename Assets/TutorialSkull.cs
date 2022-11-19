using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
///  Script to implement the talking tutorial skulls.
/// </summary>
public class TutorialSkull : MonoBehaviour
{
    public string[] tutorialText;
    string currText;

    public float textSpeed = 0.1f;
    float textTimer = 0.0f;
    public float lineDelay = 0.5f;
    float lineTimer = 0.0f;

    int currLine = 0;

    public bool isTalking = false;

    public TMP_Text text;

    public Animator anim;

    Transform player;
    public float triggerDistance = 2.0f;
    public float startTalkingDelay = 1.0f;
    float startTalkingTimer = 0.0f;
    bool playerInRange = false;

    Quaternion initRot;

    // Start is called before the first frame update
    void Start()
    {
        currText = tutorialText[currLine]; // Set current text
        text.text = ""; // Set text to empty
        initRot = transform.rotation; // Set initial rotation
        text.isOverlay = true; // Set text to overlay
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) // Find the player transform
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }



        if (playerInRange && !isTalking) // if the player is in range and the skull is not talking
        {
            startTalkingTimer += Time.deltaTime; // Increment the timer
            if (startTalkingTimer > startTalkingDelay) // If the timer surpasses the delay, set the skull to be now talking
            {
                isTalking = true;
            }
        }
        else if (!playerInRange) // Otherwise, if the player is not in range, then check if they are now in range
        {
            playerInRange = Vector3.Distance(transform.position, player.position) < triggerDistance;
            transform.rotation = Quaternion.Lerp(transform.rotation, initRot, Time.deltaTime * 2.0f); // Rotate the skull to look at the player
            if (playerInRange) // If the player is now in range, reset the timer and play the wake animation.
            {
                startTalkingTimer = 0.0f;
                anim.SetTrigger("Wake");
            }
        }
        if (playerInRange) // Otherwise, if the player is in range
        {
            playerInRange = Vector3.Distance(transform.position, player.position) < triggerDistance; // Check that the player is still in range
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.position - transform.position), Time.deltaTime * 2.0f); // Rotate the skull to its default position
            if (!playerInRange) // If the player is no longer in range, play the sleep animation, reset the text, and stop the talking
            {
                isTalking = false;
                anim.SetTrigger("Sleep");
                Reset();
            }
        }

        // Type out the text character by character, line by line
        if (isTalking)
        {
            if (textTimer > textSpeed) // Type a character every X seconds
            {
                textTimer = 0.0f;
                if (currText.Length > 0)
                {
                    // Check if the next character is the start of a tag (e.g. <color=red>)
                    if (currText[0] == '<')
                    {
                        // Find the end of the tag
                        int tagEnd = currText.IndexOf('>') + 1;
                        // Add the whole tag to the text
                        text.text += currText.Substring(0, tagEnd);
                        // Remove the tag from the current text
                        currText = currText.Substring(tagEnd);
                    }
                    else
                    {
                        // If it's not a tag, just add one character
                        text.text += currText[0];
                        currText = currText.Substring(1);
                    }
                }
                else
                {
                    if (lineTimer > lineDelay)
                    {
                        lineTimer = 0.0f;
                        currLine++;
                        if (currLine < tutorialText.Length)
                        {
                            currText = tutorialText[currLine];
                            // Add a newline to the text
                            text.text += "\n";
                            text.text += "—";
                            text.text += "\n";
                        }
                        else
                        {
                            isTalking = false;
                        }
                    }
                    else
                    {
                        lineTimer += Time.deltaTime;
                    }
                }
            }
            else
            {
                textTimer += Time.deltaTime;
            }
        }
    }

    /// <summary>
    ///  Clears the textbox
    /// </summary>
    public void ClearText()
    {
        text.text = "";
    }

    /// <summary>
    ///  Resets the Skull to its default state
    /// </summary>
    public void Reset()
    {
        currLine = 0;
        textTimer = 0.0f;
        lineTimer = 0.0f;
        isTalking = false;
        currText = tutorialText[currLine];
        ClearText();
    }

    /// <summary>
    ///  Starts the talking
    /// </summary>
    public void StartTalking()
    {
        isTalking = true;
    }

    /// <summary>
    ///  Stops the talking
    /// </summary>
    public void StopTalking()
    {
        isTalking = false;
    }
}
