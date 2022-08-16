using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCondition : MonoBehaviour  /// @todo comment
{
    protected string m_title = "";
    protected string m_description = "";
    protected bool m_isComplete = false;

    public string title
    {
        get
        {
            return m_title;
        }
    }
    public string description
    {
        get
        {
            return m_description;
        }
    }
    public bool isComplete
    {
        get
        {
            UpdateCondition();
            return m_isComplete;
        }
    }

    protected virtual void UpdateCondition()
    {
        // Do nothing
    }
}