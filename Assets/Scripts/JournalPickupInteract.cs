using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// An interactable object that contributes lore ore clues to the journal.
/// @todo make an option to get current zone's boss automatically as the monster type
/// - Possibly even allow selecting a specific zone? (and get it automatically?)
/// </summary>
public class JournalPickupInteract : Interactable
{
    public enum PickupType {
        SpecificEntry,
        RandomLore,
        RandomClue,
        RandomEntry
    }

    [Tooltip("What kind of entry should be given when picked up")]
    [HideInInspector] public PickupType m_pickupType = PickupType.SpecificEntry;
    [Tooltip("The specific entry to give when picked up")]
    [HideInInspector] public JounralEntry m_linkedEntry;

    [Tooltip("Whether or not to give an entry from a specific monster or not")]
    [HideInInspector] public bool m_specificMonster = false;
    [Tooltip("The specific monster to give an entry from")]
    [HideInInspector] public MonsterInfo m_monster = null;

    [Tooltip("If this should give a random entry from the current zones boss")]
    [HideInInspector] public bool m_currentZoneBoss = false;

    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
    }

    override public void DoInteract()
    {
        base.DoInteract();
        
        if (JournalManager.instance){
            bool success = false;
            if (m_pickupType == PickupType.SpecificEntry){
                success = JournalManager.instance.DiscoverEntry(m_linkedEntry);
            } else {
                success = JournalManager.instance.DiscoverRandomEntry(GetMonsterInfo(), GetEntryType());
            }

            if (!success){
                // add money
                //@todo make this amount adjustable
                EconomyManager.instance.AddMoney(10);
            }
        }
    }

    /// <summary>
    /// Gets the entry type based on user selection in inspector.
    /// </summary>
    /// <returns></returns>
    public JounralEntry.EntryType? GetEntryType(){
        switch (m_pickupType){
            case PickupType.SpecificEntry:
                return m_linkedEntry.m_entryType;
            case PickupType.RandomLore:
                return JounralEntry.EntryType.Lore;
            case PickupType.RandomClue:
                return JounralEntry.EntryType.Clue;
            case PickupType.RandomEntry:
                return null;
            default:
                return null;
        }
    }

    /// <summary>
    /// Gets the monster type based on user selection in inspector.
    /// </summary>
    /// <returns></returns>
    private MonsterInfo GetMonsterInfo(){
        if (m_specificMonster){
            return m_monster;
        } else {
            if (m_currentZoneBoss){
                return MissionManager.instance?.GetCurrentZone()?.GetRandomZoneBoss();
            } else {
                return null;
            }
        }
    }

    // custom editor
    #if UNITY_EDITOR
    [CustomEditor(typeof(JournalPickupInteract))]
    public class JournalPickupInteractEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            JournalPickupInteract myScript = (JournalPickupInteract)target;

            //space
            EditorGUILayout.Space();

            // header
            GUILayout.Label("Journal Pickup", EditorStyles.boldLabel);

            myScript.m_pickupType = (PickupType)EditorGUILayout.EnumPopup("Pickup Type", myScript.m_pickupType);
            
            if (myScript.m_pickupType == PickupType.SpecificEntry){
                myScript.m_linkedEntry = (JounralEntry)EditorGUILayout.ObjectField("Entry", myScript.m_linkedEntry, typeof(JounralEntry), false);
            } else {
                myScript.m_specificMonster = EditorGUILayout.Toggle("Specific Monster", myScript.m_specificMonster);
                if (myScript.m_specificMonster){
                    myScript.m_monster = (MonsterInfo)EditorGUILayout.ObjectField("Monster", myScript.m_monster, typeof(MonsterInfo), false);
                } else {
                    myScript.m_currentZoneBoss = EditorGUILayout.Toggle("Current Zone Boss", myScript.m_currentZoneBoss);
                }
            }

            //on change, save
            if (GUI.changed)
            {
                EditorUtility.SetDirty(myScript);
            }
        }
    }
    #endif
}