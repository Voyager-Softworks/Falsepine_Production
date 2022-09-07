using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  For some reason the arm couldnt be a part of the boss' model, so I have to make a script to handle this one single case.
///  This script has a method called by an animation event in the boss's animation to trigger the animation in the hand object.
/// </summary>
public class ScriptToMakeTheArmWork : MonoBehaviour
{
    public Animator handAnimator;
    public void TriggerHandAnimation()
    {
        handAnimator.SetTrigger("Attack");
    }
}
