/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: PlaySound.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI.Audio
{
    /// <summary>
    /// A Node that plays a sound.
    /// </summary>
    public class PlaySound : ActionBase
    {
        AudioSource audioSource;
        public PlaySound()
        {
            AddProperty<AudioClip>("Sound", null);
            AddProperty<float>("Volume", 1);
            AddProperty<float>("Pitch", 1);
            AddProperty<float>("Delay", 0);
        }
        
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if(audioSource == null)
            {
                audioSource = agent.GetComponentInChildren<AudioSource>();
                if(audioSource == null)
                {
                    Debug.LogError("No AudioSource found on " + agent.gameObject.name);
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
            }
            audioSource.volume = GetProperty<float>("Volume");
            audioSource.pitch = GetProperty<float>("Pitch");
            audioSource.clip = GetProperty<AudioClip>("Sound");
            audioSource.PlayDelayed(GetProperty<float>("Delay"));
            state = NodeData.State.Success;
            return NodeData.State.Success;
        }
    }
}