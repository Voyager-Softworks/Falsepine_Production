using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Stats Profile", menuName = "Stats Profile")]
public class StatsProfile : ScriptableObject, StatsManager.UsesStats
{
    // StatsManager.UsesStats interface implementation
    [SerializeField] public List<StatsManager.StatType> m_usedStatTypes = new List<StatsManager.StatType>();
    public List<StatsManager.StatType> GetStatTypes()
    {
        return m_usedStatTypes;
    }
    public void AddStatType(StatsManager.StatType statType)
    {
        if (!m_usedStatTypes.Contains(statType))
        {
            m_usedStatTypes.Add(statType);
        }
    }
    public void RemoveStatType(StatsManager.StatType statType)
    {
        if (m_usedStatTypes.Contains(statType))
        {
            m_usedStatTypes.Remove(statType);
        }
    }

    // custom editor
#if UNITY_EDITOR
    [CustomEditor(typeof(StatsProfile))]
    public class StatsProfileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            StatsProfile statsProfile = (StatsProfile)target;

            bool needToSave = false;
            StatsManager.StatTypeListDropdown(statsProfile, out needToSave);

            if (needToSave)
            {
                EditorUtility.SetDirty(statsProfile);
            }
        }
    }
#endif
}
