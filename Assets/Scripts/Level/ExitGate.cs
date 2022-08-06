using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExitGate : MonoBehaviour
{
    public enum GateDestination
    {
        Next,
        //Previous,
        Town,
        //Boss,
    }

    public GateDestination m_destination;

    public List<LevelCondition> m_conditions;

    public float m_checkInterval = 1.5f;
    private float m_checkTimer = 0.0f;
    private bool m_unlocked = false;

    [Header("Refs")]
    public GameObject lockedObject;
    public GameObject unlockedObject;
    public GameObject unlockSound;

    // Start is called before the first frame update
    void Start()
    {
        Lock();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_unlocked) return;

        // periodically check if all conditions are met
        m_checkTimer += Time.deltaTime;
        if (m_checkTimer >= m_checkInterval)
        {
            m_checkTimer = 0.0f;
            if (CheckConditions()){
                Unlock();
            }
        }
    }

    public bool CheckConditions()
    {
        bool allConditionsMet = true;
        foreach (LevelCondition condition in m_conditions)
        {
            // skip entries that are not enabled
            if (!condition.enabled || condition.gameObject.activeSelf == false){
                continue;
            }

            if (!condition.isComplete)
            {
                allConditionsMet = false;
                break;
            }
        }
        if (!m_unlocked && allConditionsMet){
            Unlock();
        }

        return allConditionsMet;
    }

    public void Unlock()
    {
        m_unlocked = true;

        lockedObject.SetActive(false);
        unlockedObject.SetActive(true);

        if (unlockSound != null){
            Instantiate(unlockSound, transform.position, Quaternion.identity);
        }
    }

    public void Lock()
    {
        m_unlocked = false;
        lockedObject.SetActive(true);
        unlockedObject.SetActive(false);
    }

    public void TryGoToDestination()
    {
        if (!CheckConditions()){
            return;
        }

        switch (m_destination)
        {
            case GateDestination.Next:
                MissionManager.instance?.LoadNextLesserScene();
                break;
            case GateDestination.Town:
                LevelController.LoadTown();
                break;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.root.tag == "Player") {
            TryGoToDestination();
        }
    }

    // Editor
    #if UNITY_EDITOR
    [CustomEditor(typeof(ExitGate))]
    public class ExitGateEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ExitGate exitGate = target as ExitGate;
            if (exitGate == null) return;
            base.OnInspectorGUI();

            //space
            EditorGUILayout.Space();

            if (GUILayout.Button("Set Up")) {
                SetUp();
            }
        }

        public void SetUp(){
            // add all conditions in the scene to the list
            ExitGate exitGate = target as ExitGate;
            if (exitGate == null) return;

            // get KillAll_Condition if it exists, otherwise add it
            LevelCondition killAllCondition = exitGate.GetComponent<KillAll_Condition>();
            if (killAllCondition == null)
            {
                killAllCondition = exitGate.gameObject.AddComponent<KillAll_Condition>();
            }

            exitGate.m_conditions = new List<LevelCondition>();
            foreach (LevelCondition condition in GameObject.FindObjectsOfType<LevelCondition>())
            {
                // if it already exists, skip it
                if (exitGate.m_conditions.Contains(condition)) continue;

                exitGate.m_conditions.Add(condition);
            }

            // get DebugExitGate if it exists, and remove it
            DebugExitGate debugExitGate = exitGate.GetComponent<DebugExitGate>();
            if (debugExitGate != null)
            {
                DestroyImmediate(debugExitGate);
            }

            // get parent of this object
            Transform parent = exitGate.transform.parent;

            // get boundy child
            Transform boundy = parent.Find("boundryExit");
            if (boundy != null)
            {
                exitGate.lockedObject = boundy.gameObject;
            }

            // get light child
            Transform light = parent.Find("exitLight");
            if (light != null)
            {
                exitGate.unlockedObject = light.gameObject;
            }

            // save
            EditorUtility.SetDirty(exitGate);
        }
    }
    #endif
}
