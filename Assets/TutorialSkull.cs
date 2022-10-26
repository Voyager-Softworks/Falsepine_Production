using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        currText = tutorialText[currLine];
        text.text = "";
        initRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }



        if (playerInRange && !isTalking)
        {
            startTalkingTimer += Time.deltaTime;
            if (startTalkingTimer > startTalkingDelay)
            {
                isTalking = true;
            }
        }
        else if (!playerInRange)
        {
            playerInRange = Vector3.Distance(transform.position, player.position) < triggerDistance;
            transform.rotation = Quaternion.Lerp(transform.rotation, initRot, Time.deltaTime * 2.0f);
            if (playerInRange)
            {
                startTalkingTimer = 0.0f;
                anim.SetTrigger("Wake");
            }
        }
        if (playerInRange)
        {
            playerInRange = Vector3.Distance(transform.position, player.position) < triggerDistance;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.position - transform.position), Time.deltaTime * 2.0f);
            if (!playerInRange)
            {
                isTalking = false;
                anim.SetTrigger("Sleep");
                Reset();
            }
        }

        if (isTalking)
        {
            if (textTimer > textSpeed)
            {
                textTimer = 0.0f;
                if (currText.Length > 0)
                {
                    text.text += currText[0];
                    currText = currText.Remove(0, 1);
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

    public void ClearText()
    {
        text.text = "";
    }

    public void Reset()
    {
        currLine = 0;
        textTimer = 0.0f;
        lineTimer = 0.0f;
        isTalking = false;
        currText = tutorialText[currLine];
        ClearText();
    }

    public void StartTalking()
    {
        isTalking = true;
    }

    public void StopTalking()
    {
        isTalking = false;
    }
}
