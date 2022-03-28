using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;

namespace NodeAI
{
    [System.Serializable]
    public class LinkEvent : UnityEvent<Link> { };
    [System.Serializable]
    public class Link 
    {
        [SerializeField]
        public LinkPoint input;
        [SerializeField]
        public string ID;
        [SerializeField]
        public LinkPoint output;
        //public Action<Link> OnClick;
        [SerializeField]
        public LinkEvent OnClickEvent;

        //Link
        //Parameters:
        //   LinkPoint input: The input link point.
        //   LinkPoint output: The output link point.
        //   LinkEvent OnClickEvent: The event to be called when the link is clicked.
        //Description:
        //   Creates a link.
        public Link(LinkPoint input, LinkPoint output, LinkEvent OnClickEvent)
        {
            this.input = input;
            this.output = output;
            this.OnClickEvent = OnClickEvent;
        }
        
        //RelinkEvents
        //Parameters:
        //    LinkEvent OnClickEvent: The event to be called when the link is clicked.
        //Description:
        //    Relinks the link's OnClickEvent.
        public void RelinkEvents(LinkEvent OnClickEvent)
        {
            this.OnClickEvent = OnClickEvent;
        }
    
        #if UNITY_EDITOR
        //Draw
        //Description:
        //    Draws the link.
        public void Draw()
        {
            Handles.DrawBezier(
                input.rect.center,
                output.rect.center,
                input.rect.center - Vector2.left * 50f,
                output.rect.center + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            if(Handles.Button((input.rect.center + output.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
            {
                if(OnClickEvent != null)
                {
                    OnClickEvent.Invoke(this);
                }
            }

            if(input == null || output == null)
            {
                OnClickEvent.Invoke(this);
            }
        }
        #endif

        
    

        
    }
}
