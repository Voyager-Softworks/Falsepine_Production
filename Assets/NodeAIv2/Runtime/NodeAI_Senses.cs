/*
 * Bachelor of Software Engineering
 * Media Design School
 * Auckland
 * New Zealand
 * 
 * (c) 2022 Media Design School
 * 
 * File Name: NodeAI_Senses.cs
 * Description: 
 * Author: Nerys Thamm
 * Mail: nerysthamm@gmail.com
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NodeAI
{
    /// <summary>
    /// An Event object containing sensory data.
    /// </summary>
    [System.Serializable]
    public class SensoryEvent
    {
        public SensoryEvent(GameObject _source, GameObject _target, float _urgency, SenseType _senseType) ///< Constructor.
        {
            source = _source;
            target = _target;
            urgency = _urgency;
            type = _senseType;
        }
        public GameObject source { get; } ///< The source of the event.
        public GameObject target { get; } ///< The target of the event.
        public float distance { get => Vector3.Distance(source.transform.position, target.transform.position); } ///< The distance between the source and target.
        public Vector3 direction { get { return source.transform.position - target.transform.position; } } ///< The direction between the source and target.
        public float time { get; } ///< The time the event was created.
        public float age { get { return Time.time - time; } } ///< The time since the event was created.
        public float urgency { get; } ///< The urgency of the event.
        public float salience ///< The salience of the event.
        {
            get
            {
                return urgency / (distance + age);
            }
        }
        public enum SenseType ///< The type of the event.
        {
            VISUAL, ///< Events caused by the Agent percieving visual (seeing) information.
            AURAL, ///< Events caused by the Agent percieving auditory (hearing) information.
            SOMATIC ///< Events caused by the Agent percieving somatic (feeling) information.
        }
        public SenseType type { get; } ///< The type of the event.
    }
    /// <summary>
    /// A comparer for SensoryEvents.
    /// </summary>
    public class SensoryEventComparer : IComparer<SensoryEvent>
    {
        public int Compare(SensoryEvent x, SensoryEvent y)
        {
            if (x.salience > y.salience)
                return -1;
            else if (x.salience < y.salience)
                return 1;
            else
                return 0;
        }
    }



    /// <summary>
    ///  A class which gives a NodeAI_Agent sensory awareness.
    /// </summary>
    /// <para>
    /// This class is responsible for giving the NodeAI_Agent sensory awareness of its environment and things that happen to it.
    /// When placed on a GameObject with a NodeAI_Agent component, the NodeAI_Agent will be able to perceive things in the environment.
    /// The Agent will automatically be able to percieve things visually, but auditory and somatic sensory data must be added manually.
    /// </para>
    /// <para>
    /// For example, the following features could be implemented using auditory or somatic sensory events:
    /// </para>
    /// <list type="bullet">
    ///     <item>
    ///        <term> Hearing the player's footsteps </term>
    ///       <description> The script on the player used to play footstep sounds could register auditory events on any nearby Agents, allowing them to become aware of the player if footsteps are heard.
    ///      </description>
    ///    </item>
    ///   <item>
    ///      <term> Touching a hazard </term>
    ///     <description> A Hazardous area could register a somatic event on any agents which collide with it, and the agent could react to it accordingly by moving away from the hazard.
    ///   </description>
    /// </item>
    /// </list>
    public class NodeAI_Senses : MonoBehaviour
    {
        public delegate void SensoryEventHandler(SensoryEvent e); ///< The delegate for handling sensory events.

        public event SensoryEventHandler OnSensoryEvent; ///< The event for handling sensory events.
        List<SensoryEvent> sensoryEvents = new List<SensoryEvent>(); ///< The list of sensory events.
        List<GameObject> noticedObjects = new List<GameObject>(); ///< The list of objects which the Agent has noticed.

        /// <summary>
        /// Registers a sensory event with the Agent.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="target">The target of the event.</param>
        /// <param name="urgency">The urgency of the event.</param>
        /// <param name="senseType">The type of the event.</param>
        /// <para>
        /// The source and target of the event must be GameObjects.
        /// </para>
        public void RegisterSensoryEvent(GameObject source, GameObject target, float urgency, SensoryEvent.SenseType type) ///< Registers a sensory event.
        {
            sensoryEvents.Add(new SensoryEvent(source, target, urgency, type));
            if (OnSensoryEvent != null)
                OnSensoryEvent.Invoke(sensoryEvents.Last());
        }

        Vector3 eyesForward; ///< The direction representing the forward direction of the Agent's eyes. This is used to correct the position of the vision cone when the bone on the model is oriented strangely.
        public Transform eyesBone; ///< The transform of the bone which is used to represent the Agent's eyes.
        public float maxHearingRange = 10; ///< The maximum range of the Agent's hearing.

        public enum BoneDirection /// The direction of the bone which is used to represent the Agent's eyes.
        {
            Forward, ///< The bone is oriented forward.
            Right, ///< The bone is oriented to the right.
            Left, ///< The bone is oriented to the left.
            Up, ///< The bone is oriented upwards.
            Back, ///< The bone is oriented backwards.
            Down ///< The bone is oriented downwards.
        }

        public BoneDirection eyeDirection; ///< The direction of the bone which is used to represent the Agent's eyes.

        public float sightDistance = 10f; ///< The distance of the Agent's vision cone.
        public float sightAngle = 90f; ///< The angle of the Agent's vision cone.

        public LayerMask sightMask; ///< The layer mask of the layers which the Agent's vision cone can see.


        // Start is called before the first frame update
        void Start()
        {
            OnSensoryEvent += DebugSense;
        }

        /// <summary>
        ///  A debug function to investigate the sensory events.
        /// </summary>
        /// <param name="e">The SensoryEvent</param>
        void DebugSense(SensoryEvent e)
        {
            Debug.DrawLine(eyesBone.position, e.source.transform.position, Color.magenta);
        }

        // Update is called once per frame
        void Update()
        {
            switch (eyeDirection)
            {
                case BoneDirection.Forward:
                    eyesForward = eyesBone.forward;
                    break;
                case BoneDirection.Right:
                    eyesForward = eyesBone.right;
                    break;
                case BoneDirection.Up:
                    eyesForward = eyesBone.up;
                    break;
                case BoneDirection.Back:
                    eyesForward = -eyesBone.forward;
                    break;
            }
            sensoryEvents.Sort((a, b) => a.salience.CompareTo(b.salience));
            if (sensoryEvents.Count > 20)
            {
                sensoryEvents.RemoveAt(sensoryEvents.Count - 1);
            }
            while (noticedObjects.Count > 20)
            {
                noticedObjects.RemoveAt(noticedObjects.Count - 1);
            }
            //noticedObjects.Clear();



            GameObject[] visible = GetVisibleObjects();
            foreach (GameObject o in visible)
            {
                if (!noticedObjects.Contains(o))
                {
                    sensoryEvents.Add(new SensoryEvent(o, gameObject, 1f, SensoryEvent.SenseType.VISUAL));
                    OnSensoryEvent?.Invoke(sensoryEvents.Last());
                }
                else
                {
                    //Move to front of list
                    int index = noticedObjects.IndexOf(o);
                    if (index != 0)
                    {
                        noticedObjects.RemoveAt(index);
                        noticedObjects.Insert(0, o);
                    }
                }
            }
            if (noticedObjects.Count >= visible.Length) noticedObjects.RemoveRange(visible.Length, noticedObjects.Count - visible.Length);
            sensoryEvents.ForEach(e =>
            {
                if (e.salience > 0 && e.age > 0.2f)
                {
                    if (!noticedObjects.Contains(e.source))
                    {
                        noticedObjects.Insert(0, e.source);
                    }
                }
            });
        }


        /// <summary>
        /// Returns true if the Agent can see the target.
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <returns>True if the Agent can see the target.</returns>
        public bool CanSee(GameObject target)
        {
            return GetVisibleObjects().Contains(target);
        }

        /// <summary>
        /// Checks if the Agent can see an object with the given tag.
        /// </summary>
        /// <param name="tag">The tag of the object to check.</param>
        /// <returns>True if the Agent can see an object with the given tag.</returns>
        public bool CanSeeTag(string tag) ///< Returns true if the Agent can see an object with the given tag.
        {
            return GetVisibleObjects().Any(o => o.CompareTag(tag));
        }

        /// <summary>
        /// Check if the Agent is currently aware of the target.
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <returns>True if the Agent is currently aware of the target.</returns>
        public bool IsAwareOf(GameObject target) ///< Returns true if the Agent is aware of the target.
        {
            if (target == null)
            {
                return false;
            }
            return noticedObjects.Contains(target);
        }

        /// <summary>
        /// Checks if the Agent is currently aware of an object with the given tag.
        /// </summary>
        /// <param name="tag">The tag of the object to check.</param>
        /// <returns>True if the Agent is currently aware of an object with the given tag.</returns>
        public bool IsAwareOfTag(string tag) ///< Returns true if the Agent is aware of an object with the given tag.
        {
            return noticedObjects.Any(o => o.CompareTag(tag));
        }

        /// <summary>
        /// Returns the list of objects which the Agent can see with the given tag.
        /// </summary>
        /// <param name="tag">The tag of the objects to check.</param>
        /// <returns>The list of objects which the Agent can see with the given tag.</returns>
        public GameObject GetAwareObjectWithTag(string tag) ///< Returns the first object with the given tag which the Agent is aware of.
        {
            return noticedObjects.FirstOrDefault(o => o.CompareTag(tag));
        }

        /// <summary>
        /// Returns the list of objects which the Agent can see.
        /// </summary>
        /// <returns>The list of objects which the Agent can see.</returns>
        public GameObject[] GetVisibleObjects() ///< Returns all objects which the Agent can see.
        {
            RaycastHit[] hits = Physics.SphereCastAll(eyesBone.position, sightDistance / 2, eyesForward, sightDistance / 2, sightMask);
            List<GameObject> visibleObjects = new List<GameObject>();
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    continue;
                }
                if (hit.distance > sightDistance)
                {
                    continue;
                }
                Vector3 dir = hit.transform.position - transform.position;
                if (Vector3.Angle(eyesForward, dir) < sightAngle / 2)
                {
                    //check line of sight
                    RaycastHit[] hits2 = Physics.RaycastAll(eyesBone.position, dir, dir.magnitude);
                    bool lineOfSight = true;
                    foreach (RaycastHit hit2 in hits2)
                    {
                        if (hit.collider.gameObject == this.gameObject)
                        {
                            continue;
                        }
                        if (hit2.collider.gameObject != hit.collider.gameObject)
                        {
                            lineOfSight = false;
                            break;
                        }
                    }
                    if (lineOfSight)
                    {
                        visibleObjects.Add(hit.collider.gameObject);
                    }
                }
            }
            return visibleObjects.ToArray();
        }

        /// <summary>
        /// Draws the vision cone of the Agent.
        /// </summary>
        public void OnDrawGizmosSelected() ///< Draws the vision cone gizmo
        {

            Gizmos.color = Color.yellow;
            if (eyesBone) CustomGizmos.DrawCone(eyesBone, eyesForward, sightAngle, sightDistance);

            foreach (GameObject o in noticedObjects)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawIcon(o.transform.position + (Vector3.up * 2.0f), "d_animationvisibilitytoggleon", true, Color.magenta);
            }
            if (sensoryEvents.Count > 0)
            {
                for (int i = 0; i < sensoryEvents.Count; i++)
                {
                    Gizmos.color = Color.magenta * (1.0f - (i / (float)sensoryEvents.Count));
                    Gizmos.DrawLine(eyesBone.position, sensoryEvents[i].source.transform.position);
                }
            }
        }

    }
}
