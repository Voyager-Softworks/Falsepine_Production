
<h1 align="center">
  <br>
  <!-- <a href="http://www.amitmerchant.com/electron-markdownify"><img src="https://raw.githubusercontent.com/amitmerchant1990/electron-markdownify/master/app/img/markdownify.png" alt="Markdownify" width="200"></a> -->
  <br>
  NodeAI
  <br>
</h1>

<h4 align="center">A Node-Graph based Behaviour Tree editor for <a href="http://Unity.com" target="_blank">Unity</a>.</h4>


<h4 align="center">
  <a href="https://github.com/Nerys-Thamm">Credits</a> •
  <a href="#license">License</a>
</h4>

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986186934684635166/ExampleBehaviour.gif)

## Why NodeAI?



## Key Features

* Node-Graph based behaviour tree designer.
* Flexible and powerful AI Agent.
* Senses module for environmental awareness.
* Runtime visual debugging.
* Easy to use.
* Powerful and Extensible.

## How To Install

To import this plugin into Unity, simply drag the plugin file into your Project tab, and it will import.

## Getting Started

To get started, open the NodeAI Graph Editor

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986203790346690590/OpenGraph.gif)

## The NodeAI Graph Editor
You will see the NodeAI Graph Editor open.
The editor consists of a few parts, these being:
* The Graph: The grid are where you'll be placing nodes
* The Toolbar:
  - Save Button: This saves your changes to the AI Behaviour.
  - New Behaviour Button: This lets you create a new AI Behaviour.
  - Behaviour Slot: Shows the behaviour you are currently editing.

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986206819158478888/unknown.png)

<h2>Creating a new Behaviour:</h2>

When you create a new behaviour, it will appear in your project view, where you can rename it. If you want to edit the behaviour at any time, just double-click it to open it in the Editor.

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986208100270882933/unknown.png)

<h3>Now we're ready to get started!</h3>

## Creating your first AI Behaviour

When you open your newly created AI Behaviour, you will see it is now being edited in the NodeAI Graph Editor. You will also notice that a new node has appeared, this is your entry node, and you will be building your whole behaviour off of it.

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986208522264002600/unknown.png)

## The Node Search Window

Click the + button to add a new node to the graph.
You will notice that a search window appears, which you can use to select the node you wish to insert.

You can either search nodes by name, or look through the catagories.

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986209356003540992/unknown.png)

## The Nodes
Behaviour trees use three different kinds of nodes:
* Control Nodes: Nodes that control the flow of execution across the graph.
* Action Nodes: Nodes which make the Agent do something.
* Condition Nodes: Nodes which check some sort of condition.

Lets start with Control Nodes. These nodes are what determine how the execution flow will respond to the success or failure of your action and condition nodes. 

The types of Control Nodes are:
* Sequence Nodes: These nodes will run each of their child nodes, from top to bottom, until either all of them have succeeded, or one of them fails.
* Selector Nodes: These nodes will run each of their child nodes, from top to bottom, until either all of them fail or one has succeeded.
* Parallel Nodes: These nodes will run all of their child nodes concurrently, until one of them succeeds, or all of them fail.
* Decorator Nodes: These nodes can only have one child, and they change the way that child behaves in some fashion.

Nodes are connected to eachother left-to-right, with each node being the child of the node it was created from (you can see this from the purple sequence input port on the child node being connected to the sequence output port on the parent node).

The child nodes of a parent are ordered top to bottom, and will automatically layout as such when you create them, though, of course how you choose to layout your graph is entirely up to you.

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986212076613664828/unknown.png)

Next up, Action Nodes. These nodes are used to control how your AI Agent behaves, for example having them seek a target, play an animation, play a sound, or anything you choose to code using the NodeAI library.

Action nodes can have properties, which are fields with values you can set to customise the behaviour of that action node. For example, the GoTo node takes position, stopping distance, speed, and acceleration properties to control how it functions. When using this node, you tell the Agent where exactly you want it to go by setting the position property.

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986212923078754374/unknown.png)

Condition nodes work similarly to action nodes, however they arent used to make the Agent do anything, they simply check whether their condition is true or not.

## Parameter Nodes
Sometimes, we need to get external information passed into our behaviour from the inspector so we can further customise how our AI Agents function. Lets do it!

Start by pressing the + button on the Parameters blackboard

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986214936785072128/unknown.png)

You will be given a search window where you can select the type of parameter you want. Simply click the one you would like to add, and it will appear in the blackboard. You can double click its name to rename it, press the X button to remove it, or drag it onto the graph to use it in your behaviour.

To use the parameter in your behaviour, simply link it up to the property you wish to set.

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986216078898237450/AttatchingAParam.gif)

## Moving, Grouping, and Deleting Nodes

If you want to move, group, or delete nodes, you can do so by selecting them (either by clicking or dragging a selection box over them), and right-clicking to open the context menu. From there you can move, group, or delete all selected nodes. 

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986216983265685504/MoveGroupDelete.gif)

## Using Query Nodes.

Query Nodes are a special type of node that are used to either get data from the scene, process input data into output data, or both. You can create one of these by right-clicking on the graph, and selecting the "Query Node" option. This will open a search window, where you can either search for, or browse for the Query node you would like to add.

Query nodes are connected the same way you connect any other node, and can prove useful for when you need to process data to use as inputs for condition or action nodes.

<h3>Got a Behaviour designed? Sweet, lets try it out!</h3>

## NodeAI Agents

The NodeAI Agent component is how you bring your NPC GameObjects to life.
To get started, simply search for and add the NodeAI_Agent script to the GameObject of your NPC.

Once you have done so, drag your newly created AI Behaviour into the AI Behaviour slot in the component. Once you have done so, make sure that the GameObject has all of the components required by the nodes in your behaviour. Dont worry! If you're missing any, the component will let you know in the unity console when you enter play mode.

From the NodeAI Agent component, you can control two main aspects of your new AI Agent. These being, the Agent faction, which is used in some behaviours to determine which other agents are friendly, and the parameters. You will notice that the parameters you added earlier in the behaviour you created have shown up in the inspector. You can set them now, or you can also set them at runtime in order to test different parts of your behaviour.

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986219019684175872/unknown.png)

## NodeAI Senses

The Senses component is an optional addition that allows your Agent to use behaviours containing nodes in the Senses module. These nodes are used to help your agent percieve and react to objects and events in their environment. To enable this functionality for you agent, simply add the NodeAI_Senses component to its GameObject. 

In the inspector, you should assign the Eyes Bone property as the transform of whatever part of your model best represents their head or eyes. This is what the senses module will use to determine what they can see. You can also edit the sight distance and angle to fine tune your Agent's cone of vision. The sight mask option allows you to choose which object layers your Agent can see, which can be useful when you want to make certain things invisible to them.

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986220639188832336/unknown.png)

## Debugging at Runtime

Sometimes, it can be frustrating when something just isnt working and you dont know why. We understand, we really do, and thats why NodeAI lets you see your Behaviours running in real time!
Simply select an Agent with the same behaviour you currently have open in the Graph Editor, while the game is running, and watch the Editor switch to Debug mode. In Debug mode, you can see coloured lines beneath each node, which tell you what its currently doing.
Here's what the colours mean:
* White: This node is idle, meaning it hasnt been activated yet and isnt doing anything.
* Yellow: This node is currently still running.
* Green: This node has finished running, and it managed to do whatever it was trying to do successfully (go you!)
* Red: This node has finished running, but it failed at doing what it was trying to do.

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986225309764448296/RuntimeDebugging.gif)


## Creating your own nodes

New nodes can be created easily, and will be detected automatically by NodeAI.

To create one, simply create a new C# script, add an include for NodeAI, and change the class it inherits from "monobehaviour" to one of the following:
* <a href='/classNodeAI_1_1ActionBase.html'>NodeAI.ActionBase</a>: For creating an action node.
* <a href='/classNodeAI_1_1ConditionBase.html'>NodeAI.ConditionBase</a>: For creating a condition node.
* <a href='/classNodeAI_1_1DecoratorBase.html'>NodeAI.DecoratorBase</a>: For creating a decorator node.
* <a href='/classNodeAI_1_1Query.html'>NodeAI.Query</a>: For creating a query node.

<p>For more information on how creating nodes works, and code examples of how you can create them yourself, see the code documentation.

## Thats it from us, now go make games!

![screenshot](https://cdn.discordapp.com/attachments/686335357406412949/986225758580142160/RuntimeShowoff.gif)
