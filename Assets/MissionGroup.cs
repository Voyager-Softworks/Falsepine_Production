using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MissionGroup : MonoBehaviour
{
    public Mission m_linkedMission;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // editor
    #if UNITY_EDITOR
    [CustomEditor(typeof(MissionGroup))]
    public class MissionGroupEditor : Editor
    {
        // menu item
        [MenuItem("Dev/Set Up Mission Groups")]
        static void SetUpGroups()
        {
            Mission investigate = null;
            // get the Mission containing the word "Investigate" from Assets/Scripts/Missions/GrizzlyPeaks
            foreach (var mission in AssetDatabase.FindAssets("Investigate", new[] { "Assets/Scripts/Missions/GrizzlyPeaks" }).Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<Mission>))
            {
                if (mission.m_type == Mission.MissionType.INVESTIGATION)
                {
                    investigate = mission;
                    break;
                }
            }

            // find all objects containing the name "Objective", with no parent:

            var allObjectives = GameObject.FindObjectsOfType<GameObject>().Where(o => o.name.ToLower().Contains("objective") && o.transform.parent == null);


            //check if they have a MissionGroup component, if not, add it:
            foreach (var objective in allObjectives)
            {
                if (objective.GetComponent<MissionGroup>() == null)
                {
                    objective.AddComponent<MissionGroup>();
                }

                if (objective.name.ToLower().Contains("investigate"))
                {
                    objective.GetComponent<MissionGroup>().m_linkedMission = investigate;
                }
            }
        }
    }
    #endif
}
