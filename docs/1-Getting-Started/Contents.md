## 1. Getting Started

[1.1. Quick Start](#1.1.-quick-start)

[1.2. Interface Overview](#1.2.-interface-overview)

[1.3. Components](#1.3.-components)

[1.4. Documentation Component](#1.4.-documentation-component)

---
### 1.1. Quick Start

#### Objective:

In this tutorial we'll take the shortest route from an empty session to a moving Robot in the HAL Robotics Framework for Grasshopper.

#### Demo Files:

[1.1 - Getting Started.gh](../ExampleFiles/Tutorials/1.1%20-%20Getting%20Started.gh)

#### Requirements to follow along:

A PC with Rhinoceros 3D, Grasshopper and the HAL Robotics Framework installed.

A basic knowledge of Rhinoceros 3D and Grasshopper are highly recommended.

#### Background:

The HAL Robotics Framework is a modular software library that simplify the modelling, programming and simulation of processes involving industrial machines. The client for McNeel software gives you access to the vast majority of the Framework's functionality from within Grasshopper, a visual programming environment that runs within Rhinoceros 3D.

#### How to:

Once you've opened Grasshopper and logged in to the HAL Robotics Framework to access your licenses you should be presented with a blank Grasshopper canvas and a new **HAL Robotics** tab at the top your screen. There are plenty of tools within this tab to allow you to program just about any process you want, and other tutorials will look at each of them individually as well as the [structure of the interface](../1-Getting-Started/Contents.md#1.2.-interface-overview), but for now, we're just going to get up and running as quickly as possible.

The first component we're going to use is the **Robot**. You'll find it inside the **Cell** menu. When we place that component on the canvas not much happens. This is because **Robot** is one of a few special types of component that we've developed to extend the default Grasshopper interface. We'll see what each does as we come across them and they are all covered in a [special tutorial](../1-Getting-Started/Contents.md#1.3.-components) so feel free to have a look at that for more detail. This particular component has an interlocking square icon (⧉) on the name bar. This indicates that it can be double-clicked to get a pop-up window. If we open that window on the **Robot** component then we get the Robot Catalog. You can select any [Robot](../7-Glossary/Contents.md#manipulator) you want by simply clicking on it and then the "Select" button. I'm going to choose the `IRB 1200-70` as that's what I have next to me at the office. Once we've made our selection it will pop up into the scene. If you want to change your choice, you can always double click on the component again.

We have a [Robot](../7-Glossary/Contents.md#manipulator), but we're almost always going to need a [Tool](../7-Glossary/Contents.md#end-effector), or [End Effector](../7-Glossary/Contents.md#end-effector), for our [Robot](../7-Glossary/Contents.md#manipulator) so I'm going to add a **Tool** component, again from the **Cell** menu. We assume that you will want to create your own [Tools](../7-Glossary/Contents.md#end-effector) so this catalog component contains only a few options to get you started. The creation of [Tools](../7-Glossary/Contents.md#end-effector) is covered in future tutorials ([1](../2-Cell/Contents.md#2.4.-create-a-tool),[2](../2-Cell/Contents.md#2.5.-calibrate-a-reference-or-tool),[3](../2-Cell/Contents.md#2.6.-create-a-multi-part-tool)) so please have a look at those for more information. I'm going to select the `extruder` because I know it's about the right size for this [Robot](../7-Glossary/Contents.md#manipulator).

Once we have a [Robot](../7-Glossary/Contents.md#manipulator) and a [Tool](../7-Glossary/Contents.md#end-effector) in the scene, we need to connect the two together. We do that using the **Attach** component. We will see that once we've assigned the **Robot** as the _Parent_ and **Tool** as the _Child_, the `extruder` is attached to the end or flange of the [Robot](../7-Glossary/Contents.md#manipulator). That's all there is to creating our very simple [Robot Cell](../7-Glossary/Contents.md#cell).

We're going to hide all the components we've already added to ensure that we can see what we're doing as we proceed.

Now that we have a [Robot](../7-Glossary/Contents.md#manipulator) in the scene, we need to [program](../7-Glossary/Contents.md#7.2.-programming) it. To get started I'm just going to make it follow a curve that I have drawn in Rhino and referenced in Grasshopper. [Robots](../7-Glossary/Contents.md#manipulator) don't inherently understand geometry so we have to convert this curve into a series of [Targets](../7-Glossary/Contents.md#target) for the [Robot](../7-Glossary/Contents.md#manipulator) to follow. We can this with the **Target** component under the **Motion** menu. When we place the component on the canvas you should see that its key input is a list of _Frames_ but this component also has a black band at the bottom which says "Template 1/n". This is the second new component type we've come across. Template components can be right-clicked upon and their shape changed. In this case you can see that there are a few different ways of creating [Targets](../7-Glossary/Contents.md#target). We've got a curve as input so we're going to choose **Target from Curve**. There are a number of things we can change here but to keep things simple we'll keep the defaults and just assign the curve in the _Curve_ input. You should see a number of [Targets](../7-Glossary/Contents.md#target) appear in the viewport. The exact number and pattern, of course, will vary according to the curve you're using. Now that we have some [Targets](../7-Glossary/Contents.md#target), we need to actually instruct the [Robot](../7-Glossary/Contents.md#manipulator) to move through them. We do this using the **Move** component which is found in the **Procedure** menu. There are plenty of options to control how a [Robot](../7-Glossary/Contents.md#manipulator) gets to these [Targets](../7-Glossary/Contents.md#target) and they are discussed in detail in future tutorials ([1](../3-Motion/Contents.md#3.3.-change-motion-settings),[2](../3-Motion/Contents.md#3.5.-synchronize-motion)). The bare minimum is to assign our **Targets** to **Move** and there we have a [Procedure](../7-Glossary/Contents.md#procedure) ready for [Execution](../7-Glossary/Contents.md#7.3.-simulation). Again, I'm going to hide the components other than **Move** to keep things simple on screen.

Now that we have a [Robot](../7-Glossary/Contents.md#manipulator) and a [Procedure](../7-Glossary/Contents.md#procedure), we need to link the two together. In the HAL Robotics Framework, as with your real [Robot](../7-Glossary/Contents.md#manipulator), the entity that interprets programming to actuate a machine is a [Controller](../7-Glossary/Contents.md#controller). We can get a **Controller** from the **Cell** menu. We can see that this also has two interlocking squares (⧉) and therefore can be double-clicked to get a pop-up window. From this Controller Catalog you can choose the [Controller](../7-Glossary/Contents.md#controller) you are using as you did with the **Robot**. Please note, you will need the extension for any brand installed to be able to load their [Controllers](../7-Glossary/Contents.md#controller). If you didn't install any extensions you can continue with the `Generic Controller` for now and install the brand-specific extensions from the [HAL Robotics Framework Installer](../0-Administration-and-Setup/Contents.md#0.1.-install) later. The [Robot](../7-Glossary/Contents.md#manipulator) we have here uses an `IRC5 Compact V2` so I'm going to pick that. You can configure the complete setup of your [Controller](../7-Glossary/Contents.md#controller) and we have a [separate tutorial](../6-Control/Contents.md#6.1.-configure-a-virtual-controller) on that but for now I'm going to "Configure" with the defaults. The position and visual representation of the **Controller** is of little interest in this case so I'm going to hide it immediately. With the **Controller** in place we can assign the **Robot** and **Procedure** to link the two together.

Now, of course, we want to see whether the [Robot](../7-Glossary/Contents.md#manipulator) can make it round our curve without issue. This is referred to as "Solving" and is done using the **Procedure Solver** component in the **Simulation** menu. With that component in place we can assign the _Controller_ and toggle the _Solve_ Boolean to `true`. This will start the solving and any issues, errors or notification will be output under _Notifications_. If we now want to visualize that solved [Procedure](../7-Glossary/Contents.md#procedure) then we can add the **Execute** component to the end of our chain from, again, the **Simulation** menu. This should come with its **Execution Control** pre-attached but if it doesn't you can find the **Execution Control** under the **Simulation** menu as well. Once the _Solution_ is assigned to the **Execute** component we're ready to hit _Play_ and watch our [Robot](../7-Glossary/Contents.md#manipulator) make its way around the curve. If your [Robot](../7-Glossary/Contents.md#manipulator) is moving, congratulations, you've completed your first session with the HAL Robotics Framework for Grasshopper.

---
### 1.2. Interface Overview

#### Objective:

In this tutorial we'll take a look at the layout HAL Robotics Framework components in Grasshopper.

#### Requirements to follow along:

A PC with the Rhinoceros 3D, Grasshopper and the HAL Robotics Framework installed.

#### Background:

The HAL Robotics Framework for Grasshopper installs a new tab within the Grasshopper UI called **HAL Robotics**. You will know that the Framework is installed if you can see the **HAL Robotics** tab and a number of blue hexagonal parameters components on the **Params** tab.

#### How to:

In the **Params** tab you will find a number of HAL Robotics Framework parameters. These are all in blue hexagons and may come in useful for keeping your documents organized. The parameters panels are organized in the same way as the **HAL Robotics** tab, as we'll see shortly, with the addition of all of these units which can be used to ensure you use the units most appropriate to your way of working regardless of the units of your model space.

Within the **HAL Robotics** tab, you will see 7 (at the time of writing) panels of components. These are ordered and organized to guide you through the process of setting up your processes in the HAL Robotics Framework.

a.  **Cell** - The Cell panel covers everything you'll need to build up a virtual version of your robotic [Cell](../7-Glossary/Contents.md#cell). This includes [Robot](../7-Glossary/Contents.md#manipulator), [Positioner](../7-Glossary/Contents.md#positioner) and [Tool](../7-Glossary/Contents.md#end-effector) presets, as well as the components required to build your own [Mechanisms](../7-Glossary/Contents.md#mechanism), [Parts](../7-Glossary/Contents.md#part), set up your [I/O Signals](../7-Glossary/Contents.md#signal) and assemble all of these into a complete digital system through a [Controller](../7-Glossary/Contents.md#controller).

b.  **Motion** - The Motion panel contains all the components to create and manipulate [Targets](../7-Glossary/Contents.md#target) and control how your [Robots](../7-Glossary/Contents.md#manipulator) are going to move towards those [Targets](../7-Glossary/Contents.md#target) by specifying the Speed, Acceleration, [Blend](../7-Glossary/Contents.md#blend) and Kinematic Settings.

c.  **Procedure** - The Procedure panel is where you'll find all the tools necessary to program your machines. This includes [Move](../7-Glossary/Contents.md#motion-action), [Wait](../7-Glossary/Contents.md#wait-action) and [Signal Change](../7-Glossary/Contents.md#signal-action) [Actions](../7-Glossary/Contents.md#action) as well as utilities for changing [Tools](../7-Glossary/Contents.md#end-effector) at runtime, manipulating [Parts](../7-Glossary/Contents.md#part) and Combining these into a [Procedure](../7-Glossary/Contents.md#procedure) ready to [Simulate](../7-Glossary/Contents.md#7.3.-simulation).

d.  **Simulation** - The Simulation panel contains everything you need to [Solve](../7-Glossary/Contents.md#solving) and Execute a [Simulation](../7-Glossary/Contents.md#7.3.-simulation) of your [Procedures](../7-Glossary/Contents.md#procedure).

e.  **Control** - The Control panel is where you'll find components to get your [Procedures](../7-Glossary/Contents.md#procedure) out of the digital world and onto your real machines. This includes functionality such as [Exporting](../7-Glossary/Contents.md#export) and [Uploading](../7-Glossary/Contents.md#upload) code.

f.  **Utilities** - The Utilities tab contains useful tools such as Frame creators which will enable you to input or output frame data in any standard formalism.

g.  **Help** - The Help panel contains the Documentation component and other tools to help you look up information about the software.

---
### 1.3. Components

#### Objective:

In this tutorial we'll look at the component variations and patterns added to HAL Robotics Framework components in Grasshopper.

#### Requirements to follow along:

A PC with the Rhinoceros 3D, Grasshopper and the HAL Robotics Framework installed.

#### How to:

On any given HAL Robotics Framework component, you will notice a few recurring patterns and symbols in the names of inputs and outputs. These patterns can be used individually or combined on a single parameter. The Create Part component is a good example of all of these.

a.  →Name - An arrow ( → ) preceding an input name means that the input is mandatory. Component will not compute until data is provided to every mandatory input.

b.  \[Name\] - Square brackets around the name of an input or output mean that the data will be treated as a list or collection.

c. {Name} - Curly brackets around the name of an input or output mean that the data will be treated as a Grasshopper DataTree.

d.  Name (unit) - A name followed by a unit in brackets e.g. (mm) or (rad), means that the input will be treated in a specific unit. These units can be changed by right-clicking on the input, hovering over _Unit_ and selecting the unit you prefer to use. You can assign a textual expression to these inputs and it will be calculated for you e.g. 3m + 35mm.

To help keep the interface and number of components manageable we have introduced 3 new modes of interaction with Grasshopper components. Components that feature each new interaction mode can be identified easily.

a.  Windowed Components a.k.a. Pop-up Components, such as the **Robot** preset component, feature two interlocking squares (⧉) in their name. You can double click on these components to get a pop-up window with additional component inputs. This could be a catalog of available [Robot](../7-Glossary/Contents.md#manipulator) or [Controller](../7-Glossary/Contents.md#controller) presets, or an overview of [Procedure](../7-Glossary/Contents.md#procedure) execution progress.

b.  Overloaded Components, such as the **Create Reference** component, can be recognized by their black bar at the bottom which states _Shift + ↕ (1/2)_, or similar. Overloads of a component all perform the same function, such as creating a [Reference](../7-Glossary/Contents.md#reference), but are designed to simplify components for typical use by keeping advanced inputs out of the way until you need them.

c.  Templated Components, such as the **Frame** component, can be identified by their black bar at the bottom which states _Template 1/3_, or similar. Templates are a means of grouping components that create similar objects or use different construction methods. For example, in the case of **Frame**, the templates represent different formalisms for frame creation such as Euler, Quaternion etc. Similarly, in the **Create Target** component, templates all create [Targets](../7-Glossary/Contents.md#target) but use different inputs e.g. from a Frame, from Joint Positions or from a Curve.

---
### 1.4. Documentation Component

#### Objective:

In this tutorial we'll look at the **Documentation** component and how it can help you discover functionality within the HAL Robotics Framework for Grasshopper.

#### Requirements to follow along:

A PC with the Rhinoceros 3D, Grasshopper and the HAL Robotics Framework installed.

Reading or watching the Component Variations tutorial is highly recommended.

#### How to:

The **Documentation** component can be accessed through the HAL menu under Help -\> Documentation or via the **Help** panel in the **HAL Robotics** tab of Grasshopper. You can drag this component onto an existing component to display its documentation, or if you instantiate the component on the canvas, you will be presented with a list of all the available HAL Robotics Framework component Templates. Using the search bar at the top of the window you can look for functionality that interests you. For example, if you type "speed" and trigger the search by hitting enter or space you will see a list of Templates which use the word "speed" in their title or description. The first option presented should be the **Speed Settings**. By selecting that component, if it isn't already, you will see the component layout at the bottom of the window with descriptions of each input and output, as well as a series of tabs which will show the different Overloads of the component. From here you can add the component to your current document by clicking "add to document" or close the window to cancel. This same functionality is extended to many other Grasshopper libraries and plugins which can be activated in the _Libraries_ slide-out on the left-hand side of the window.