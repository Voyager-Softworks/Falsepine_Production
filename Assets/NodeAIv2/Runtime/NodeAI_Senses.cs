using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NodeAI
{
    [System.Serializable]
        public class SensoryEvent
        {
            public SensoryEvent(GameObject _source, GameObject _target, float _urgency, SenseType _senseType)
            {
                source = _source;
                target = _target;
                urgency = _urgency;
                type = _senseType;
            }
            public GameObject source{get;}
            public GameObject target{get;}
            public float distance{get => Vector3.Distance(source.transform.position, target.transform.position);}
            public Vector3 direction { get { return source.transform.position - target.transform.position; } }
            public float time{get;}
            public float age{get{return Time.time - time;}}
            public float urgency{get;}
            public float salience
            {
                get
                {
                    return urgency / (distance + age);
                }
            }
            public enum SenseType
            {
                VISUAL,
                AURAL,
                SOMATIC
            }
            public SenseType type{get;}
        }

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


    public class NodeAI_Senses : MonoBehaviour
    {
        

        
        public delegate void SensoryEventHandler(SensoryEvent e);

        public event SensoryEventHandler OnSensoryEvent;
        List<SensoryEvent> sensoryEvents = new List<SensoryEvent>();
        List<GameObject> noticedObjects = new List<GameObject>();

        public void RegisterSensoryEvent(GameObject source, GameObject target, float urgency, SensoryEvent.SenseType type)
        {
            sensoryEvents.Add(new SensoryEvent(source, target, urgency, type));
            if (OnSensoryEvent != null)
                OnSensoryEvent.Invoke(sensoryEvents.Last());
        }

        Vector3 eyesForward;
        public Transform eyesBone;
        public float maxHearingRange = 10;

        public enum BoneDirection
        {
            Forward,
            Right,
            Left,
            Up,
            Back,
            Down
        }

        public BoneDirection eyeDirection;

        public float sightDistance = 10f;
        public float sightAngle = 90f;


        // Start is called before the first frame update
        void Start()
        {
            OnSensoryEvent += DebugSense;
        }

        void DebugSense(SensoryEvent e)
        {
            Debug.DrawLine(eyesBone.position, e.source.transform.position, Color.magenta);
        }

        // Update is called once per frame
        void Update()
        {
            sensoryEvents.Sort((a, b) => a.salience.CompareTo(b.salience));
            if(sensoryEvents.Count > 20)
            {
                sensoryEvents.RemoveAt(sensoryEvents.Count - 1);
            }
            while(noticedObjects.Count > 20)
            {
                noticedObjects.RemoveAt(noticedObjects.Count - 1);
            }
            //noticedObjects.Clear();

            GameObject[] visible = GetVisibleObjects();
            foreach(GameObject o in visible)
            {
                if(!noticedObjects.Contains(o))
                {
                    noticedObjects.Add(o);
                    
                    sensoryEvents.Add(new SensoryEvent(o, gameObject, 1f, SensoryEvent.SenseType.VISUAL));
                    OnSensoryEvent?.Invoke(sensoryEvents.Last());
                }
                else
                {
                    //Move to front of list
                    int index = noticedObjects.IndexOf(o);
                    if(index != 0)
                    {
                        noticedObjects.RemoveAt(index);
                        noticedObjects.Insert(0, o);
                    }
                }
            }
            if(noticedObjects.Count >= visible.Length) noticedObjects.RemoveRange(visible.Length, noticedObjects.Count - visible.Length);
        }

        public bool CanSee(GameObject target)
        {
            return GetVisibleObjects().Contains(target);
        }

        public bool CanSeeTag(string tag)
        {
            return GetVisibleObjects().Any(o => o.CompareTag(tag));
        }

        public bool IsAwareOf(GameObject target)
        {
            if(target == null)
            {
                return false;
            }
            return noticedObjects.Contains(target);
        }

        public bool IsAwareOfTag(string tag)
        {
            return noticedObjects.Any(o => o.CompareTag(tag));
        }

        public GameObject[] GetVisibleObjects()
        {
            RaycastHit[] hits = Physics.SphereCastAll(eyesBone.position, sightDistance/2, eyesForward, sightDistance/2);
            List<GameObject> visibleObjects = new List<GameObject>();
            foreach(RaycastHit hit in hits)
            {
                if(hit.collider.gameObject == this.gameObject)
                {
                    continue;
                }
                if(hit.distance > sightDistance)
                {
                    continue;
                }
                Vector3 dir = hit.transform.position - transform.position;
                if(Vector3.Angle(eyesForward, dir) < sightAngle / 2)
                {
                    //check line of sight
                    RaycastHit[] hits2 = Physics.RaycastAll(eyesBone.position, dir, dir.magnitude);
                    bool lineOfSight = true;
                    foreach(RaycastHit hit2 in hits2)
                    {
                        if(hit.collider.gameObject == this.gameObject)
                        {
                            continue;
                        }
                        if(hit2.collider.gameObject != hit.collider.gameObject)
                        {
                            lineOfSight = false;
                            break;
                        }
                    }
                    if(lineOfSight)
                    {
                        visibleObjects.Add(hit.collider.gameObject);
                    }
                }
            }
            return visibleObjects.ToArray();
        }

        public void OnDrawGizmos()
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
            Gizmos.color = Color.yellow;
            CustomGizmos.DrawCone(eyesBone, eyesForward, sightAngle, sightDistance);
            
            foreach(GameObject o in noticedObjects)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawIcon(o.transform.position + (Vector3.up*2.0f), "d_animationvisibilitytoggleon", true, Color.magenta);
            }
        }
    }
}
