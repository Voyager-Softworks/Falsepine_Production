using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

//[REQUIRED]
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// An example of a custom item type that can be used in the item database.
/// </summary>
//Make sure to set the [Serializable] attribute on this class, so that it can be saved!
[Serializable]
public class Equipment : Item
{
    public interface Useable
    {
        bool TryUse(Item _item);
    }

    // make sure to set variables as serialized fields in the inspector, so that they can be saved!

    public override string GetTypeDisplayName(){
        return "Tool";
    }

    [SerializeField] public GameObject m_equipmentPrefab;
    [SerializeField] public float m_useDelay = 0.75f;
    public float m_useDelayTimer = 0;

    /// <summary>
    /// [REQUIRED] Used to create a copy of the item. Make sure to set any unique values here!
    /// </summary>
    public override Item CreateInstance()
    {
        // [REQUIRED] Create base item
        Equipment newItem = (Equipment)base.CreateInstance();

        // Setting unique values here:
        // example valye
        newItem.m_equipmentPrefab = m_equipmentPrefab;
        newItem.m_useDelay = m_useDelay;

        return newItem;
    }

    // update
    public override void ManualUpdate(GameObject _owner)
    {
        base.ManualUpdate(_owner);

        // update all timers and ensure they are never negative:
        m_useDelayTimer = Mathf.Max(0, m_useDelayTimer - Time.deltaTime);
    }

    public bool UseEquipment(Transform _throwTransform, Vector3 _direction, GameObject _owner)
    {
        GameObject newEquipment = Instantiate(m_equipmentPrefab, _throwTransform.position, Quaternion.identity);
        if (newEquipment.GetComponent<ItemThrow>() != null){
            newEquipment.GetComponent<ItemThrow>().TossPrefab(_throwTransform, _direction, _owner);
            
            WasUsed();

            return true;
        }

        if (newEquipment.GetComponent<Useable>() != null){
            if (newEquipment.GetComponent<Useable>().TryUse(this)){
                WasUsed();
                // set parent to owner
                newEquipment.transform.SetParent(_owner.transform);

                // Destroy timer
                Destroy(newEquipment, 15f);

                return true;
            }
            else {
                Destroy(newEquipment);
            }
        }

        return false;
    }

    private void WasUsed(){
        currentStackSize -= 1;

        m_useDelayTimer = m_useDelay;
        
        if (currentStackSize <= 0){
            // remove item from inventory
            Inventory inv = InventoryManager.instance.GetInventory("player");
            int index = inv.GetItemIndex(this);
            if (index != -1){
                inv.RemoveItemFromInventory(index);
            }
        }
    }
    
    //Custom editor for this class
    #if UNITY_EDITOR
    [CustomEditor(typeof(Equipment))]
    public class EquipmentEditor : ItemEditor
    {
        public override void OnInspectorGUI()
        {
            // [REQUIRED] draw base editor (Item in this case)
            base.OnInspectorGUI();
            // [REQUIRED] get the editor target
            Equipment item = (Equipment)target;

            // red box for weapon stats
            GUI.backgroundColor = Color.red;
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;
            // bold center text
            GUILayout.Label("Equipment Stats", CustomEditorStuff.center_bold_label);

            // Your custom values here
            item.m_equipmentPrefab = (GameObject)EditorGUILayout.ObjectField("Equipment Prefab", item.m_equipmentPrefab, typeof(GameObject), false);
            item.m_useDelay = EditorGUILayout.FloatField("Use Delay", item.m_useDelay);

            //end red box
            GUILayout.EndVertical();

            // on change, save the changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(item);
            }
        }
    }
    #endif
	
	
}
