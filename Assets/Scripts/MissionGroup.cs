using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Manages a group of objects in scene, enable/disable based on current mission
/// </summary>
public class MissionGroup : MonoBehaviour
{
    public Mission m_linkedMission;

    // editor
#if UNITY_EDITOR
    [CustomEditor(typeof(MissionGroup))]
    public class MissionGroupEditor : Editor
    {
        /// <summary>
        /// Sets the the mission groups in the scene (not good idea to use)
        /// </summary>
        //[MenuItem("Dev/Set Up Mission Groups")]
        static void SetUpGroups()
        {
            Mission investigate = null;
            Mission collect = null;
            Mission exterminate = null;
            // get the Mission containing the word "Investigate" from Assets/Scripts/Missions/GrizzlyPeaks
            foreach (var mission in AssetDatabase.FindAssets("_", new[] { "Assets/Scripts/Missions/GrizzlyPeaks" }).Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<Mission>))
            {
                if (mission.m_size != Mission.MissionSize.LESSER)
                {
                    continue;
                }

                switch (mission.m_type)
                {
                    case Mission.MissionType.INVESTIGATION:
                        investigate = mission;
                        break;
                    case Mission.MissionType.COLLECTION:
                        collect = mission;
                        break;
                    case Mission.MissionType.EXTERMINATION:
                        exterminate = mission;
                        break;
                }
            }

            // find all objects containing the name "Objective", with no parent:

            var allObjectives = GameObject.FindObjectsOfType<GameObject>(true).Where(o => o.name.ToLower().Contains("objective") && o.transform.parent == null);

            int changed = 0;
            //check if they have a MissionGroup component, if not, add it:
            foreach (var objective in allObjectives)
            {
                if (objective.GetComponent<MissionGroup>() == null)
                {
                    objective.AddComponent<MissionGroup>();
                }

                if (objective.name.ToLower().Contains("invest"))
                {
                    objective.GetComponent<MissionGroup>().m_linkedMission = investigate;
                }
                else if (objective.name.ToLower().Contains("collect"))
                {
                    objective.GetComponent<MissionGroup>().m_linkedMission = collect;
                }
                else if (objective.name.ToLower().Contains("exterm"))
                {
                    objective.GetComponent<MissionGroup>().m_linkedMission = exterminate;
                }

                //save the changes:
                EditorUtility.SetDirty(objective);

                changed++;
            }

            Debug.Log("Changed " + changed + " objectives.");
        }
    }
#endif
}
