using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// This class is used by the Mission class to store data about a specific mission condition. <br/>
/// It is intended to be used as a base class for specific mission conditions. <br/>
/// E.g. Kill 10 enemies (with shotguns), collect 5 items (without taking damage), etc.
/// </summary>
[Serializable]
public class MissionCondition : ISerializationCallbackReceiver
{
    // constructor
    public MissionCondition()
    {
        m_state = ConditionState.INCOMPLETE;

        // set name to class name
        m_name = GetType().Name;
    }

    public enum ConditionState {
        INCOMPLETE,
        COMPLETE,
        FAILED
    };
    
    [HideInInspector][SerializeField] public string m_name = "New Mission Condition";
    [SerializeField] protected ConditionState m_state = ConditionState.INCOMPLETE;

    public virtual string GetDescription()
    {
        return "An empty mission condition, please choose a condition type from the dropdown menu.";
    }

    void OnValidate()
    {
        // update name
        m_name = GetDescription();
    }
    void ISerializationCallbackReceiver.OnBeforeSerialize() => this.OnValidate();
    void ISerializationCallbackReceiver.OnAfterDeserialize() { }

    public virtual void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
    }

    public virtual void Update()
    {
    }

    public virtual ConditionState GetState(){
        UpdateState();

        return m_state;
    }

    public virtual void SetState(ConditionState _state){
        m_state = _state;
    }

    public virtual void UpdateState()
    {
        
    }

    public virtual void BeginCondition()
    {
        
    }

    public virtual void EndCondition()
    {
        
    }

    // equality operator ==
    public static bool operator ==(MissionCondition a, MissionCondition b)
    {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        // Return true if the fields match:
        return a.m_name == b.m_name && a.m_state == b.m_state;
    }
    // inequality operator !=
    public static bool operator !=(MissionCondition a, MissionCondition b)
    {
        return !(a == b);
    }
    // equality operator
    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        MissionCondition p = obj as MissionCondition;
        if ((System.Object)p == null)
        {
            return false;
        }

        // return if they are ==
        return (p == this);
    }
    // override hashcode
    public override int GetHashCode()
    {
        return m_name.GetHashCode() ^ m_state.GetHashCode();
    }
}