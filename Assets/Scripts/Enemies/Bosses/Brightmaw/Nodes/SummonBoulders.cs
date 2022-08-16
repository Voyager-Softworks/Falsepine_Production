using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace Boss.Brightmaw
{
    /// <summary>
    ///  A node responsible for summoning boulders around the Brightmaw boss.
    /// </summary>
    public class SummonBoulders : NodeAI.ActionBase
    {
        public List<GameObject> m_boulders = new List<GameObject>();
        bool spawned = false;
        public SummonBoulders()
        {
            tooltip = "Summons boulders";
            AddProperty<int>("Number of boulders", 1);
            AddProperty<float>("Boulder Speed", 1.0f);
            AddProperty<float>("Min Radius", 0.0f);
            AddProperty<float>("Max Radius", 0.0f);
            AddProperty<GameObject>("Boulder Prefab", null);
        }

        /// <summary>
        /// > This function is called when the game starts
        /// </summary>
        public override void OnInit()
        {
            spawned = false;
            m_boulders.Clear();
        }


        /// <summary>
        /// For each boulder, spawn a boulder at a random position within a radius of the agent, and
        /// give it a random velocity
        /// </summary>
        /// <param name="NodeAI_Agent">The agent that is running the tree.</param>
        /// <param name="current">The current leaf that is being evaluated.</param>
        /// <returns>
        /// The state of the node.
        /// </returns>
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (!spawned)
            {
                for (int i = 0; i < GetProperty<int>("Number of boulders"); i++)
                {
                    GameObject boulder = Instantiate(GetProperty<GameObject>("Boulder Prefab"), agent.transform.position, Quaternion.identity);
                    Vector3 direction = Random.insideUnitSphere;
                    direction.y = 0;
                    direction.Normalize();
                    boulder.transform.position += direction * Random.Range(GetProperty<float>("Min Radius"), GetProperty<float>("Max Radius"));
                    boulder.GetComponent<Rigidbody>().velocity = Vector3.up * GetProperty<float>("Boulder Speed");
                    m_boulders.Add(boulder);
                }
                spawned = true;
            }
            state = NodeData.State.Success;
            return state;
        }
    }
}

