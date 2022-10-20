using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class LootDrop
{
    public LootDrop()
    {
        // set name to class name
        m_name = GetType().Name;
    }

    [HideInInspector] public string m_name = "Loot";

    [Tooltip("The weight is compared against other entires to calcualte chance")] 
    public float m_weight = 1.0f;
    [Tooltip("The chance this item will be dropped (0-1) = (0%-100%)")]
    [ReadOnly] public float m_chance = 1.0f;

    public virtual void PerformDrop()
    {
        Debug.Log("Dropped " + m_name);
    }
}

[System.Serializable]
public class LootDrop_Item : LootDrop
{
    [SerializeField] public Item m_item;

    public override void PerformDrop()
    {
        if (m_item == null){
            Debug.LogError("No item to drop");
            return;
        }

        Inventory home = InventoryManager.instance.GetInventory("home");
        if (home != null)
        {
            if (home.TryAddItemToInventory(m_item.CreateInstance()) == null)
            {
                // message
                MessageManager.instance?.AddMessage("Reward: " + m_item.m_displayName, "bag");
                // notify
                NotificationManager.instance?.AddIconAtPlayer("bag");
                // sound
                UIAudioManager.instance?.equipSound.Play();
            }
        }
    }
}

[System.Serializable]
public class LootDrop_Money : LootDrop
{
    // min and max amount of money to drop
    [SerializeField] public int m_minAmount = 1;
    [SerializeField] public int m_maxAmount = 1;

    public override void PerformDrop()
    {
        int amount = Random.Range(m_minAmount, m_maxAmount + 1);
        
        // add money to player
        EconomyManager.instance.AddMoney(amount);

        // send message
        MessageManager.instance?.AddMessage("Reward: +" + amount + " silver", "silver");
        // notify
        NotificationManager.instance?.AddIconAtPlayer("silver");
    }

}

[CreateAssetMenu(fileName = "LootPool", menuName = "Loot/LootPool")]
public class LootPool : ScriptableObject
{
    // SerializeReference so that we can have different types of loot drops
    [SerializeReference] public List<LootDrop> m_lootDrops = new List<LootDrop>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        // update chances
        UpdateChances();
    }

    private void UpdateChances()
    {
        float totalWeight = 0;
        foreach (var lootDrop in m_lootDrops)
        {
            totalWeight += lootDrop.m_weight;
        }
        foreach (var lootDrop in m_lootDrops)
        {
            lootDrop.m_chance = lootDrop.m_weight / totalWeight;
        }
    }

    public LootDrop GetRandomDrop(){
        // update chances to ensure total weight is 1.0
        UpdateChances();

        // get random value between 0 and 1
        float random = Random.Range(0.0f, 1.0f);
        float currentChance = 0;
        foreach (var lootDrop in m_lootDrops)
        {
            currentChance += lootDrop.m_chance;
            // if the random value is less than the current chance, return this loot drop
            if (random <= currentChance)
            {
                return lootDrop;
            }
        }
        // if we get here, something went wrong
        Debug.LogError("No loot drop found, something went wrong, please check this function", this);
        return null;
    }

    // custom editor for the loot pool
#if UNITY_EDITOR
    [CustomEditor(typeof(LootPool))]
    public class LootPoolEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            LootPool lootPool = (LootPool)target;

            List<System.Type> types = GetDropTypes();

            // add a button to add a new loot drop
            if (GUILayout.Button("Add Drop"))
            {
                GenericMenu menu = new GenericMenu();

                // add a menu item for each type
                foreach (System.Type type in types)
                {
                    menu.AddItem(new GUIContent(type.Name), false, () => { lootPool.m_lootDrops.Add((LootDrop)System.Activator.CreateInstance(type)); });
                }

                menu.ShowAsContext();
            }
        }

        /// <summary>
        /// Gets the drop types using reflection
        /// </summary>
        /// <returns></returns>
        public List<System.Type> GetDropTypes()
        {
            List<System.Type> dropTypes = new List<System.Type>();

            // get all the types in the assembly
            System.Type[] types = Assembly.GetAssembly(typeof(LootPool)).GetTypes();

            // loop through all the types
            foreach (var type in types)
            {
                // if the type is a subclass of loot drop
                if (type.IsSubclassOf(typeof(LootDrop)))
                {
                    // add it to the list
                    dropTypes.Add(type);
                }
            }

            return dropTypes;
        }
    }
#endif
}