using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    [System.Serializable]
    public class NodeTree 
    {
        public NodeData rootNode;
        public Leaf rootLeaf;
        public List<NodeData> nodes;

        public void PropogateExposedProperties(List<NodeData.SerializableProperty> properties)
        {
            rootLeaf.PropogateExposedProperties(properties);
        }
        public void DrawGizmos(NodeAI_Agent agent)
        {
            rootLeaf.DrawGizmos(agent);
        }

        [System.Serializable]
        public class Leaf
        {
            public NodeData nodeData;
            public List<Leaf> children;

            public void PropogateExposedProperties(List<NodeData.SerializableProperty> properties)
            {
                foreach (var property in properties)
                {
                    List<NodeData.SerializableProperty> propertiesToChange = new List<NodeData.SerializableProperty>();
                    propertiesToChange.AddRange(nodeData.runtimeLogic.GetPropertiesWhereParamReference(property.GUID));
                    foreach (var propertyToChange in propertiesToChange)
                    {
                        propertyToChange.ivalue = property.ivalue;
                        propertyToChange.fvalue = property.fvalue;
                        propertyToChange.bvalue = property.bvalue;
                        propertyToChange.svalue = property.svalue;
                        propertyToChange.v2value = property.v2value;
                        propertyToChange.v3value = property.v3value;
                        propertyToChange.v4value = property.v4value;
                        propertyToChange.cvalue = property.cvalue;
                        propertyToChange.ovalue = property.ovalue;
                    }
                }
                foreach (var child in children)
                {
                    child.PropogateExposedProperties(properties);
                }
            }

            public void DrawGizmos(NodeAI_Agent agent)
            {
                if(nodeData.runtimeLogic)
                {
                    nodeData.runtimeLogic.DrawGizmos(agent);
                }
                foreach (var child in children)
                {
                    child.DrawGizmos(agent);
                }
            }
            
            public Leaf()
            {
                children = new List<Leaf>();
            }
        }

        public static NodeTree CreateFromNodeData(NodeData rootNodeData, List<NodeData> data)
        {
            NodeTree nodeTree = new NodeTree();
            nodeTree.rootNode = rootNodeData;
            nodeTree.nodes = data;
            nodeTree.rootLeaf = new Leaf();
            nodeTree.rootLeaf.nodeData = rootNodeData;
            nodeTree.BuildTree(nodeTree.rootLeaf);
            return nodeTree;
        }

        private void BuildTree(Leaf leaf)
        {
            foreach (var child in leaf.nodeData.childGUIDs)
            {
                var childLeaf = new Leaf();
                childLeaf.nodeData = nodes.Find(x => x.GUID == child);
                leaf.children.Add(childLeaf);
                leaf.children.Sort((x, y) => x.nodeData.position.y.CompareTo(y.nodeData.position.y));
                BuildTree(childLeaf);
            }
            
        }
    }
}
