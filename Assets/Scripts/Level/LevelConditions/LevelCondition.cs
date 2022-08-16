using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for conditions in the level that need to be completed in order to progress.
/// </summary>
public class LevelCondition : MonoBehaviour
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

    /// <summary>
    /// Update the condition based on implimented criteria.
    /// </summary>
    protected virtual void UpdateCondition()
    {
        // Do nothing
    }
}