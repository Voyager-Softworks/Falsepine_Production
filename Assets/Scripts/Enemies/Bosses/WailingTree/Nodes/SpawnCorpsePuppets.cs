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
            AddProperty<GameObject>("Screamer Prefab", null);
            AddProperty<float>("Spawn Radius", 10f);
            AddProperty<float>("SpawnDelay", 0.5f);
            AddProperty<float>("SpawnDelayJitter", 0.1f);
            AddProperty<int>("Puppet Spawn Count", 3);
            AddProperty<int>("Screamer Spawn Count", 1);

        }

        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if (init == false)
            {
                if (timer > GetProperty<float>("SpawnDelay"))
                {
                    Transform spawnPoint = GameObject.FindWithTag("Player").transform;
                    if (spawned.Count < GetProperty<int>("Puppet Spawn Count"))
                    {
                        GameObject corpsePuppet = Instantiate(GetProperty<GameObject>("Corpse Puppet Prefab"), GetSpawnPosition(spawnPoint.position), Quaternion.Euler(0, Random.Range(0, 360), 0));
                        spawned.Add(corpsePuppet);
                    }
                    else if (spawned.Count < GetProperty<int>("Puppet Spawn Count") + GetProperty<int>("Screamer Spawn Count"))
                    {
                        GameObject screamer = Instantiate(GetProperty<GameObject>("Screamer Prefab"), GetSpawnPosition(spawnPoint.position), Quaternion.Euler(0, Random.Range(0, 360), 0));
                        spawned.Add(screamer);
                    }
                    else
                    {
                        state = NodeData.State.Success;
                        return state;
                    }
                    timer = 0;
                }
                else
                {
                    timer += Time.deltaTime;
                }
                state = NodeData.State.Running;
            }
            else
            {

                state = NodeData.State.Success;

            }

            return state;
        }

        Vector3 GetSpawnPosition(Vector3 origin)
        {
            Vector3 spawnPosition = origin;
            float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
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
            for (int i = Mathf.FloorToInt(GetProperty<float>("Min Spawn Angle")); i < Mathf.CeilToInt(GetProperty<float>("Max Spawn Angle")); i++)
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

