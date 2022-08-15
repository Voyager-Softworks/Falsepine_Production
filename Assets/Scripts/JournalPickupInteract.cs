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
public class JournalPickupInteract : Interactable   /// @todo Comment
{
    public enum PickupType {
        SpecificEntry,
        RandomLore,
        RandomClue,
        RandomEntry
    }

    [HideInInspector] public PickupType m_pickupType = PickupType.SpecificEntry;

    [HideInInspector] public JounralEntry m_linkedEntry;

    [HideInInspector] public bool m_specificMonster = true;
    [HideInInspector] public JounralEntry.MonsterType m_monsterType = JounralEntry.MonsterType.Bonestag;

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
            if (m_pickupType == PickupType.SpecificEntry){
                JournalManager.instance.DiscoverEntry(m_linkedEntry);
            } else {
                JournalManager.instance.DiscoverRandomEntry(GetMonsterType(), GetEntryType());
            }
        }
    }

    private JounralEntry.EntryType? GetEntryType(){
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

    private JounralEntry.MonsterType? GetMonsterType(){
        if (m_specificMonster){
            return m_monsterType;
        } else {
            return null;
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
            
            if (myScript.m_pickupType == PickupType.SpecificEntry) {
                myScript.m_linkedEntry = (JounralEntry)EditorGUILayout.ObjectField("Linked Entry", myScript.m_linkedEntry, typeof(JounralEntry), false);
            } else {
                myScript.m_specificMonster = EditorGUILayout.Toggle("Specific Monster", myScript.m_specificMonster);
                if (myScript.m_specificMonster) {
                    myScript.m_monsterType = (JounralEntry.MonsterType)EditorGUILayout.EnumPopup("Monster Type", myScript.m_monsterType);
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