using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;

namespace NodeAI
{
    
    [System.Serializable]
    public class LinkPointEvent : UnityEvent<LinkPoint> { };
    [System.Serializable]
    public enum LinkType
    {
        Input,
        Output
    }
    [System.Serializable]
    public enum LinkDataType
    {
        Float = 0,
        Int = 1,
        Bool = 2,
        Sequence = 3
    }
    [System.Serializable]
    public class LinkPoint 
    {
        public int fieldIndex = 0;
        public Rect rect;
        [SerializeField]
        public LinkType type;
        [SerializeField]
        public LinkDataType dataType;
        [SerializeField]
        public string NodeID;
        [NonSerialized]
        public Node node;
        #if UNITY_EDITOR
        [SerializeField]
        public GUIStyle style;
        #endif
        [SerializeField]
        public List<string> linkIDs;
        [SerializeField]
        public LinkPointEvent OnClickEvent;
        #if UNITY_EDITOR
        //LinkPoint
        //Parameters:
        //   string NodeID: The ID of the node this link point belongs to.
        //   LinkType type: The type of link point.
        //   LinkDataType dataType: The data type of the link point.
        //   GUIStyle style: The style of the link point.
        //   LinkPointEvent OnClickEvent: The event to be called when the link point is clicked.
        //Description:
        //   Creates a link point.
        public LinkPoint(string NodeID, LinkType type, LinkDataType dataType, GUIStyle style, LinkPointEvent OnClickEvent)
        {
            this.NodeID = NodeID;
            this.type = type;
            this.dataType = dataType;
            this.style = style;
            this.OnClickEvent = OnClickEvent;
            rect = new Rect(0, 0, 10, 20);
        }
        #endif
        //ReconnectEvents
        //Parameters:
        //   LinkPointEvent OnClickEvent: The event to be called when the link point is clicked.
        //Description:
        //   Reconnects the link point's OnClickEvent.
        public void ReconnectEvents(LinkPointEvent OnClickEvent)
        {
            this.OnClickEvent = OnClickEvent;
        }

        //ReconnectLinks
        //Parameters:
        //   AIController controller: The controller of the node this link point belongs to.
        //Description:
        //   Reconnects the link point's links.
        public void ReconnectLinks(AIController controller)
        {
            foreach (string linkID in linkIDs)
            {
                Link link = controller.GetLinkFromID(linkID);
                if (link != null)
                {
                    if(type == LinkType.Output)
                    {
                        link.input = this;
                    }
                    else
                    {
                        link.output = this;
                    }
                }
            }
        }
        #if UNITY_EDITOR
        //Draw
        //Parameters:
        //   int line: The line the link point is on.
        //   rect: The rect of the link point.
        //Description:
        //   Draws the link point.
        public void Draw(int line, Rect rect)
        {
            this.rect = new Rect(0, 0, 10, 20);
            this.rect.y = rect.y + 5 + ((EditorGUIUtility.singleLineHeight + 3) * line);

            if(type == LinkType.Input)
            {
                this.rect.x = rect.x - this.rect.width + 8;
            }
            else if(type == LinkType.Output)
            {
                this.rect.x = rect.x + rect.width - 8;
            }

            if(GUI.Button(this.rect, "", EditorStyles.miniButton))
            {
                if(OnClickEvent != null)
                {
                    //OnClick(this);
                    OnClickEvent.Invoke(this);
                }
            }
        }
        #endif

        
        
    }
    
}
