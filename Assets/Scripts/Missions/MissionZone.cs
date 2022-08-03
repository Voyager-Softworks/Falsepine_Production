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

    [SerializeField] public string m_title;
    [TextArea(4, 10)]
    [SerializeField] public string m_description;
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