using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Mission Zone class is responsible for storing data about a specific misison zone <br/>
/// E.g. Snow Zone, its missions, and its scenes.
/// @todo Decide if we should allow saving and quitting during mission. <br/>
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
    public List<MonsterInfo> m_zoneMonsters = new List<MonsterInfo>();


    [Header("Missions")]
    public List<Mission> m_possibleMissions;

    private int m_missionCount = 5;
    public List<Mission> m_missions = new List<Mission>();

    public Mission currentMission;


    [Header("Scenes")]
    public Utilities.SceneField m_startScene;
    public Utilities.SceneField m_endScene;
    public int m_middleSceneCount = 6;
    public List<Utilities.SceneField> m_possibleMiddleScenes;
    public List<Utilities.SceneField> m_middleScenes = new List<Utilities.SceneField>();
    public Utilities.SceneField m_bossScene;

    //public string m_currentScenePath = "";

    /// <summary>
    /// Randomises the current missions from the possible missions
    /// </summary>
    private void RandomiseMissions()
    {
        // make new list of possible missions
        List<Mission> tempPLM = new List<Mission>();

        // randomise tempPLM
        tempPLM = m_possibleMissions.OrderBy(x => UnityEngine.Random.value).ToList();

        // add to missions
        for (int i = 0; i < m_missionCount && i < tempPLM.Count(); i++)
        {
            m_missions.Add(tempPLM[i]);
        }
    }

    /// <summary>
    /// Randomises the current selection of scenes from the possible scenes.
    /// </summary>
    private void RandomiseMiddleScenes()
    {
        // make new list of possible missions
        List<Utilities.SceneField> tempPMS = new List<Utilities.SceneField>();

        // randomise tempPLM
        tempPMS = m_possibleMiddleScenes.OrderBy(x => UnityEngine.Random.value).ToList();

        // add to missions
        for (int i = 0; i < m_middleSceneCount; i++)
        {
            m_middleScenes.Add(tempPMS[i]);
        }
    }

    /// <summary>
    /// Resets the current zone and all missions within it to be not completed.
    /// </summary>
    public void Reset(){
        // clear missions
        m_missions.Clear();

        // reset possible missions
        foreach (Mission m in m_possibleMissions)
        {
            m.Reset();
        }

        // clear middle scenes
        m_middleScenes.Clear();

        // clear current scene indexs
        //m_currentScenePath = "";

        // Randomise all
        RandomiseMissions();
        RandomiseMiddleScenes();
    }

    /// <summary>
    /// Reinstantiate all missions to ensure that we have modifiable copies of the missions. <br/>
    /// - Dont want to modify the original mission assets.
    /// </summary>
    public void ReinstantiateMissions(){
        //store current mission info
        int currentIndex = -1;
        if (currentMission){
            currentIndex = GetMissionIndex(currentMission);
        }

        // mission list
        for (int i = 0; i < m_possibleMissions.Count; i++)
        {
            m_possibleMissions[i] = Instantiate(m_possibleMissions[i]);
        }
        for (int i = 0; i < m_missions.Count; i++)
        {
            if (m_missions[i])
            {
                m_missions[i] = Instantiate(m_missions[i]);
            }
        }

        //set current mission
        currentMission = GetMission(currentIndex);
    }

    /// <summary>
    /// Tries to set current mission to the given mission. <br/>
    /// Also sets current scene to -1. (This is to indicate that the current scene is not set yet)
    /// </summary>
    /// <param name="_misison"></param>
    /// <returns></returns>
    public bool TryStartMission(Mission _misison){
        //check if mission is in list
        int index = GetMissionIndex(_misison);
        if (index == -1){
            return false;
        }

        currentMission = _misison;

        //m_currentScenePath = "";
        return true;
    }

    /// <summary>
    /// Sets current mission to null
    /// </summary>
    /// <returns></returns>
    public bool TryReturnMission(){
        currentMission = null;
        return true;
    }


    /// <summary>
    /// Returns the index of the mission in the mission list.
    /// </summary>
    /// <param name="_mission"></param>
    /// <returns></returns>
    public int GetMissionIndex(Mission _mission)
    {
        return m_missions.IndexOf(_mission);
    }

    /// <summary>
    /// Returns the mission of specified size and index.
    /// </summary>
    /// <param name="_size"></param>
    /// <param name="_index"></param>
    /// <returns></returns>
    public Mission GetMission(int _index){
        //check if index is valid
        if (_index < 0 || _index >= m_missions.Count) return null;
        return m_missions[_index];
    }

    /// <summary>
    /// Gets the current scene index of this zone.
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSceneIndex(){
        string currentScenePath = SceneManager.GetActiveScene().path;

        // add start middle and end scenes to new list
        List<Utilities.SceneField> temp = new List<Utilities.SceneField>();
        temp.Add(m_startScene);
        temp.AddRange(m_middleScenes);
        temp.Add(m_endScene);

        // check if current scene is in list
        if (temp.Any(x => currentScenePath.Contains(x.scenePath))){
            // get index of current scene
            return temp.FindIndex(x => currentScenePath.Contains(x.scenePath));
        }
        
        return -1;
    }

    public string GetNextScenePath(){
        // add start middle and end scenes to new list
        List<Utilities.SceneField> temp = new List<Utilities.SceneField>();
        temp.Add(m_startScene);
        temp.AddRange(m_middleScenes);
        temp.Add(m_endScene);

        int currentSceneIndex = GetCurrentSceneIndex();
        if (currentSceneIndex == -1){
            return "";
        }
        // if last, return ""
        if (currentSceneIndex == temp.Count - 1){
            return "";
        }
        return temp[currentSceneIndex + 1].scenePath;
    }

    /// <summary>
    /// Gets a random boss type from this zone.
    /// </summary>
    /// <returns></returns>
    public MonsterInfo GetRandomZoneBoss(){
        // get all bosses from list
        List<MonsterInfo> bosses = new List<MonsterInfo>();
        foreach (MonsterInfo info in m_zoneMonsters){
            if (info.m_type == MonsterInfo.MonsterType.Boss){
                bosses.Add(info);
            }
        }
        // get random boss
        if (bosses.Count == 0){
            return null;
        }
        return bosses[UnityEngine.Random.Range(0, bosses.Count)];
    }

    /// <summary>
    /// Gets a random monster type from this zone.
    /// </summary>
    /// <returns></returns>
    public MonsterInfo GetRandomZoneMinion(){
        // get all minions from list
        List<MonsterInfo> minions = new List<MonsterInfo>();
        foreach (MonsterInfo info in m_zoneMonsters){
            if (info.m_type == MonsterInfo.MonsterType.Minion){
                minions.Add(info);
            }
        }
        // get random minion
        if (minions.Count == 0){
            return null;
        }
        return minions[UnityEngine.Random.Range(0, minions.Count)];
    }

    // equality operator
    public static bool operator ==(MissionZone a, MissionZone b)
    {
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        return a.m_title == b.m_title;
    }
    //inequality operator
    public static bool operator !=(MissionZone a, MissionZone b)
    {
        return !(a == b);
    }
    //override equals
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj.GetType() != typeof(MissionZone))
        {
            return false;
        }
        return this == (MissionZone)obj;
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
        [SerializeField] public List<Mission.Serializable_Mission> m_possibleMissions;
        [SerializeField] public int m_missionCount;
        [SerializeField] public List<Mission.Serializable_Mission> m_missions;
        [SerializeField] public Mission.Serializable_Mission currentMission;

        // scenes
        [SerializeField] public Utilities.SceneField m_startScene;
        [SerializeField] public Utilities.SceneField m_endScene;
        [SerializeField] public Utilities.SceneField m_bossScene;
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
            m_possibleMissions = new List<Mission.Serializable_Mission>();
            m_missions = new List<Mission.Serializable_Mission>();

            // fill with data
            foreach (Mission m in mz.m_possibleMissions)
            {
                m_possibleMissions.Add(new Mission.Serializable_Mission(m));
            }
            // missions
            m_missionCount = mz.m_missionCount;
            foreach (Mission m in mz.m_missions)
            {
                m_missions.Add(new Mission.Serializable_Mission(m));
            }

            // current mission
            currentMission = new Mission.Serializable_Mission(mz.currentMission);


            // Scenes:
            m_startScene = mz.m_startScene;
            m_endScene = mz.m_endScene;
            m_bossScene = mz.m_bossScene;
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
            // set empty lists
            mz.m_possibleMissions = new List<Mission>();
            mz.m_missions = new List<Mission>();

            // fill with data
            foreach (Mission.Serializable_Mission sm in m_possibleMissions)
            {
                mz.m_possibleMissions.Add(sm.ToMission());
            }
            // missions
            mz.m_missionCount = m_missionCount;
            foreach (Mission.Serializable_Mission sm in m_missions)
            {
                mz.m_missions.Add(sm.ToMission());
            }

            // make temp current mission
            Mission tempCurrentMission = currentMission.ToMission();

            // find temp in missions
            foreach (Mission m in mz.m_missions)
            {
                if (m == tempCurrentMission)
                {
                    mz.currentMission = m;
                    break;
                }
            }

            // Scenes:
            mz.m_startScene = m_startScene;
            mz.m_endScene = m_endScene;
            mz.m_bossScene = m_bossScene;
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