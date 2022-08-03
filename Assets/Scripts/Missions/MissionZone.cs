using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Mission Zone class is responsible for storing data about a specific misison zone <br/>
/// E.g. Snow Zone, its missions, and its scenes.
/// @Todo: Decide if we should allow saving and quitting during mission. <br/>
/// - If we do, we need to make sure that we save clues and current scene correctly, to now allow abusing of the save system.
/// </summary>
[CreateAssetMenu(fileName = "New Mission", menuName = "Missions/New Mission Zone")]
[Serializable]
public class MissionZone : ScriptableObject
{
    [Serializable]
    public enum ZoneArea
    {
        SNOW,
        DESERT,
        REDWOOD,
        SWAMP,
        FOREST
    }

    public string m_title;
    [TextArea(4, 10)]
    public string m_description;
    public ZoneArea m_area;

    [Header("Missions")]
    public List<Mission> m_possibleLesserMissions;
    public List<Mission> m_possibleGreaterMissions;

    private int m_lesserMissionCount = 3;
    private List<Mission> m_lesserMissions = new List<Mission>();
    private Mission m_greaterMission;

    [Header("Scenes")]
    public Utilities.SceneField m_startScene;
    public Utilities.SceneField m_endScene;
    public int m_middleSceneCount = 6;
    public List<Utilities.SceneField> m_possibleMiddleScenes;
    private List<Utilities.SceneField> m_middleScenes = new List<Utilities.SceneField>();

    public void RandomiseLesserMissions()
    {
        // make new list of possible lesser missions
        List<Mission> tempPLM = new List<Mission>();

        // randomise tempPLM
        tempPLM = m_possibleLesserMissions.OrderBy(x => UnityEngine.Random.value).ToList();

        // add to lesser missions
        for (int i = 0; i < m_lesserMissionCount; i++)
        {
            m_lesserMissions.Add(tempPLM[i]);
        }
    }

    public void RandomiseGreaterMission()
    {
        // make new list of possible lesser missions
        List<Mission> tempPGM = new List<Mission>();

        // randomise tempPLM
        tempPGM = m_possibleGreaterMissions.OrderBy(x => UnityEngine.Random.value).ToList();

        // add to lesser missions
        m_greaterMission = tempPGM[0];
    }

    public void RandomiseMiddleScenes()
    {
        // make new list of possible lesser missions
        List<Utilities.SceneField> tempPMS = new List<Utilities.SceneField>();

        // randomise tempPLM
        tempPMS = m_possibleMiddleScenes.OrderBy(x => UnityEngine.Random.value).ToList();

        // add to lesser missions
        for (int i = 0; i < m_middleSceneCount; i++)
        {
            m_middleScenes.Add(tempPMS[i]);
        }
    }

    /// <summary>
    /// Serializable class equivalent for the MissionZone ScriptableObject
    /// </summary>
    [Serializable]
    public class Serializable_MissionZone
    {
        [SerializeField] public string m_title;
        [SerializeField] public string m_description;
        [SerializeField] public MissionZone.ZoneArea m_area;

        // missions
        [SerializeField] public List<Mission.Serializable_Mission> m_possibleLesserMissions;
        [SerializeField] public List<Mission.Serializable_Mission> m_possibleGreaterMissions;
        [SerializeField] public int m_lesserMissionCount;
        [SerializeField] public List<Mission.Serializable_Mission> m_lesserMissions;
        [SerializeField] public Mission.Serializable_Mission m_greaterMission;

        // scenes
        [SerializeField] public Utilities.SceneField m_startScene;
        [SerializeField] public Utilities.SceneField m_endScene;
        [SerializeField] public int m_middleSceneCount;
        [SerializeField] public List<Utilities.SceneField> m_possibleMiddleScenes;
        [SerializeField] public List<Utilities.SceneField> m_middleScenes;

        public Serializable_MissionZone(MissionZone mz)
        {
            m_title = mz.m_title;
            m_description = mz.m_description;
            m_area = mz.m_area;



            // Missions:
            // make empty lists
            m_possibleLesserMissions = new List<Mission.Serializable_Mission>();
            m_possibleGreaterMissions = new List<Mission.Serializable_Mission>();
            m_lesserMissions = new List<Mission.Serializable_Mission>();

            // fill with data
            foreach (Mission m in mz.m_possibleLesserMissions)
            {
                m_possibleLesserMissions.Add(new Mission.Serializable_Mission(m));
            }
            foreach (Mission m in mz.m_possibleGreaterMissions)
            {
                m_possibleGreaterMissions.Add(new Mission.Serializable_Mission(m));
            }
            m_lesserMissionCount = mz.m_lesserMissionCount;
            foreach (Mission m in mz.m_lesserMissions)
            {
                m_lesserMissions.Add(new Mission.Serializable_Mission(m));
            }
            m_greaterMission = new Mission.Serializable_Mission(mz.m_greaterMission);



            // Scenes:
            m_startScene = mz.m_startScene;
            m_endScene = mz.m_endScene;
            m_middleSceneCount = mz.m_middleSceneCount;

            // make empty lists
            m_possibleMiddleScenes = new List<Utilities.SceneField>();
            m_middleScenes = new List<Utilities.SceneField>();

            // fill with data
            foreach (Utilities.SceneField sf in mz.m_possibleMiddleScenes)
            {
                m_possibleMiddleScenes.Add(sf);
            }
            foreach (Utilities.SceneField sf in mz.m_middleScenes)
            {
                m_middleScenes.Add(sf);
            }
        }

        public MissionZone ToMissionZone()
        {
            MissionZone mz = new MissionZone();

            mz.m_title = m_title;
            mz.m_description = m_description;
            mz.m_area = m_area;

            // Missions:
            // make empty lists
            mz.m_possibleLesserMissions = new List<Mission>();
            mz.m_possibleGreaterMissions = new List<Mission>();
            mz.m_lesserMissions = new List<Mission>();

            // fill with data
            foreach (Mission.Serializable_Mission sm in m_possibleLesserMissions)
            {
                mz.m_possibleLesserMissions.Add(sm.ToMission());
            }
            foreach (Mission.Serializable_Mission sm in m_possibleGreaterMissions)
            {
                mz.m_possibleGreaterMissions.Add(sm.ToMission());
            }
            mz.m_lesserMissionCount = m_lesserMissionCount;
            foreach (Mission.Serializable_Mission sm in m_lesserMissions)
            {
                mz.m_lesserMissions.Add(sm.ToMission());
            }
            mz.m_greaterMission = m_greaterMission.ToMission();

            // Scenes:
            mz.m_startScene = m_startScene;
            mz.m_endScene = m_endScene;
            mz.m_middleSceneCount = m_middleSceneCount;

            // make empty lists
            mz.m_possibleMiddleScenes = new List<Utilities.SceneField>();
            mz.m_middleScenes = new List<Utilities.SceneField>();

            // fill with data
            foreach (Utilities.SceneField sf in m_possibleMiddleScenes)
            {
                mz.m_possibleMiddleScenes.Add(sf);
            }
            foreach (Utilities.SceneField sf in m_middleScenes)
            {
                mz.m_middleScenes.Add(sf);
            }

            return mz;
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(MissionZone))]
    public class MissionZoneEditor : Editor
    {
        private MissionZone mz;

        public override void OnInspectorGUI()
        {
            if (mz == null)
            {
                mz = (MissionZone)target;
            }

            DrawDefaultInspector();
            EditorGUILayout.Space();
            DropAreaGUI();
        }

        public void DropAreaGUI()
        {
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            // set background color
            GUI.backgroundColor = Color.green;
            GUI.Box(drop_area, "Drag Scenes Here");
            GUI.backgroundColor = Color.white;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (System.Object dragged_object in DragAndDrop.objectReferences)
                        {
                            SceneAsset scene = dragged_object as SceneAsset;
                            if (scene != null)
                            {
                                Utilities.SceneField sf = new Utilities.SceneField();
                                sf.SceneAsset = scene;
                                mz.m_possibleMiddleScenes.Add(sf);

                                EditorUtility.SetDirty(mz);
                            }
                        }
                    }
                    break;
            }
        }
    }
    #endif
}