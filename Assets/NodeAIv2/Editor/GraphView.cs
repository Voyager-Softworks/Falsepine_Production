using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System.Linq;

namespace NodeAI
{
    public class GraphView : UnityEditor.Experimental.GraphView.GraphView
    {
        private SearchWindow searchWindow;
        private BlackboardSearchWindow blackboardSearchWindow;
        private QuerySearchWindow querySearchWindow;
        public List<NodeData.Property> exposedProperties = new List<NodeData.Property>();
        public Blackboard blackboard;

        public Node currHoveredNode;

        public GraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("GraphStyle"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new SelectionDropper());
            
            RegisterCallback<MouseDownEvent>(HandleMouseDownEvents);
            RegisterCallback<MouseOverEvent>(HandleMouseOverEvent);
            RegisterCallback<MouseLeaveEvent>(HandleMouseLeaveEvent);
            RegisterCallback<DragPerformEvent>(HandleDragEndEvent);
            

            var g = new Group();
            

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
            searchWindow = ScriptableObject.CreateInstance<SearchWindow>();
            searchWindow.Init(this);
            blackboardSearchWindow = ScriptableObject.CreateInstance<BlackboardSearchWindow>();
            blackboardSearchWindow.Init(this);
            querySearchWindow = ScriptableObject.CreateInstance<QuerySearchWindow>();
            querySearchWindow.Init(this);
            
        }

        private void AddSearchWindow(Vector2 position)
        {
            
            UnityEditor.Experimental.GraphView.SearchWindow.Open(new SearchWindowContext(position), searchWindow);

        }

        public void AddBlackboardSearchWindow(Vector2 position)
        {
            UnityEditor.Experimental.GraphView.SearchWindow.Open(new SearchWindowContext(position), blackboardSearchWindow);
        }

        private void GroupNodes()
        {
            if (selection.Count() > 1)
            {
                var group = new Group
                {
                    title = "Group"
                };
                group.style.backgroundColor = Color.gray * 0.5f;
                foreach (var node in selection.OfType<Node>())
                {
                    group.AddElement(node);
                }
                ClearSelection();
                AddElement(group);
                selection.Add(group);
            }
        }

        public void CreateGroup(string name, List<Node> nodes)
        {
            var group = new Group
            {
                title = name
            };
            group.style.backgroundColor = Color.gray * 0.5f;
            foreach (var node in nodes)
            {
                group.AddElement(node);
            }
            AddElement(group);
        }

        private void UngroupNodes()
        {
            if (selection.Count() > 0)
            {
                var group = selection.OfType<Group>().FirstOrDefault();
                if (group != null)
                {
                    foreach (var node in group.Children().OfType<Node>())
                    {
                        AddElement(node);
                    }
                    RemoveElement(group);
                    selection.Remove(group);
                }
            }
        }
        private void UnlinkAndRemove(Node n)
        {
            foreach (var port in n.outputContainer.Children().OfType<Port>())
            {
                port.connections.ToList().ForEach(c => RemoveElement(c));
            }
            foreach (var port in n.inputContainer.Children().OfType<Port>())
            {
                port.connections.ToList().ForEach(c => RemoveElement(c));
            }
            RemoveElement(n);
        }
        private void UnlinkAndRemove(Edge e)
        {
            e.input.Disconnect(e);
            e.output.Disconnect(e);
            RemoveElement(e);
        }
        private void RemoveSelectedNodes()
        {
            selection.OfType<Node>().ToList().ForEach(n => UnlinkAndRemove(n));
            ClearSelection();
        }
        private void RemoveSelectedEdges()
        {
            selection.OfType<Edge>().ToList().ForEach(e => UnlinkAndRemove(e));
            ClearSelection();
        }

        private void HandleDragEndEvent(DragPerformEvent e)
        {
            
            if (selection.First() is BlackboardField)
            {
                var pill = selection.First() as BlackboardField;
                NodeData.SerializableProperty prop = exposedProperties.Find(p => p.name == pill.text);
                AddElement(GenerateParameterNode(prop.GUID, prop.type, e.localMousePosition - (Vector2)viewTransform.position));
            }
        }
        private void HandleMouseDownEvents(MouseDownEvent e)
        {
            if (e.button == 1)
            {
                
                    var menu = new GenericMenu();
                    if(selection.OfType<Node>().Count() > 0)
                    {
                        menu.AddItem(new GUIContent("Delete"), false, RemoveSelectedNodes);
                    }
                    if(selection.OfType<Edge>().Count() > 0)
                    {
                        menu.AddItem(new GUIContent("Delete"), false, RemoveSelectedEdges);
                    }
                    if(selection.Count() > 1)
                    {
                        menu.AddItem(new GUIContent("Group"), false, GroupNodes);
                        menu.AddItem(new GUIContent("Delete All"), false, RemoveSelectedNodes);
                    }
                    if(selection.OfType<Group>().Count() > 0)
                    {
                        menu.AddItem(new GUIContent("Ungroup"), false, UngroupNodes);
                    }
                    menu.AddItem(new GUIContent("Add Query Node"), false, () => UnityEditor.Experimental.GraphView.SearchWindow.Open(new SearchWindowContext(e.mousePosition), querySearchWindow));
                    menu.ShowAsContext();
                
            }
        }

        private void HandleMouseOverEvent(MouseOverEvent e)
        {
            if (e.target is Node)
            {
                currHoveredNode = e.target as Node;
                currHoveredNode.mainContainer.style.backgroundColor = Color.white;
            }
            else
            {
                if(currHoveredNode != null)
                {
                    currHoveredNode.mainContainer.style.backgroundColor = Color.clear;
                    if(currHoveredNode.outputPort != null && currHoveredNode.outputPort.connections.Count() > 0)
                        currHoveredNode.outputPort.connections.ToList().ForEach(c => c.input.node.mainContainer.style.backgroundColor = Color.clear);
                }
                currHoveredNode = null;
            }
        }

        private void HandleMouseLeaveEvent(MouseLeaveEvent e)
        {
            if(currHoveredNode == e.target)
            {
                currHoveredNode.mainContainer.style.backgroundColor = Color.clear;
                currHoveredNode = null;
            }
        }

        public void AddEntryNode()
        {
            AddElement(GenerateEntryPointNode());
        }

        private Port GeneratePort(Node node, Direction dir, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, dir, capacity, typeof(float));
        }

        private Node GenerateParameterNode(string paramReference, System.Type paramType, Vector2 position)
        {
            Node node = new Node
            {
                title = exposedProperties.Find(x => x.GUID == paramReference).name,
                paramReference = paramReference,
                GUID = System.Guid.NewGuid().ToString(),
                nodeType = NodeData.Type.Parameter
            };
            node.SetPosition(new Rect(position, new Vector2(200, 50)));
            node.outputPort = GeneratePort(node, Direction.Output, Port.Capacity.Multi);
            node.outputPort.portType = paramType;
            node.outputPort.portName = paramType.Name;
            node.outputContainer.Add(node.outputPort);
            node.RefreshExpandedState();
            node.RefreshPorts();
            return node;
        }

        public Node GenerateQueryNode(string name, Query query, Vector2 position)
        {
            Node node = new Node
            {
                title = name,
                query = query,
                GUID = System.Guid.NewGuid().ToString(),
                nodeType = NodeData.Type.Query
            };
            node.SetPosition(new Rect(position, new Vector2(200, 50)));
            NodeData.Property[] properties = query.GetProperties().ToArray();
            foreach(NodeData.Property p in properties)
            {
                AddPropertyField(node, p, query);
            }
            node.RefreshExpandedState();
            node.RefreshPorts();
            return node;
        }

        private Node GenerateEntryPointNode()
        {
            Node entryPointNode = new Node
            {
                title = "Entry Point",
                GUID = System.Guid.NewGuid().ToString(),
                nodeType = NodeData.Type.EntryPoint
            };
            entryPointNode.SetPosition(new Rect(new Vector2(0, 0), new Vector2(200, 200)));

            entryPointNode.capabilities = UnityEditor.Experimental.GraphView.Capabilities.Selectable;

            Port newPort = GeneratePort(entryPointNode, Direction.Output);
            newPort.portName = "";
            newPort.portColor = Color.magenta;
            entryPointNode.outputContainer.Add(newPort);
            entryPointNode.outputPort = newPort;

            entryPointNode.runtimeLogic = ScriptableObject.CreateInstance<Repeater>();

            Button btn_newChild = new Button(() =>
            {
                if(entryPointNode.outputPort.Query("connection").ToList().Count > 1) return;
                AddSearchWindow(entryPointNode.outputPort.GetPosition().position);
                
            });
            entryPointNode.titleContainer.Add(btn_newChild);

            

            entryPointNode.RefreshExpandedState();
            entryPointNode.RefreshPorts();


            return entryPointNode;
        }

        public Node ContextCreateNode(Node parent, NodeData.Type type, string name, RuntimeBase logic)
        {
            Node newNode = GenerateNode(type, name, logic);
            newNode.tooltip = logic.tooltip;
            newNode.styleSheets.Add(Resources.Load<StyleSheet>("NodeStyle"));
            switch(type)
            {
                case NodeData.Type.EntryPoint:
                    newNode.titleContainer.style.fontSize = 20;
                    newNode.titleContainer.style.backgroundColor = Color.green * 0.65f;
                    break;
                case NodeData.Type.Parameter:
                    newNode.titleContainer.style.color = Color.white;
                    newNode.titleContainer.style.backgroundColor = Color.grey;
                    break;
                case NodeData.Type.Action:
                    newNode.titleContainer.style.color = Color.black;
                    newNode.titleContainer.style.backgroundColor = Color.red * 0.65f;
                    break;
                case NodeData.Type.Condition:
                    newNode.titleContainer.style.color = Color.black;
                    newNode.titleContainer.style.backgroundColor = Color.blue * 0.65f;
                    break;
                case NodeData.Type.Decorator:
                    newNode.titleContainer.style.color = Color.black;
                    newNode.titleContainer.style.backgroundColor = Color.yellow * 0.65f;
                    break;
                case NodeData.Type.Selector:
                    newNode.titleContainer.style.color = Color.black;
                    newNode.titleContainer.style.backgroundColor = Color.magenta * 0.65f;
                    break;
                case NodeData.Type.Sequence:
                    newNode.titleContainer.style.color = Color.black;
                    newNode.titleContainer.style.backgroundColor = Color.magenta * 0.65f;
                    break;
                case NodeData.Type.Parallel:
                    newNode.titleContainer.style.color = Color.black;
                    newNode.titleContainer.style.backgroundColor = Color.magenta * 0.65f;
                    break;
                
                
            
            }
            newNode.titleContainer.style.fontSize = 30;
            newNode.titleContainer.style.unityFontStyleAndWeight = FontStyle.Bold;
            AddElement(newNode);
            if(parent.outputPort.connections.Count() > 0)
            {
                Node lowest = (Node)parent.outputPort.connections.OrderBy(x => x.input.node.GetPosition().y).Last().input.node;
                newNode.SetPosition(new Rect(new Vector2(parent.GetPosition().x + parent.GetPosition().width + 50,(lowest.GetPosition().yMax + 10)), new Vector2(200, 200)));
            }
            else
            {
                newNode.SetPosition(new Rect(new Vector2(parent.GetPosition().x + parent.GetPosition().width + 50, parent.GetPosition().y), new Vector2(200, 200)));
            }
            AddElement(parent.outputPort.ConnectTo(newNode.inputPort));

            parent.RefreshExpandedState();
            parent.RefreshPorts();
            newNode.RefreshExpandedState();
            newNode.RefreshPorts();

            return newNode;
            
        }

        public Node GenerateNode(NodeData data)
        {
            Node newNode = new Node();
            if(data.nodeType == NodeData.Type.Parameter || data.nodeType == NodeData.Type.Query)
                newNode.title = data.title;
            else
                newNode.title = data.runtimeLogic.GetType().Name;
            newNode.GUID = data.GUID;
            newNode.nodeType = data.nodeType;
            if(!data.noLogic) newNode.tooltip = data.runtimeLogic.tooltip;
            newNode.SetPosition(new Rect(data.position, new Vector2(1000, 200)));
            newNode.styleSheets.Add(Resources.Load<StyleSheet>("NodeStyle"));
            switch (data.nodeType)
            {
                case NodeData.Type.Action:
                    newNode.inputPort = GeneratePort(newNode, Direction.Input);
                    newNode.inputPort.portName = "";
                    
                    break;
                case NodeData.Type.Condition:
                    newNode.inputPort = GeneratePort(newNode, Direction.Input);
                    newNode.inputPort.portName = "";
                    
                    break;
                case NodeData.Type.Decorator:
                    newNode.inputPort = GeneratePort(newNode, Direction.Input);
                    newNode.inputPort.portName = "";
                    newNode.outputPort = GeneratePort(newNode, Direction.Output);
                    newNode.outputPort.portName = "";
                    break;
                case NodeData.Type.Sequence:
                    newNode.inputPort = GeneratePort(newNode, Direction.Input);
                    newNode.inputPort.portName = "";
                    newNode.outputPort = GeneratePort(newNode, Direction.Output, Port.Capacity.Multi);
                    newNode.outputPort.portName = "";
                    break;
                case NodeData.Type.Selector:
                    newNode.inputPort = GeneratePort(newNode, Direction.Input);
                    newNode.inputPort.portName = "";
                    newNode.outputPort = GeneratePort(newNode, Direction.Output, Port.Capacity.Multi);
                    newNode.outputPort.portName = "";
                    break;
                case NodeData.Type.Parallel:
                    newNode.inputPort = GeneratePort(newNode, Direction.Input);
                    newNode.inputPort.portName = "";
                    newNode.outputPort = GeneratePort(newNode, Direction.Output, Port.Capacity.Multi);
                    newNode.outputPort.portName = "";
                    break;
                case NodeData.Type.EntryPoint:
                    newNode.outputPort = GeneratePort(newNode, Direction.Output );
                    newNode.outputPort.portName = "";
                    break;
                case NodeData.Type.Parameter:
                    newNode.outputPort = GeneratePort(newNode, Direction.Output);
                    newNode.outputPort.portName = "Output";
                    newNode.outputPort.portType = exposedProperties.Find(x => x.GUID == data.parentGUID).type;
                    break;
                

            }

            if(newNode.inputPort != null)
                newNode.inputPort.portColor = Color.magenta;
                newNode.inputContainer.Add(newNode.inputPort);
            if(newNode.outputPort != null)
                newNode.outputPort.portColor = newNode.nodeType == NodeData.Type.Parameter ? Color.green : Color.magenta;
                newNode.outputContainer.Add(newNode.outputPort);

            switch(data.nodeType)
            {
                case NodeData.Type.EntryPoint:
                    newNode.titleContainer.style.color = Color.black;
                    newNode.titleContainer.style.backgroundColor = Color.green * 0.65f;
                    break;
                case NodeData.Type.Parameter:
                    newNode.titleContainer.style.color = Color.white;
                    newNode.titleContainer.style.backgroundColor = Color.grey;
                    break;
                case NodeData.Type.Query:
                    newNode.titleContainer.style.color = Color.black;
                    newNode.titleContainer.style.backgroundColor = Color.grey * 0.65f;
                    break;
                case NodeData.Type.Action:
                    newNode.titleContainer.style.color = Color.black;
                    newNode.titleContainer.style.backgroundColor = Color.red * 0.65f;
                    break;
                case NodeData.Type.Condition:
                    newNode.titleContainer.style.color = Color.black;
                    newNode.titleContainer.style.backgroundColor = Color.blue * 0.65f;
                    break;
                case NodeData.Type.Decorator:
                    newNode.titleContainer.style.color = Color.black;
                    newNode.titleContainer.style.backgroundColor = Color.yellow * 0.65f;
                    break;
                case NodeData.Type.Selector:
                    newNode.titleContainer.style.color = Color.black;
                    newNode.titleContainer.style.backgroundColor = Color.magenta * 0.65f;
                    break;
                case NodeData.Type.Sequence:
                    newNode.titleContainer.style.color = Color.black;
                    newNode.titleContainer.style.backgroundColor = Color.magenta * 0.65f;
                    break;
                case NodeData.Type.Parallel:
                    newNode.titleContainer.style.color = Color.black;
                    newNode.titleContainer.style.backgroundColor = Color.magenta * 0.65f;
                    break;
                
                
            
            }
            newNode.titleContainer.style.fontSize = 30;
            newNode.titleContainer.style.unityFontStyleAndWeight = FontStyle.Bold;
            newNode.runtimeLogic = data.runtimeLogic;
            newNode.query = data.query;
            if(newNode.runtimeLogic != null)
            {
                NodeData.Property[] properties = newNode.runtimeLogic.GetProperties().ToArray();
                foreach(NodeData.Property p in properties)
                {
                    AddPropertyField(newNode, p, newNode.runtimeLogic);
                }
            }
            if(newNode.query != null)
            {
                NodeData.Property[] properties = newNode.query.GetProperties().ToArray();
                foreach(NodeData.Property p in properties)
                {
                    AddPropertyField(newNode, p, newNode.query);
                }
            }
            if(!(newNode.nodeType == NodeData.Type.Action || newNode.nodeType == NodeData.Type.Condition || newNode.nodeType == NodeData.Type.Parameter || newNode.nodeType == NodeData.Type.Query))
            {
                Button btn_newChild = new Button(() =>
                {
                    if (newNode.outputPort.connected && newNode.outputPort.capacity == Port.Capacity.Single) return;
                    AddSearchWindow(GUIUtility.GUIToScreenPoint(newNode.GetGlobalCenter()));
                    searchWindow.SetSelectedNode(newNode);
                })
                {
                    text = "+"
                };
                newNode.titleContainer.Add(btn_newChild);
            }
            
            newNode.RefreshExpandedState();
            newNode.RefreshPorts();

            return newNode;
        }

        public Node GenerateNode(NodeData.Type nodeType, string name, RuntimeBase logic)
        {
            Node node = new Node
            {
                title = name,
                GUID = System.Guid.NewGuid().ToString(),
                nodeType = nodeType
            };
            node.SetPosition(new Rect(new Vector2(0, 0), new Vector2(400, 200)));
            node.runtimeLogic = logic;
            switch (nodeType)
            {
                case NodeData.Type.Action:
                    {
                        Port newPort = GeneratePort(node, Direction.Input);
                        newPort.portName = "";
                        node.inputContainer.Add(newPort);
                        node.inputPort = newPort;
                    }
                    break;
                case NodeData.Type.Condition:
                    {
                        Port newPort = GeneratePort(node, Direction.Input);
                        newPort.portName = "";
                        node.inputContainer.Add(newPort);
                        node.inputPort = newPort;
                    }
                    break;
                case NodeData.Type.Decorator:
                    {
                        Port newPort = GeneratePort(node, Direction.Input);
                        newPort.portName = "";
                        node.inputContainer.Add(newPort);
                        node.inputPort = newPort;

                        newPort = GeneratePort(node, Direction.Output);
                        newPort.portName = "";
                        node.outputContainer.Add(newPort);
                        node.outputPort = newPort;
                    }
                    break;
                case NodeData.Type.Sequence:
                    {
                        Port newPort = GeneratePort(node, Direction.Input);
                        newPort.portName = "";
                        node.inputContainer.Add(newPort);
                        node.inputPort = newPort;

                        newPort = GeneratePort(node, Direction.Output, Port.Capacity.Multi);
                        newPort.portName = "";
                        node.outputContainer.Add(newPort);
                        node.outputPort = newPort;
                    }
                    break;
                case NodeData.Type.Selector:
                    {
                        Port newPort = GeneratePort(node, Direction.Input);
                        newPort.portName = "";
                        node.inputContainer.Add(newPort);
                        node.inputPort = newPort;

                        newPort = GeneratePort(node, Direction.Output, Port.Capacity.Multi);
                        newPort.portName = "";
                        node.outputContainer.Add(newPort);
                        node.outputPort = newPort;
                    }
                    break;
                case NodeData.Type.Parallel:
                    {
                        Port newPort = GeneratePort(node, Direction.Input);
                        newPort.portName = "";
                        node.inputContainer.Add(newPort);
                        node.inputPort = newPort;

                        newPort = GeneratePort(node, Direction.Output, Port.Capacity.Multi);
                        newPort.portName = "";
                        node.outputContainer.Add(newPort);
                        node.outputPort = newPort;
                    }
                    break;
            }
            if(node.inputPort != null)
                node.inputPort.portColor = Color.magenta;
            if(node.outputPort != null)
                node.outputPort.portColor = node.nodeType == NodeData.Type.Parameter ? Color.green : Color.magenta;
            if(!(node.nodeType == NodeData.Type.Action || node.nodeType == NodeData.Type.Condition))
            {
                Button btn_newChild = new Button(() =>
                {
                    if (node.outputPort.connected && node.outputPort.capacity == Port.Capacity.Single) return;
                    AddSearchWindow(GUIUtility.GUIToScreenPoint(node.GetGlobalCenter()));
                    searchWindow.SetSelectedNode(node);
                })
                {
                    text = "+"
                };
                node.titleContainer.Add(btn_newChild);
            }
            NodeData.Property[] properties = logic.GetProperties().ToArray();
            foreach(NodeData.Property p in properties)
            {
                AddPropertyField(node, p, logic);
            }
            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(new Vector2(0, 0), new Vector2(200, 200)));

            return node;
        }

        void AddPropertyField(Node node, NodeData.SerializableProperty property, RuntimeBase logic)
        {
            var newPort = GeneratePort(node, Direction.Input);
            newPort.portName = property.name;
            newPort.portType = property.type;
            newPort.portColor = Color.green;
            Node paramNode = nodes.ToList().ConvertAll(x => x as Node).Find(x => x.GUID == property.GUID);
            Node queryNode = nodes.ToList().ConvertAll(x => x as Node).Find(x => x.nodeType == NodeData.Type.Query && x.query.GetProperties().Find(y => y.GUID == property.paramReference) != null);
            if(property.type == typeof(bool))
            {
                var boolField = new Toggle
                {
                    name = property.name,
                    value = (property).bvalue
                };
                boolField.RegisterValueChangedCallback(evt =>
                {
                    (property).bvalue = boolField.value;
                    logic.SetProperty(property.name, boolField.value);
                });
                newPort.contentContainer.Add(boolField);
            }
            else if(property.type == typeof(int))
            {
                var intField = new IntegerField
                {
                    name = property.name,
                    value = property.ivalue
                };
                intField.RegisterValueChangedCallback(evt =>
                {
                    (property).ivalue = intField.value;
                    logic.SetProperty(property.name, intField.value);
                });
                newPort.contentContainer.Add(intField);
            }
            else if(property.type == typeof(float))
            {
                var floatField = new FloatField
                {
                    name = property.name,
                    value = (property).fvalue
                };
                floatField.RegisterValueChangedCallback(evt =>
                {
                    (property).fvalue = floatField.value;
                    logic.SetProperty(property.name, floatField.value);
                });
                newPort.contentContainer.Add(floatField);
            }
            else if(property.type == typeof(string))
            {
                var textField = new TextField
                {
                    name = property.name,
                    value = (property).svalue
                };
                textField.RegisterValueChangedCallback(evt =>
                {
                    (property).svalue = textField.value;
                    logic.SetProperty<string>(property.name, textField.value);
                });
                newPort.contentContainer.Add(textField);
            }
            else if(property.type == typeof(Vector2))
            {
                var vectorField = new Vector2Field
                {
                    name = property.name,
                    value = (property).v2value
                };
                vectorField.RegisterValueChangedCallback(evt =>
                {
                    (property).v2value = vectorField.value;
                    logic.SetProperty<Vector2>(property.name, vectorField.value);
                });
                node.style.minWidth = 250;
                
                vectorField.style.width = 50;
                vectorField.style.overflow = Overflow.Visible;
                newPort.contentContainer.Add(vectorField);
            }
            else if(property.type == typeof(Vector3))
            {
                var vectorField = new Vector3Field
                {
                    name = property.name,
                    value = (property).v3value
                };
                vectorField.RegisterValueChangedCallback(evt =>
                {
                    (property).v3value = vectorField.value;
                    logic.SetProperty<Vector3>(property.name, vectorField.value);
                });
                node.style.minWidth = 250;
                
                vectorField.style.width = 50;
                vectorField.style.overflow = Overflow.Visible;
                
                newPort.contentContainer.Add(vectorField);
            }
            else if(property.type == typeof(Vector4))
            {
                var vectorField = new Vector4Field
                {
                    name = property.name,
                    value = (property).v4value
                };
                vectorField.RegisterValueChangedCallback(evt =>
                {
                    (property).v4value = vectorField.value;
                    logic.SetProperty<Vector4>(property.name, vectorField.value);
                });
                node.style.minWidth = 300;
                
                vectorField.style.width = 50;
                vectorField.style.overflow = Overflow.Visible;
                newPort.contentContainer.Add(vectorField);
            }
            else if(property.type == typeof(Color))
            {
                var colorField = new ColorField
                {
                    name = property.name,
                    value = (property).cvalue
                };
                colorField.RegisterValueChangedCallback(evt =>
                {
                    (property).cvalue = colorField.value;
                    logic.SetProperty<Color>(property.name, colorField.value);
                });
                newPort.contentContainer.Add(colorField);
            }
            else
            {
                var objField = new ObjectField
                {
                    name = property.name,
                    value = (UnityEngine.Object)(property).ovalue,
                    objectType = property.type
                };

                objField.RegisterValueChangedCallback(evt =>
                {
                    (property).ovalue = objField.value;
                    logic.SetProperty(property.name, objField.value, property.type);
                });
                
                objField.style.maxWidth = 150;
                newPort.contentContainer.Add(objField);
                objField.allowSceneObjects = false;
                

                
            }
            node.inputContainer.Add(newPort);
            node.inputPorts.Add(newPort);
            if(paramNode != null)
            {
                var newEdge = paramNode.outputPort.ConnectTo(newPort);
                AddElement(newEdge);

            }
            else if(queryNode != null)
            {
                var newEdge = queryNode.outputPorts[queryNode.query.GetProperties().FindIndex(x => x.GUID == property.paramReference)].ConnectTo(newPort);
                AddElement(newEdge);

            }
        }

        void AddPropertyField(Node node, NodeData.SerializableProperty property, Query query)
        {
            var newPort = GeneratePort(node,property.output ? Direction.Output : Direction.Input);
            newPort.portName = property.name;
            newPort.portType = property.type;
            newPort.portColor = Color.green;
            Node paramNode = nodes.ToList().ConvertAll(x => x as Node).Find(x => x.GUID == property.GUID);
            Node queryNode = nodes.ToList().ConvertAll(x => x as Node).Find(x => x.nodeType == NodeData.Type.Query && x.query.GetProperties().Find(y => y.GUID == property.paramReference) != null);
            if(!property.output)
            {
                if(property.type == typeof(bool))
                {
                    var boolField = new Toggle
                    {
                        name = property.name,
                        value = (property).bvalue
                    };
                    boolField.RegisterValueChangedCallback(evt =>
                    {
                        (property).bvalue = boolField.value;
                        query.SetProperty(property.name, boolField.value);
                    });
                    newPort.contentContainer.Add(boolField);
                }
                else if(property.type == typeof(int))
                {
                    var intField = new IntegerField
                    {
                        name = property.name,
                        value = property.ivalue
                    };
                    intField.RegisterValueChangedCallback(evt =>
                    {
                        (property).ivalue = intField.value;
                        query.SetProperty(property.name, intField.value);
                    });
                    newPort.contentContainer.Add(intField);
                }
                else if(property.type == typeof(float))
                {
                    var floatField = new FloatField
                    {
                        name = property.name,
                        value = (property).fvalue
                    };
                    floatField.RegisterValueChangedCallback(evt =>
                    {
                        (property).fvalue = floatField.value;
                        query.SetProperty(property.name, floatField.value);
                    });
                    newPort.contentContainer.Add(floatField);
                }
                else if(property.type == typeof(string))
                {
                    var textField = new TextField
                    {
                        name = property.name,
                        value = (property).svalue
                    };
                    textField.RegisterValueChangedCallback(evt =>
                    {
                        (property).svalue = textField.value;
                        query.SetProperty<string>(property.name, textField.value);
                    });
                    newPort.contentContainer.Add(textField);
                }
                else if(property.type == typeof(Vector2))
                {
                    var vectorField = new Vector2Field
                    {
                        name = property.name,
                        value = (property).v2value
                    };
                    vectorField.RegisterValueChangedCallback(evt =>
                    {
                        (property).v2value = vectorField.value;
                        query.SetProperty<Vector2>(property.name, vectorField.value);
                    });
                    node.style.minWidth = 250;
                    
                    vectorField.style.width = 50;
                    vectorField.style.overflow = Overflow.Visible;
                    newPort.contentContainer.Add(vectorField);
                }
                else if(property.type == typeof(Vector3))
                {
                    var vectorField = new Vector3Field
                    {
                        name = property.name,
                        value = (property).v3value
                    };
                    vectorField.RegisterValueChangedCallback(evt =>
                    {
                        (property).v3value = vectorField.value;
                        query.SetProperty<Vector3>(property.name, vectorField.value);
                    });
                    node.style.minWidth = 250;
                    
                    vectorField.style.width = 50;
                    vectorField.style.overflow = Overflow.Visible;
                    
                    newPort.contentContainer.Add(vectorField);
                }
                else if(property.type == typeof(Vector4))
                {
                    var vectorField = new Vector4Field
                    {
                        name = property.name,
                        value = (property).v4value
                    };
                    vectorField.RegisterValueChangedCallback(evt =>
                    {
                        (property).v4value = vectorField.value;
                        query.SetProperty<Vector4>(property.name, vectorField.value);
                    });
                    node.style.minWidth = 300;
                    
                    vectorField.style.width = 50;
                    vectorField.style.overflow = Overflow.Visible;
                    newPort.contentContainer.Add(vectorField);
                }
                else if(property.type == typeof(Color))
                {
                    var colorField = new ColorField
                    {
                        name = property.name,
                        value = (property).cvalue
                    };
                    colorField.RegisterValueChangedCallback(evt =>
                    {
                        (property).cvalue = colorField.value;
                        query.SetProperty<Color>(property.name, colorField.value);
                    });
                    newPort.contentContainer.Add(colorField);
                }
                else
                {
                    var objField = new ObjectField
                    {
                        name = property.name,
                        value = (UnityEngine.Object)(property).ovalue,
                        objectType = property.type
                    };

                    objField.RegisterValueChangedCallback(evt =>
                    {
                        (property).ovalue = objField.value;
                        query.SetProperty(property.name, objField.value, property.type);
                    });
                    
                    objField.style.maxWidth = 150;
                    newPort.contentContainer.Add(objField);
                    objField.allowSceneObjects = false;
                }
                node.inputContainer.Add(newPort);
                node.inputPorts.Add(newPort);
            }
            else
            {
                node.outputContainer.Add(newPort);
                node.outputPorts.Add(newPort);
            }
            if(paramNode != null)
            {
                var newEdge = paramNode.outputPort.ConnectTo(newPort);
                AddElement(newEdge);

            }
            else if(queryNode != null)
            {
                var newEdge = queryNode.outputPorts[queryNode.query.GetProperties().FindIndex(x => x.GUID == property.paramReference)].ConnectTo(newPort);
                AddElement(newEdge);

            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort!=port && startPort.node!=port.node && port.portType == startPort.portType && port.direction != startPort.direction)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        public void AddPropertyToBlackboard(NodeData.Property exposedProperty)
        {
            if(exposedProperties == null)
            {
                exposedProperties = new List<NodeData.Property>();
            }
            NodeData.Property p = new NodeData.Property
            {
                type = exposedProperty.type,
                name = exposedProperty.name,
                GUID = exposedProperty.GUID,
                value = exposedProperty.value
            };



            exposedProperties.Add(p);

            var container = new VisualElement();
            var blackboardField = new BlackboardField{ text = p.name, typeText = p.type.Name };

            var delButton = new Button(() =>
            {
                nodes.ForEach(x =>
                {
                    if (((Node)x).paramReference == p.GUID)
                    {
                        ((Node)x).outputPort.connections.ToList().ForEach(y =>
                        {
                            y.input.Disconnect(y);
                            y.output.Disconnect(y);
                            RemoveElement(y);
                        });

                        RemoveElement(x);
                    }
                });


                exposedProperties.Remove(p);
                container.RemoveFromHierarchy();
            })
            {
                text = "X"
            };
            blackboardField.Add(delButton);
            
            container.Add(blackboardField);
            
            
            
            //container.Add(newButton);
            blackboard.Add(container);

        }

        

        

        

        

        
    }
}
