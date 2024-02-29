## 3. Programming

[3.0. Procedures](#30-procedures)

[3.1. Motion](#31-motion)

[3.2. Wait for a Time](#32-wait-for-a-time)

[3.3. Set a Signal](#33-set-a-signal)

[3.4. Paths](#34-paths)

[3.5. Custom Actions](#35-custom-actions)

[3.6. Structuring Procedures](#36-structuring-procedures)

[3.7. Validation and Simulation](#37-validation-and-simulation)

---
### 3.0. Procedures

#### Objective:

In this tutorial we'll cover the main structure and background of [Procedures](../../Overview/Glossary.md#procedure) in FlowBuilder.

#### Background:

[Procedures](../../Overview/Glossary.md#procedure) are a set of [Actions](../../Overview/Glossary.md#action) to be executed by a [Controller](../../Overview/Glossary.md#controller). You'll find tools to create individual, atomic [Actions](../../Overview/Glossary.md#action) like [Move](../../Overview/Glossary.md#motion-action) or [Wait](../../Overview/Glossary.md#wait-action) as well as ones which generate complex [Toolpaths](../../Overview/Glossary.md#toolpath) comprised of 10s, 100s or even 1000s of individual [Actions](../../Overview/Glossary.md#action).

Each [Robot](../../Overview/Glossary.md#manipulator) has a main [Procedure](../../Overview/Glossary.md#procedure), always the first in the list, assigned to it. You can add [Procedures](../../Overview/Glossary.md#procedure) to a [Robot](../../Overview/Glossary.md#manipulator) to help you [structure your code](#36-structuring-procedures), but it's only the main one which will be [Solved](../../Overview/Glossary.md#solving), [Simulated](../../Overview/Glossary.md#73-simulation) or [Exported](../../Overview/Glossary.md#74-control). [Procedures](../../Overview/Glossary.md#procedure) can be added via the **+** button in the upper right-hand corner of the **Programming** screen and renamed,or removed through the menu button next to it.

---
### 3.1. Motion

#### Objective:

In this tutorial we'll look at the different ways that we can program individual [Motions](../../Overview/Glossary.md#motion-action) in FlowBuilder.

#### Requirements to follow along:

- HAL Robotics FlowBuilder installed on a PC. See [Installation](../../Overview/0-Administration-and-Setup/Contents.md#01-install) if you need to install the software.
- An open [project](../1-Getting-Started/Contents.md#11-projects)
- A [Robot](../../Overview/Glossary.md#manipulator) in the **Scene**
- A [Controller](../../Overview/Glossary.md#controller) in the **Scene**

#### Background:

[Targets](../../Overview/Glossary.md#target) are the way we define to where a [Robot](../../Overview/Glossary.md#manipulator) should move. They do not, in and of themselves, define how a [Robot](../../Overview/Glossary.md#manipulator) should move so we can mix and match [Target](../../Overview/Glossary.md#target) creation techniques with different types of [Move](../../Overview/Glossary.md#motion-action).

#### How to:

There are 2 main ways of creating [Targets](../../Overview/Glossary.md#target), both of which are available from **Target** component in the **HAL Robotics** tab, **Motion** panel.

The first way of creating a [Target](../../Overview/Glossary.md#target) is a [Cartesian](../../Overview/Glossary.md#cartesian-space) [Target](../../Overview/Glossary.md#target) from a **Frame**. When a [Robot](../../Overview/Glossary.md#manipulator) moves to a [Cartesian](../../Overview/Glossary.md#cartesian-space) [Target](../../Overview/Glossary.md#target) the active [TCP](../../Overview/Glossary.md#endpoint) of our [Robot](../../Overview/Glossary.md#manipulator) will align with the [Target's](../../Overview/Glossary.md#target) position and orientation. If we create a **Frame** in Grasshopper, such as by selecting a point and making it the origin of an XY Plane. When we assign this to our [Target](../../Overview/Glossary.md#target) component and hide the previous components, we can see our [Target](../../Overview/Glossary.md#target) axes centred on the point we had selected. Of course, because this is Grasshopper, if we move that point our [Target](../../Overview/Glossary.md#target) moves with it. We can also set the [Reference](../../Overview/Glossary.md#reference) here. Please take a look at the [References tutorial](../2-Cell/Contents.md#22-create-a-reference) to see how those work.

The Z axis of this [Target](../../Overview/Glossary.md#target) is pointing up. In our [Tool creation tutorial](../2-Cell/Contents.md#22-create-a-tool), we recommended that the Z axis of [Tool](../../Overview/Glossary.md#end-effector) [TCPs](../../Overview/Glossary.md#endpoint) point out of the [Tool](../../Overview/Glossary.md#end-effector), following the co-ordinate system flow of the Robot itself. That means that when our Robot reaches the [Target](../../Overview/Glossary.md#target) we've just created, the [Tool](../../Overview/Glossary.md#end-effector) will also be pointing up. That may be desirable but remember that setting the orientation of your [Targets](../../Overview/Glossary.md#target) is just as important as their positions and therefore creating the correct **Frame** is critical. We have found a number of cases where creating [Targets](../../Overview/Glossary.md#target) facing directly downwards, with their X axes towards world -X is a useful default and have added a shortcut to create those by-passing points directly to a [Target](../../Overview/Glossary.md#target) parameter.

The other primary way of creating a [Target](../../Overview/Glossary.md#target) is in [Joint space](../../Overview/Glossary.md#joint-space), that is by defining the desired position of each active [Joint](../../Overview/Glossary.md#joint) of your [Robot](../../Overview/Glossary.md#manipulator). We can do this by changing the template of our [Target](../../Overview/Glossary.md#target) component by right-clicking and selecting **From Joint Positions**. The inputs are now slightly different. We need to pass a [Mechanism](../../Overview/Glossary.md#mechanism) into the component to visualize the [Robot](../../Overview/Glossary.md#manipulator) in its final position and ensure that the _Positions_ we give are valid. The other required input is a list of the _Positions_ of each active [Robot](../../Overview/Glossary.md#manipulator) [Joint](../../Overview/Glossary.md#joint). It's important to note that unlike many other inputs in the HAL Robotics Framework these _Positions_ must be defined in SI units (metres and radians) because they can legitimately contain both lengths and angles. If we create a few sliders, six for a six-axis [Robot](../../Overview/Glossary.md#manipulator), merge into a single list and ensure that we're in SI units we can visualize the final position of our [Robot](../../Overview/Glossary.md#manipulator) at these [Joint](../../Overview/Glossary.md#joint) positions.

Using these two [Target](../../Overview/Glossary.md#target) creation methods we can get our [Robots](../../Overview/Glossary.md#manipulator) to perform any motion we require. That being said, particularly in a Grasshopper-centric workflow, we often want to follow a Curve as we did in the [Getting Started](../1-Getting-Started/Contents.md#1-getting-started) tutorial. To facilitate that we have included a **From Curve** template in the **Target** component. This variation of the component takes a _Curve_ and creates [Targets](../../Overview/Glossary.md#target) to approximate the _Curve_. We do this by subdividing (or discretizing) it. The _Discretization_ can be controlled using the input and changed between straight line segments only or line segments and arcs. The accuracy of the approximation can be controlled using this _Tolerance_ input. The distance given here is the maximum allowed deviation from the input _Curve_.

All of these [Target](../../Overview/Glossary.md#target) creation options give exactly the same types of [Target](../../Overview/Glossary.md#target) so can be used interchangeably in your [Move](../../Overview/Glossary.md#motion-action) components.

---
### 3.2. Wait for a Time

#### Objective:

In this tutorial we'll create a [Wait Action](../../Overview/Glossary.md#wait-action) that pauses [Robot](../../Overview/Glossary.md#manipulator) execution for a fixed period of time using the HAL Robotics Framework for Grasshopper.

#### Requirements to follow along:

- HAL Robotics FlowBuilder installed on a PC. See [Installation](../../Overview/0-Administration-and-Setup/Contents.md#01-install) if you need to install the software.
- An open [project](../1-Getting-Started/Contents.md#11-projects)
- A [Robot](../../Overview/Glossary.md#manipulator) in the **Scene**
- A [Controller](../../Overview/Glossary.md#controller) in the **Scene**

#### Background:

In certain scenarios it may be necessary to have your [Robot](../../Overview/Glossary.md#manipulator) [Wait](../../Overview/Glossary.md#wait-action) in its current position. This could be because it's taking a measurement, a [Tool](../../Overview/Glossary.md#end-effector) is working or simply because something else is happening in the environment. If the time to [Wait](../../Overview/Glossary.md#wait-action) is a constant, such as the time required for a gripper to open, then a **Wait Time** [Action](../../Overview/Glossary.md#action) is a good solution.

#### How to:

From the **Programming** screen, select the [Group](#36-structuring-procedures) into which you want to add your new [Wait](../../Overview/Glossary.md#wait-action), or click anywhere in the white space to clear your current selection. You can always drag and drop [Actions](../../Overview/Glossary.md#action) onto [Groups](#36-structuring-procedures) or in between other [Actions](../../Overview/Glossary.md#action) to restructure your [Procedure](../../Overview/Glossary.md#procedure) later. Either of those states will enable the _Item Type_ selector to list [Wait](../../Overview/Glossary.md#wait-action) as an option.

Click **+** and you'll start creating a [Wait Action](../../Overview/Glossary.md#wait-action). The default, **From Time** _Creator_ will allow you to set the time for which the robot should pause. For example, if the **Time** is set to 2 seconds, when we [Simulate](#37-validation-and-simulation) the [Robot](../../Overview/Glossary.md#manipulator) pauses for 2 seconds.

Once you are happy with the [Wait](../../Overview/Glossary.md#wait-action)'s setup, ensure the name makes it easy to identify and click **ok** in the upper right corner to return to the **Programming** screen.

---
### 3.3. Set a Signal

#### Objective:

In this tutorial we'll change the state of a [Signal](../../Overview/Glossary.md#signal) at runtime in FlowBuilder.

#### Requirements to follow along:

- HAL Robotics FlowBuilder installed on a PC. See [Installation](../../Overview/0-Administration-and-Setup/Contents.md#01-install) if you need to install the software.
- An open [project](../1-Getting-Started/Contents.md#11-projects)
- A [Robot](../../Overview/Glossary.md#manipulator) in the **Scene**
- A [Controller](../../Overview/Glossary.md#controller) in the **Scene**
- A [Signal](../../Overview/Glossary.md#signal) in your **Network**. If you don't have one, see the [Create a Signal](../2-Cell/Contents.md#26-create-a-signal) tutorial for more information.

#### Background:

Electrical Input and Output (I/O) [Signals](../../Overview/Glossary.md#signal) are used to activate or deactivate [Tools](../../Overview/Glossary.md#end-effector), trigger actions on remote machines or pass data between **Sensors**. The activation of these [Signals](../../Overview/Glossary.md#signal) needs to be triggered at the right time during program execution, something we can do easily with [Signal Actions](../../Overview/Glossary.md#signal-action).

#### How to:

In our previous tutorial, we created a digital output [Signal](../../Overview/Glossary.md#signal) within a [Controller](../../Overview/Glossary.md#controller) and assigned it an appropriate _Name_.. We now want to change the state of that [Signal](../../Overview/Glossary.md#signal) during the execution of a [Procedure](../../Overview/Glossary.md#procedure). To do so, from the **Programming** screen, select the [Group](#36-structuring-procedures) into which you want to add your new [Signal Action](../../Overview/Glossary.md#signal-action), or click anywhere in the white space to clear your current selection. You can always drag and drop [Actions](../../Overview/Glossary.md#action) onto [Groups](#36-structuring-procedures) or in between other [Actions](../../Overview/Glossary.md#action) to restructure your [Procedure](../../Overview/Glossary.md#procedure) later. Either of those states will enable the _Item Type_ selector to list [Set Signal](../../Overview/Glossary.md#signal-action) as an option.

Click **+** and you'll start creating a [Signal Action](../../Overview/Glossary.md#signal-action). You'll have a _Creator_ for each type of [Signal](../../Overview/Glossary.md#signal) available, e.g. **Set Digital Output** or **Set Analog Output**. Within each you will find a list of the relevant [Signals](../../Overview/Glossary.md#signal) in your **Network** and a **Value** which should be assigned.

Once you are happy with the [Signal Action](../../Overview/Glossary.md#signal-action)'s setup, ensure the name makes it easy to identify and click **ok** in the upper right corner to return to the **Programming** screen.

---
### 3.4. Paths

#### Objective:

In this tutorial we'll see how to combine different [Procedures](../../Overview/Glossary.md#procedure) to chain sequences using the HAL Robotics Framework for Grasshopper.

#### Requirements to follow along:

- HAL Robotics FlowBuilder installed on a PC. See [Installation](../../Overview/0-Administration-and-Setup/Contents.md#01-install) if you need to install the software.
- An open [project](../1-Getting-Started/Contents.md#11-projects)
- A [Robot](../../Overview/Glossary.md#manipulator) in the **Scene**
- A [Controller](../../Overview/Glossary.md#controller) in the **Scene**

#### Background:

[Procedures](../../Overview/Glossary.md#procedure) are sequences of atomic [Actions](../../Overview/Glossary.md#action), such as [Move](../../Overview/Glossary.md#motion-action), [Wait](../../Overview/Glossary.md#wait-action) or [Change Signal State](../../Overview/Glossary.md#signal-action). Each of these are created individually but need to be combined to be executed one after the other by our [Robot](../../Overview/Glossary.md#manipulator). [Procedures](../../Overview/Glossary.md#procedure) can also be created as reusable sequences of [Actions](../../Overview/Glossary.md#action) for example moving to a home position and opening a gripper.

#### How to:

To combine multiple [Procedures](../../Overview/Glossary.md#procedure), we can use the **Combine Actions** component from the **HAL Robotics** tab, **Procedure** panel. This component allows us to name our new [Procedure](../../Overview/Glossary.md#procedure) with the _Alias_ input. This is extremely useful for identifying your [Procedure](../../Overview/Glossary.md#procedure) later, particularly when using it more than once. The only mandatory input for this component is the list of [Procedures](../../Overview/Glossary.md#procedure) and [Actions](../../Overview/Glossary.md#action) to be _Combined_. In Grasshopper we can pass any number of wires into the same input and the **Combine Actions** component will create a [Procedure](../../Overview/Glossary.md#procedure) for each branch of items it gets. However, to ensure that we keep a clean document and an easy means of changing the order of [Procedures](../../Overview/Glossary.md#procedure) it is recommended to use something like a **Merge** component and flattening all the inputs. Once those are _Combined_, we will have single [Procedure](../../Overview/Glossary.md#procedure) that executes each of our sub-[Procedures](../../Overview/Glossary.md#procedure) one after the other.

Once a [Procedure](../../Overview/Glossary.md#procedure) has been assigned to a [Controller](../../Overview/Glossary.md#controller) and [Solved](../../Overview/Glossary.md#solving) it is useful to see how a [Simulation](../../Overview/Glossary.md#73-simulation) is progressing through that [Procedure](../../Overview/Glossary.md#procedure) so we can see where any issues may lie or which phases might be taking longer than we expect. We can do that using the **Procedure Browser**. To access the **Procedure Browser**, we need to ensure that we have an **Execution Control** connected to a complete **Execute** component. Once that's in place we can double-click on the **Execution Control** to open the **Procedure Browser**. In this window we can see our execution controls, reset, play/pause, next, previous and loop as well as all of our actions. Alongside that we have a time slider that allows you to speed up or slow down the [Simulation](../../Overview/Glossary.md#73-simulation) of your [Procedures](../../Overview/Glossary.md#procedure) without affecting your program itself. The rest of the **Procedure Browser** window shows the [Procedure](../../Overview/Glossary.md#procedure) that you are executing and the progress of each [Action](../../Overview/Glossary.md#action) within it. This **Procedure Browser** view also serves to demonstrate the purpose of the _Compact_ input on our **Combine Procedure** component. By default, _Compact_ is set to `true`. This compacts all of the incoming [Procedures](../../Overview/Glossary.md#procedure) and creates a single, flat list of [Actions](../../Overview/Glossary.md#action). If, however, we toggle _Compact_ to `false` we see that all of our previous [Procedures](../../Overview/Glossary.md#procedure) are maintained in the hierarchy and can be collapsed or expanded to view their contents. The hierarchical, un-compacted mode can be particularly useful if you reuse sub-[Procedures](../../Overview/Glossary.md#procedure).

---
### 3.5. Custom Actions

#### Objective:

In this tutorial we'll use a [Custom Action](../../Overview/Glossary.md#custom-action) to trigger an existing [Robot](../../Overview/Glossary.md#manipulator) function using FlowBuilder.

#### Requirements to follow along:

- HAL Robotics FlowBuilder installed on a PC. See [Installation](../../Overview/0-Administration-and-Setup/Contents.md#01-install) if you need to install the software.
- An open [project](../1-Getting-Started/Contents.md#11-projects)
- A [Robot](../../Overview/Glossary.md#manipulator) in the **Scene**
- A [Controller](../../Overview/Glossary.md#controller) in the **Scene**

#### Background:

When working with a fully integrated [Cell](../../Overview/Glossary.md#cell) or using a [Robot](../../Overview/Glossary.md#manipulator) with pre-built functionality which isn't natively supported by the HAL Robotics Framework, you may want add code to your export which calls an existing function in the [Controller](../../Overview/Glossary.md#controller). We do this using [Custom Actions](../../Overview/Glossary.md#custom-action). Common for [Custom Actions](../../Overview/Glossary.md#custom-action) are opening or closing a gripper, running tool change procedures, starting logging, activating collision boxes, popping up messages to the operator etc.

#### How to:

From the **Programming** screen, select the [Group](#36-structuring-procedures) into which you want to add your new [Custom Action](../../Overview/Glossary.md#custom-action), or click anywhere in the white space to clear your current selection. You can always drag and drop [Actions](../../Overview/Glossary.md#action) onto [Groups](#36-structuring-procedures) or in between other [Actions](../../Overview/Glossary.md#action) to restructure your [Procedure](../../Overview/Glossary.md#procedure) later. Either of those states will enable the _Item Type_ selector to list [Custom Action](../../Overview/Glossary.md#custom-action) as an option.

Click **+** and you'll start creating a [Custom Action](../../Overview/Glossary.md#custom-action). The main thing required here is our **Code**. This should just be the textual representation of the code that you want to export. For example if you wanted to create a pop-up message on an ABB robot you could write _TPWrite "Hello Robot";_ and that exact line of code will be exported within your program.

Other than the _Name_, which we recommend always setting, the other setting is **Simulation**. This allows you to select a [Procedure](../../Overview/Glossary.md#procedure) which will change how this [Action](../../Overview/Glossary.md#action) is simulated but won't affect how it's [Exported](../../Overview/Glossary.md#export). If you know it's going to take a second for your gripper to close, for example, you could put a [Wait](../../Overview/Glossary.md#wait-action) [Action](../../Overview/Glossary.md#action) in a [sub-procedure](#36-structuring-procedures), assign it to your **Simulation** and the program will pause when simulated but the code won't contain any [Wait](../../Overview/Glossary.md#wait-action) instructions.

Once you are happy with the [Custom Action](../../Overview/Glossary.md#custom-action)'s setup, ensure the name makes it easy to identify and click **ok** in the upper right corner to return to the **Programming** screen.

---
### 3.6. Structuring Procedures

#### Objective:

In this tutorial we'll see how to simplify the programming of multi-[Mechanism](../../Overview/Glossary.md#mechanism) setups using **Target Resolvers** in the HAL Robotics Framework for Grasshopper.

#### Requirements to follow along:

- HAL Robotics FlowBuilder installed on a PC. See [Installation](../../Overview/0-Administration-and-Setup/Contents.md#01-install) if you need to install the software.
- An open [project](../1-Getting-Started/Contents.md#11-projects)
- A [Robot](../../Overview/Glossary.md#manipulator) in the **Scene**
- A [Controller](../../Overview/Glossary.md#controller) in the **Scene**

#### Background:

When we have multiple [Robots](../../Overview/Glossary.md#manipulator) or [Mechanisms](../../Overview/Glossary.md#mechanism), such as [Positioners](../../Overview/Glossary.md#positioner), in a [Cell](../../Overview/Glossary.md#cell) programming can become increasingly complex and, in many scenarios, we only really have one set of [Targets](../../Overview/Glossary.md#target) that we care about which a [Positioner](../../Overview/Glossary.md#positioner) should be relocating to facilitate access by a [Manipulator](../../Overview/Glossary.md#manipulator). We refer to this configuration, where one [Mechanism](../../Overview/Glossary.md#mechanism) displaces the [Targets](../../Overview/Glossary.md#target) of another as **Coupled** motion.

#### How to:

As an example of this we can have a [Robot](../../Overview/Glossary.md#manipulator) drawing a straight line between two [Targets](../../Overview/Glossary.md#target) that are referenced on a rotary [Positioner](../../Overview/Glossary.md#positioner). In order to setup **Coupled** motion we need to add some settings to all the **Motion Settings** that will be used in that **Coupled** configuration. We can create **Kinematic Settings** from the **HAL Robotics** tab, **Motion** panel and because we're dealing with multiple [Mechanisms](../../Overview/Glossary.md#mechanism) we are going to change to the **Composite Kinematic Settings** template. We are now asked to input settings for the _Primary_ and _Secondary_ [Mechanisms](../../Overview/Glossary.md#mechanism). The former is typically the [Mechanism](../../Overview/Glossary.md#mechanism) that is moving the [Targets](../../Overview/Glossary.md#target) around, historically termed "Master", and the latter is the [Mechanism](../../Overview/Glossary.md#mechanism) moving to those [Targets](../../Overview/Glossary.md#target), historically termed "Slave". In our case the [Targets](../../Overview/Glossary.md#target) are referenced to the [Positioner](../../Overview/Glossary.md#positioner) and the [Robot](../../Overview/Glossary.md#manipulator) is following those [Targets](../../Overview/Glossary.md#target) around. We don't need to assign any additional **Kinematic Settings** to our [Mechanisms](../../Overview/Glossary.md#mechanism) so we can simply chain our [Mechanisms](../../Overview/Glossary.md#mechanism) into simple Kinematic Settings and into their positions in the **Composite Kinematic Settings**. These **Composite Kinematic Settings** can now be added to our **Motion Settings** for the **Coupled** [Motions](../../Overview/Glossary.md#motion-action). We have two separate sets of **Motion Settings** here; one is for the **Coupled** [Motion](../../Overview/Glossary.md#motion-action) and the other is for an asynchronous initialization [Motion](../../Overview/Glossary.md#motion-action) for each of the [Mechanisms](../../Overview/Glossary.md#mechanism).

With our settings in place we can now look at programming the [Positioner](../../Overview/Glossary.md#positioner). We could calculate the [Targets](../../Overview/Glossary.md#target) for the [Positioner](../../Overview/Glossary.md#positioner) and set them explicitly but when we're in a scenario like this welding example we can set some rules for the [Positioner](../../Overview/Glossary.md#positioner) to follow. We do this using the **Target Resolvers** from the **HAL Robotics** tab, **Motion** panel. There are a few different templates to explore but, in our case, the first is the one we want. The **Vector Aligned** **Target Resolver** tells the positioner to point the given _Axis_ of our [Targets](../../Overview/Glossary.md#target) towards a particular direction. If we can it's normally preferable to weld with Gravity so we're going to ask the [Positioner](../../Overview/Glossary.md#positioner) to point the Z axis of our [Targets](../../Overview/Glossary.md#target) straight down. The **Target Resolver** can be used in a **Move** just like a [Target](../../Overview/Glossary.md#target) provided it is duplicated to match the number of "secondary" [Targets](../../Overview/Glossary.md#target). To make that task easier we have included a template in **Move** called **Synchronized** which takes in a [Procedure](../../Overview/Glossary.md#procedure) and a [Target](../../Overview/Glossary.md#target), or **Target Resolver**, and will create all of the necessary [Moves](../../Overview/Glossary.md#motion-action) for you with the correct synchronization settings to match the input _Procedure_. **Synchronized** [Move](../../Overview/Glossary.md#motion-action) creates a [Procedure](../../Overview/Glossary.md#procedure) as an output like any other **Move** and so it can be merged and **Combined** as we would normally with any other **Move**. With both of our [Procedures](../../Overview/Glossary.md#procedure) now complete we can **Solve** and **Simulate** to see our [Positioner](../../Overview/Glossary.md#positioner) aligning itself automatically to best present the [Targets](../../Overview/Glossary.md#target) to the [Robot](../../Overview/Glossary.md#manipulator).

---
### 3.7. Validation and Simulation

#### Objective:

In this tutorial we'll see how to change the active [Tool](../../Overview/Glossary.md#end-effector) of a [Mechanism](../../Overview/Glossary.md#mechanism) during the execution of a [Procedure](../../Overview/Glossary.md#procedure) in the HAL Robotics Framework for Grasshopper.

#### Requirements to follow along:

- HAL Robotics FlowBuilder installed on a PC. See [Installation](../../Overview/0-Administration-and-Setup/Contents.md#01-install) if you need to install the software.
- An open [project](../1-Getting-Started/Contents.md#11-projects)
- A [Robot](../../Overview/Glossary.md#manipulator) in the **Scene**
- A [Controller](../../Overview/Glossary.md#controller) in the **Scene**

#### Background:

There are scenarios in which a single [Robot](../../Overview/Glossary.md#manipulator) may have access to multiple [Tools](../../Overview/Glossary.md#end-effector) and the ability to change which [Tool](../../Overview/Glossary.md#end-effector) is in use at runtime. This could be because, either, the [Tool](../../Overview/Glossary.md#end-effector) itself has multiple [Endpoints](../../Overview/Glossary.md#endpoint) or because automatic [Tool](../../Overview/Glossary.md#end-effector) changing equipment is available in the [Cell](../../Overview/Glossary.md#cell).

#### How to:

In preparation for this tutorial a number of things have been put in place. Firstly, three [Tools](../../Overview/Glossary.md#end-effector) have been created; a simple cone, an interchange plate and a double cone with 2 distinct [Endpoints](../../Overview/Glossary.md#endpoint). Secondly, these [Tools](../../Overview/Glossary.md#end-effector) have each been positioned in front of the [Robot](../../Overview/Glossary.md#manipulator) in a known location. And finally, a [Toolpath](../../Overview/Glossary.md#toolpath) has been created to go to each of the [Tool](../../Overview/Glossary.md#end-effector) picking positions with a jump in between. I have also prepared the standard **Combine**, **Solve**, and **Execute** flow.

The focus of this tutorial will be on the **Change Tool** component which can be found in the **HAL Robotics** tab under **Procedure**. Each version of this component will give us a [Procedure](../../Overview/Glossary.md#procedure) that we can Combine with [Moves](../../Overview/Glossary.md#motion-action) and [Waits](../../Overview/Glossary.md#wait-action) as we have done countless times before. The **Change Tool** component has 3 templates and we'll cover them all, starting with **Detach Tool**. We're going to use this to remove the `Cone` [Tool](../../Overview/Glossary.md#end-effector) that we've initially got attached to the [Robot](../../Overview/Glossary.md#manipulator). We want to ensure the _Mechanism_ is the combined [Mechanism](../../Overview/Glossary.md#mechanism) and we can specify the _Tool_ as the `Cone` [Tool](../../Overview/Glossary.md#end-effector). The _Tool_ input is actually optional and the currently active [Tool](../../Overview/Glossary.md#end-effector) will be removed if none is specified. We can weave this into our main [Procedure](../../Overview/Glossary.md#procedure) and if you Solve and Execute, you'll see the Cone disappear when we hit the right [Target](../../Overview/Glossary.md#target). The `Cone` is actually in the exact position we left it but it is no longer displayed because it's not part of our [Robot](../../Overview/Glossary.md#manipulator). We can use the _Environment_ input on **Execute** to force the display of mobile, non-mechanism [Parts](../../Overview/Glossary.md#part). If we now **Execute**, we should see the `Cone` hang in space where we detached it.

From here we're going to attach two [Tools](../../Overview/Glossary.md#end-effector) to the [Robot](../../Overview/Glossary.md#manipulator) in succession. The first is going to be the `Interface` which acts as something of a Tool Changer. Using the **Change Tool** component and the **Attach Tool** template we can set the combined [Mechanism](../../Overview/Glossary.md#mechanism) as the _Mechanism_ again and the `Interface` as the _Tool_. Merging this into our [Procedure](../../Overview/Glossary.md#procedure) will attach the `Interface` to our [Robot](../../Overview/Glossary.md#manipulator) and we can visualize the [Parts](../../Overview/Glossary.md#part) before they are attached using the same technique as for the `Cone`, passing the [Parts](../../Overview/Glossary.md#part) into _Environment_ on **Execute**. If we repeat this process and attach the `MultiTool` this time, you should see that the `MultiTool` gets connected to the [Active Endpoint](../../Overview/Glossary.md#endpoint) of the [Robot](../../Overview/Glossary.md#manipulator), which in this case is the end of the `Interface`. This behavior may not always be desirable e.g. stationary tools, and can be modified in the overload of **Attach Tool**.

In this final combination of [Tools](../../Overview/Glossary.md#end-effector) attached to the [Robot](../../Overview/Glossary.md#manipulator) we have two distinct potential [Endpoints](../../Overview/Glossary.md#endpoint). The final template of the **Change Tool** component allows us to set which [Endpoint](../../Overview/Glossary.md#endpoint), or [Tool](../../Overview/Glossary.md#end-effector) if you have multiple distinct [Tools](../../Overview/Glossary.md#end-effector) attached, is currently Active. We do this by specifying, once again, the combined [Mechanism](../../Overview/Glossary.md#mechanism), and the [Connection](../../Overview/Glossary.md#connection) that we want to use as the [Active Endpoint](../../Overview/Glossary.md#endpoint). To ensure consistent and deterministic output, I would recommend doing this immediately after attaching the `MultiTool` as well as when you may wish to switch between the two [Endpoints](../../Overview/Glossary.md#endpoint). With that merged and our [Tool](../../Overview/Glossary.md#end-effector) [Parts](../../Overview/Glossary.md#part) in the _Environment_ we can see everything run.

---

[Continue to: 4. Operator Workflows](../4-Workflows/Contents.md#4-operator-workflows)