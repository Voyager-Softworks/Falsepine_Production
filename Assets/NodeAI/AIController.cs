using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NodeAI
{
public class AIController : ScriptableObject
{

    [System.Serializable]
    public class StateVars
    {
        public float radius = 0.0f;
        public string tag = "";
        public float speed = 0.0f;
        public string name = "New Custom State";
    }

    [System.Serializable]
    public class AnimatorVars
    {
        public string paramName = "";
        public bool paramBool = false;
        public float paramFloat = 0.0f;
        public int paramInt = 0;
        public enum ParamType { Bool, Float, Int, Trigger }
        public ParamType paramType = ParamType.Bool;
    }

    [System.Serializable]
    public class Parameter
    {
        public string name = "s";
        public float fvalue = 0;
        public int ivalue = 0;
        public bool bvalue = false;
        [System.Serializable]
        public enum ParameterType
        {
            Float = 0,
            Int = 1,
            Bool = 2
        }
        [SerializeField]
        public ParameterType type = ParameterType.Float;

    }
    [SerializeField]
    public List<Node> nodes;
    [SerializeField]
    public List<Link> links;
    [SerializeField]
    public List<Parameter> parameters;
    [SerializeField]
    public Dictionary<string, Node> nodeDictionary;

    [SerializeField]
    public Dictionary<string, Link> linkDictionary;

    //GetLinkFromID
    //Parameters:
    //   string ID: The ID of the link to get.
    //Description:
    //   Gets a link from its ID.
    public Link GetLinkFromID(string id)
    {
        if(linkDictionary != null)
        {
            if(linkDictionary.ContainsKey(id))
            {
                return linkDictionary[id];
            }
        }
        else
        {
            foreach(Link link in links)
            {
                if(link.ID == id)
                {
                    return link;
                }
            }
        }
        return null;
    }

    //AddLink
    //Parameters:
    //   Link link: The link to add.
    //Description:
    //   Adds a link to the list of links.
    public void AddLink(Link link)
    {
        links.Add(link);
        link.ID = GenerateRandomString(20);
        if(linkDictionary == null)
        {
            linkDictionary = new Dictionary<string, Link>();
        }
        linkDictionary.Add(link.ID, link);
    }

    //RemoveLink
    //Parameters:
    //   Link link: The link to remove.
    //Description:
    //   Removes a link from the list of links.
    public void RemoveLink(Link link)
    {
        links.Remove(link);
        if(linkDictionary != null) linkDictionary.Remove(link.ID);
    }

    //GetNodeFromID
    //Parameters:
    //   string ID: The ID of the node to get.
    //Description:
    //   Gets a node from its ID.
    public Node GetNodeFromID(string id)
    {
        if(nodeDictionary != null)
        {
            if(nodeDictionary.ContainsKey(id))
            {
                return nodeDictionary[id];
            }
        }
        else
        {
            foreach(Node node in nodes)
            {
                if(node.ID == id)
                {
                    return node;
                }
            }
        }
        return null;
    }

    //GenerateRandomString
    //Parameters:
    //   int length: The length of the string to generate.
    //Description:
    //   Generates a random string.
    public static string GenerateRandomString(int length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string result = "";
        for(int i = 0; i < length; i++)
        {
            result += chars[UnityEngine.Random.Range(0, chars.Length)];
        }
        return result;
    }

    //AddNode
    //Parameters:
    //   Node node: The node to add.
    //Description:
    //   Adds a node to the list of nodes.
    public void AddNode(Node node)
    {
        nodes.Add(node);
        if(nodeDictionary == null) nodeDictionary = new Dictionary<string, Node>();
        nodeDictionary.Add(node.ID, node);
    }

    //RemoveNode
    //Parameters:
    //   Node node: The node to remove.
    //Description:
    //   Removes a node from the list of nodes.
    public void RemoveNode(Node node)
    {
        nodes.Remove(node);
        if(nodeDictionary == null) nodeDictionary = new Dictionary<string, Node>();
        nodeDictionary.Remove(node.ID);
    }

    //ReconnectNodes
    //Description:
    //   Reconnects all nodes to each other.
    public void ReconnectNodes()
    {
        foreach(Link link in links)
        {
            if(link.input.node == null)
            {
                link.input.node = GetNodeFromID(link.input.NodeID);
            }
        }
        
    }

}
}
