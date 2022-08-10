using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeAI;

namespace Boss.WailingTree
{
    /// <summary>
    ///  A node responsible for Summoning corpse puppet enemies as part of the Wailing Tree Boss Battle
    /// </summary>
    public class SpawnCorpsePuppets : NodeAI.ActionBase
    {
        List<GameObject> spawned;
        bool init = false;
        float timer = 0;
        public SpawnCorpsePuppets()
        {
            AddProperty<GameObject>("Corpse Puppet Prefab", null);
            AddProperty<float>("Spawn Radius", 10f);
            AddProperty<float>("Min Spawn Angle", 0f);
            AddProperty<float>("Max Spawn Angle", 360f);
            AddProperty<float>("SpawnDelay", 0.5f);
            AddProperty<float>("SpawnDelayJitter", 0.1f);
            AddProperty<int>("Spawn Count", 10);

        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if(init == false)
            {
                if(spawned.Count < GetProperty<int>("Spawn Count") && timer > GetProperty<float>("SpawnDelay"))
                {
                    GameObject corpsePuppet = Instantiate(GetProperty<GameObject>("Corpse Puppet Prefab"), GetSpawnPosition(agent.transform.position), Quaternion.Euler(0, Random.Range(0, 360), 0));
                    spawned.Add(corpsePuppet);
                    timer = 0;
                }
                else if(spawned.Count >= GetProperty<int>("Spawn Count"))
                {
                    init = true;
                }
                else
                {
                    timer += Time.deltaTime;
                }
            }
            else
            {
                foreach(GameObject corpsePuppet in spawned)
                {
                    if(corpsePuppet.GetComponent<EnemyHealth>().hasDied)
                    {
                        Destroy(corpsePuppet, 20.0f);
                        spawned.Remove(corpsePuppet);
                        break;
                    }
                    
                }
                if(spawned.Count == 0)
                {
                    state = NodeData.State.Success;
                }
                else
                {
                    state = NodeData.State.Running;
                }
            }
            
            return state;
        }

        Vector3 GetSpawnPosition(Vector3 origin)
        {
            Vector3 spawnPosition = origin;
            float angle = Random.Range(GetProperty<float>("Min Spawn Angle"), GetProperty<float>("Max Spawn Angle")) * Mathf.Deg2Rad;
            spawnPosition += new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * Random.Range(5.0f, GetProperty<float>("Spawn Radius"));
            return spawnPosition;
        }

        public override void OnInit()
        {
            spawned = new List<GameObject>();
            init = false;
        }

        public override void DrawGizmos(NodeAI_Agent agent)
        {
            // Draw angle lines
            Gizmos.color = Color.green;
            Vector3 old = agent.transform.position;
            for (int i =  Mathf.FloorToInt(GetProperty<float>("Min Spawn Angle")); i < Mathf.CeilToInt(GetProperty<float>("Max Spawn Angle")); i++)
            {
                float angle = i * Mathf.Deg2Rad;
                Vector3 pos = agent.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * GetProperty<float>("Spawn Radius");
                Gizmos.DrawLine(old, pos);
                old = pos;
            }
            Gizmos.DrawLine(old, agent.transform.position);
        }
    }
}

