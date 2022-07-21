/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: SeekCoverFrom.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NodeAI
{
    /// <summary>
    /// A Node that seeks cover from a target.
    /// </summary>
    /// <para>
    /// This node is part of the Demo for NodeAI.
    /// </para>
    public class SeekCoverFrom : ActionBase
    {
        NavMeshAgent navAgent; ///< The NavMeshAgent of the agent.
        GameObject coverFrom; ///< The target to seek cover from.
        Vector3 currCoverPoint; ///< The current cover point.

        bool initialized = false; ///< Whether or not the node has been initialized.
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public SeekCoverFrom()
        {
            AddProperty<GameObject>("Object", null);
            AddProperty<string>("CoverLayer", "Default");
            AddProperty<float>("Distance", 0f);
            AddProperty<float>("Min Distance from Object", 0f);
            AddProperty<float>("Max Distance from Object", 0f);
            tooltip = "Seek cover from a target object";
        }

        /// <summary>
        /// Initializes the node.
        /// </summary>
        public override void OnInit()
        {
            initialized = false;
        }

        /// <summary>
        /// Evaluates the node.
        /// </summary>
        /// <param name="agent">The agent to evaluate the node for.</param>
        /// <param name="current">The current leaf of the node tree.</param>
        /// <returns>The state of the node.</returns>
        public override NodeData.State Eval(NodeAI_Agent agent, NodeTree.Leaf current)
        {
            if(!navAgent)
            {
                navAgent = agent.GetComponent<NavMeshAgent>();
                if(!navAgent)
                {
                    Debug.LogError("No NavMeshAgent found on " + agent.gameObject.name);
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
            }

            if(!coverFrom)
            {
                coverFrom = GetProperty<GameObject>("Object");
                if(!coverFrom)
                {
                    Debug.LogError("No cover object found");
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
            }

            if(!initialized)
            {
                initialized = true;
                currCoverPoint = GetClosestCoverPoint(agent, coverFrom);
                if(currCoverPoint != Vector3.zero)
                {
                    navAgent.SetDestination(currCoverPoint);
                }
                else
                {
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }
                
                
                navAgent.stoppingDistance = 0.0f;
                navAgent.isStopped = false;
            }

            currCoverPoint = GetClosestCoverPoint(agent, coverFrom);
                if(currCoverPoint != Vector3.zero)
                {
                    navAgent.SetDestination(currCoverPoint);
                }
                else
                {
                    state = NodeData.State.Failure;
                    return NodeData.State.Failure;
                }

            if(Vector3.Distance(agent.transform.position, currCoverPoint) <= navAgent.stoppingDistance + 0.1f)
            {
                navAgent.isStopped = true;
                state = NodeData.State.Success;
                return NodeData.State.Success;
            }

            state = NodeData.State.Running;
            return NodeData.State.Running;

        }

        /// <summary>
        /// Gets the closest cover point to the agent.
        /// </summary>
        /// <param name="agent">The current agent.</param>
        /// <param name="coverFrom">The Object the agent is taking cover from.</param>
        /// <returns></returns>
        Vector3 GetClosestCoverPoint(NodeAI_Agent agent, GameObject coverFrom)
        {
            RaycastHit[] hits = Physics.SphereCastAll(agent.transform.position, GetProperty<float>("Distance"), agent.transform.forward, GetProperty<float>("Distance"), LayerMask.GetMask(GetProperty<string>("CoverLayer")));
            List<Vector3> coverPoints = new List<Vector3>();
            // For each collider, query a number of surrounding points to see if there is a navmesh edge nearby pointing away from the cover object
            foreach(RaycastHit hit in hits)
            {
                Vector3 hideDirection = (hit.collider.transform.position - coverFrom.transform.position).normalized;
                if(Vector3.Distance(hit.collider.transform.position, coverFrom.transform.position) < GetProperty<float>("Min Distance from Object") || Vector3.Distance(hit.collider.transform.position, coverFrom.transform.position) > GetProperty<float>("Max Distance from Object"))
                {
                    continue;
                }
                if(hit.collider.gameObject.transform.root.gameObject.GetComponent<Cover>())
                {
                    coverPoints.AddRange(hit.collider.gameObject.transform.root.gameObject.GetComponent<Cover>().GetAvailableCoverPoints(hideDirection));
                }


            
            }
            coverPoints.Sort(delegate(Vector3 a, Vector3 b)
            {
                return Vector3.Distance(a, agent.transform.position).CompareTo(Vector3.Distance(b, agent.transform.position));
            });
            if(coverPoints.Count > 0)
            {
                return coverPoints[0];
            }
            else
            {
                return Vector3.zero;
            }

        }

        

    }
}
