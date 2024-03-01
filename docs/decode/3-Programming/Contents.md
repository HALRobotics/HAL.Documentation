## 3. Programming

[3.0. Procedures](#30-procedures)

[3.1. Move](#31-move)

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

[Procedures](../../Overview/Glossary.md#procedure) are a sequence of [Actions](../../Overview/Glossary.md#action) to be executed by a [Controller](../../Overview/Glossary.md#controller). In the **Programming** screen you'll find tools to create individual, atomic [Actions](../../Overview/Glossary.md#action) like [Move](../../Overview/Glossary.md#motion-action) or [Wait](../../Overview/Glossary.md#wait-action) as well as ones which generate complex [Toolpaths](../../Overview/Glossary.md#toolpath) comprised of 10s, 100s or even 1000s of individual [Actions](../../Overview/Glossary.md#action).

Each [Robot](../../Overview/Glossary.md#manipulator) has a main [Procedure](../../Overview/Glossary.md#procedure), always the first in the list, assigned to it. You can add [Procedures](../../Overview/Glossary.md#procedure) to a [Robot](../../Overview/Glossary.md#manipulator) to help you [structure your code](#36-structuring-procedures), but it's only the main one which will be [Solved](../../Overview/Glossary.md#solving), [Simulated](../../Overview/Glossary.md#73-simulation) or [Exported](../../Overview/Glossary.md#74-control). [Procedures](../../Overview/Glossary.md#procedure) can be added via the **+** button in the upper right-hand corner of the **Programming** screen and renamed,or removed through the menu button next to it.

---
### 3.1. Move

#### Objective:

In this tutorial we'll look at the different ways that we can program individual [Motions](../../Overview/Glossary.md#motion-action) in FlowBuilder.

#### Requirements to follow along:

- HAL Robotics FlowBuilder installed on a PC. See [Installation](../../Overview/0-Administration-and-Setup/Contents.md#01-install) if you need to install the software.
- An open [project](../1-Getting-Started/Contents.md#11-projects)
- A [Robot](../../Overview/Glossary.md#manipulator) in the **Scene**
- A [Controller](../../Overview/Glossary.md#controller) in the **Scene**

#### Background:

Motions are the fundamental building block of Robot programming instructing the Robot to go from its current position to somewhere else. We define where a Robot should move using Targets and how they should get there with Motion Settings.
We can mix and match [Target](../../Overview/Glossary.md#target) creation techniques with different [Motion Spaces](../../Overview/Glossary.md#joint-space) ass we'll see below.

#### How to:

From the **Programming** screen, select the [Group](#36-structuring-procedures) into which you want to add your new [Move](../../Overview/Glossary.md#motion-action), or click anywhere in the white space to clear your current selection. You can always drag and drop [Actions](../../Overview/Glossary.md#action) onto [Groups](#36-structuring-procedures) or in between other [Actions](../../Overview/Glossary.md#action) to restructure your [Procedure](../../Overview/Glossary.md#procedure) later. Either of those states will enable the _Item Type_ selector to list [Move](../../Overview/Glossary.md#motion-action) as an option.

Click **+** and you'll start creating a [Move](../../Overview/Glossary.md#motion-action). There are two _Steps_ here which align with the _where_ and _how_ the Robot moves mentioned above.

There are 2 main ways of creating [Targets](../../Overview/Glossary.md#target), both of which are found in the **Target** _Step_.

The first way of creating a [Target](../../Overview/Glossary.md#target) is in [Joint space](../../Overview/Glossary.md#joint-space), that is by defining the desired position of each active [Joint](../../Overview/Glossary.md#joint) of your [Robot](../../Overview/Glossary.md#manipulator). The list of _Positions_ correspond to each active [Robot](../../Overview/Glossary.md#manipulator) [Joint](../../Overview/Glossary.md#joint). As you change these values, you can visualize the final position of our [Robot](../../Overview/Glossary.md#manipulator) at these [Joint](../../Overview/Glossary.md#joint) positions.

The other way of creating a [Target](../../Overview/Glossary.md#target) is a [Cartesian](../../Overview/Glossary.md#cartesian-space) [Target](../../Overview/Glossary.md#target) from a **Frame**. When a [Robot](../../Overview/Glossary.md#manipulator) moves to a [Cartesian](../../Overview/Glossary.md#cartesian-space) [Target](../../Overview/Glossary.md#target) the active [TCP](../../Overview/Glossary.md#endpoint) of our [Robot](../../Overview/Glossary.md#manipulator) will align with the [Target's](../../Overview/Glossary.md#target) position and orientation. As you change the values of that **Frame** you can see our [Target](../../Overview/Glossary.md#target) move and our Robot moves with it. A **Frame** is always relative to a [Reference](../../Overview/Glossary.md#reference) which can also be set here. Please take a look at the [References tutorial](../2-Cell/Contents.md#23-create-a-reference) to see how those work.

At the top of the **Target** _Step_ is a crucial selector. It allows us to specify the [Motion Space](../../Overview/Glossary.md#joint-space) in which we want to store the Target. _N.B. This does not affect **how** we move to the [Target](../../Overview/Glossary.md#target)._ 
Selecting [Joint Space](../../Overview/Glossary.md#joint-space) will mean that we consider only those [Joint](../../Overview/Glossary.md#joint) positions and we ignore the [Cartesian](../../Overview/Glossary.md#cartesian-space) **Frame**. If, for example, you were to change the length of your Tool or the location of a [Reference](../../Overview/Glossary.md#reference), it wouldn't matter because these are fixed values for each [Joint](../../Overview/Glossary.md#joint).
Selecting [Cartesian Space](../../Overview/Glossary.md#cartesian-space) will mean that we only consider the [Cartesian](../../Overview/Glossary.md#cartesian-space) **Frame**. If, for example, you were to change the length of your Tool or the location of a [Reference](../../Overview/Glossary.md#reference), we will recompute the values for each [Joint](../../Overview/Glossary.md#joint) to get the [Robot](../../Overview/Glossary.md#manipulator) into that position.

Either of these [Target](../../Overview/Glossary.md#target) creation options give exactly the same types of [Target](../../Overview/Glossary.md#target) so can be used with any **Motion Settings**.

**Motion Type** controls which path the [Robot](../../Overview/Glossary.md#manipulator) takes to a [Target](../../Overview/Glossary.md#target). In `Cartesian` mode the [TCP](../../Overview/Glossary.md#endpoint) moves in a very controlled manner along a straight line or arc. This is probably the easier motion type to visualize but can cause problems when moving between configurations or when trying to optimise cycle times. Moving in [Joint space](../../Overview/Glossary.md#joint-space) means that each [Joint](../../Overview/Glossary.md#joint) will move from one position to the next without consideration for the position of the [TCP](../../Overview/Glossary.md#endpoint). [Joint space](../../Overview/Glossary.md#joint-space) [Moves](../../Overview/Glossary.md#motion-action) always end in the same configuration and are not liable to [Singularities](../../Overview/Glossary.md#(kinematic)-singularity). It's often useful to start your [Procedures](../../Overview/Glossary.md#procedure) with a [Motion](../../Overview/Glossary.md#motion-action) in [Joint space](../../Overview/Glossary.md#joint-space) to ensure your [Robot](../../Overview/Glossary.md#manipulator) is always initialized to a known position and configuration. It's worth noting that when using [Joint space](../../Overview/Glossary.md#joint-space) [Motions](../../Overview/Glossary.md#motion-action) your [Toolpath](../../Overview/Glossary.md#toolpath) preview will be dotted until the [Procedure](../../Overview/Glossary.md#procedure) is [Solved](../../Overview/Glossary.md#solving) because we can't know ahead of time exactly where the [TCP](../../Overview/Glossary.md#endpoint) will go during that [Motion](../../Overview/Glossary.md#motion-action). Once [Solved](../../Overview/Glossary.md#solving), you will see the path your [TCP](../../Overview/Glossary.md#endpoint) will actually take in space.

_[Blends](../../Overview/Glossary.md#blend)_ sometimes called zones or approximations change how close the [Robot](../../Overview/Glossary.md#manipulator) needs to get to a [Target](../../Overview/Glossary.md#target) before moving on to the next. It's useful to consider your _[Blends](../../Overview/Glossary.md#blend)_ carefully because increasing their size can drastically improve cycle time by allowing the [Robot](../../Overview/Glossary.md#manipulator) to maintain speed instead of coming to a stop at each [Target](../../Overview/Glossary.md#target). _[Blends](../../Overview/Glossary.md#blend)_ are most easily visualized in _Position_. If we set a 100 mm radius [Blend](../../Overview/Glossary.md#blend), we can see a circle appear around our [Target](../../Overview/Glossary.md#target) (unless it's the very first in a Procedure). This indicates that the [Robot](../../Overview/Glossary.md#manipulator) will exactly follow our [Toolpath](../../Overview/Glossary.md#toolpath) until it gets within 100 mm of the [Target](../../Overview/Glossary.md#target), at which point it will start to deviate within that circle to keep its speed up and head towards the subsequent [Target](../../Overview/Glossary.md#target). It will exactly follow our [Toolpath](../../Overview/Glossary.md#toolpath) again when it leaves the circle. When we solve our [Procedure](../../Overview/Glossary.md#procedure), we can see the path our [TCP](../../Overview/Glossary.md#endpoint) will actually take getting close but not actually to all of our [Targets](../../Overview/Glossary.md#target).

_Speed_ settings, as the name implies, constrain the speed of your [Robot](../../Overview/Glossary.md#manipulator). They can be declared in [Cartesian space](../../Overview/Glossary.md#cartesian-space) to directly limit the position or orientation _Speed_ of the [TCP](../../Overview/Glossary.md#endpoint). You can also constrain the _Speeds_ of your [Robot's](../../Overview/Glossary.md#manipulator) [Joints](../../Overview/Glossary.md#joint) using the second overload or combine the two using the third overload. Please note that not all [Robot](../../Overview/Glossary.md#manipulator) manufacturers support programmable [Joint](../../Overview/Glossary.md#endpoint) speed constraints so there may be variations between your simulation and real [Robot](../../Overview/Glossary.md#manipulator) when they are used.

_Acceleration_ settings constrain the acceleration of your [Robot](../../Overview/Glossary.md#manipulator). They function in exactly the same way as the _Speeds_, constraining [Cartesian](../../Overview/Glossary.md#cartesian-space) acceleration, [Joint](../../Overview/Glossary.md#joint-space) acceleration or both.

Once you are happy with the [Move](../../Overview/Glossary.md#motion-action)'s setup, ensure the name makes it easy to identify and click **ok** in the upper right corner to return to the **Programming** screen.

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

In this tutorial we'll see how to simplify your programming by structuring your [Procedures](../../Overview/Glossary.md#procedure) in FlowBuilder.

#### Requirements to follow along:

- HAL Robotics FlowBuilder installed on a PC. See [Installation](../../Overview/0-Administration-and-Setup/Contents.md#01-install) if you need to install the software.
- An open [project](../1-Getting-Started/Contents.md#11-projects)
- A [Robot](../../Overview/Glossary.md#manipulator) in the **Scene**
- A [Controller](../../Overview/Glossary.md#controller) in the **Scene**

#### Background:

Once your Procedures start to get more complex, you will likely find that certain sequences of Actions are repeated. That could include moving the Robot to a home position, (de)activating a Tool or setting some collection of Signals. Even if those sequences aren't repeated, it may help the legibility of your Procedures to create a hierarchy to collect Actions into logical groups. We have two ways of doing that in FlowBuilder, **Groups** and Procedure Calls.

#### How to:

From the **Programming** screen, click in some white space to clear your selection. The _Item Type_ selector should now list **Group** and **Call**.

Starting with **Group**, click **+** and we'll enter the **Group** editor. There are no settings in here other than the name so set one that makes it easy to identify and click **ok** in the upper right corner to return to the **Programming** screen. If you then select your new **Group** you'll see that any Action can be created within it or existing Actions can be dragged in or out of it. That's all there is to **Groups**, they're there to help you keep Procedures organised.

Procedure Calls, however, are a little more involved. Before we can use one we'll need a sub-Procedure to call. In the top right-hand corner you'll see another **+** button and a three bar menu. Either of those can be used to add a new Procedure, and the menu can also be used to rename or delete your additional Procedures. As usual give this Procedure an identifiable name, add some Actions and then use the Procedure selector to return to your main Procedure. Once back in your main Procedure's editor, with a **Group** or nothing selected, select **Call** from the _Item Type_ selector and click **+** to add one. The default _Creator_ has a **Configure** _Step_ which will allow you to select which Procedure you want to call. This will automatically rename your **Call**. Once you are happy with the **Call**'s setup click **ok** in the upper right corner to return to the **Programming** screen.

---
### 3.7. Validation and Simulation

#### Objective:

In this tutorial we'll see how to Simulate our [Procedure](../../Overview/Glossary.md#procedure) in FlowBuilder to ensure it does what we expect.

#### Requirements to follow along:

- HAL Robotics FlowBuilder installed on a PC. See [Installation](../../Overview/0-Administration-and-Setup/Contents.md#01-install) if you need to install the software.
- An open [project](../1-Getting-Started/Contents.md#11-projects)
- A [Robot](../../Overview/Glossary.md#manipulator) in the **Scene**
- A [Controller](../../Overview/Glossary.md#controller) in the **Scene**
- Some Actions in your main Procedure

#### Background:

There are scenarios in which a single [Robot](../../Overview/Glossary.md#manipulator) may have access to multiple [Tools](../../Overview/Glossary.md#end-effector) and the ability to change which [Tool](../../Overview/Glossary.md#end-effector) is in use at runtime. This could be because, either, the [Tool](../../Overview/Glossary.md#end-effector) itself has multiple [Endpoints](../../Overview/Glossary.md#endpoint) or because automatic [Tool](../../Overview/Glossary.md#end-effector) changing equipment is available in the [Cell](../../Overview/Glossary.md#cell).

#### How to:

in the **Programming** screen, between the Procedure selector and the _Item Type_ selector you should see a large blue **solve** button. Clicking that will run a very fast simulation behind the scenes during which FlowBuilder will work out how the Robot is going to follow any Toolpaths you've created and check for potential issues. If your Procedure is solved that button will be replaced by a **simulation control bar** with **reset**, **play**/**pause**, **next**, **previous**, and **loop** buttons as well as a **time ratio** slider to change the playback speed of the simulation. This won't change the programmed or exported speeds, it's just like skipping faster or slower through time. On the far right-hand side of that bar is a button with some graphs. That will open the Procedure Timeline which will show you the details of your Robot's Motion and highlight any issues during the Procedure.

Every time you make a change to anything in your Procedure you'll need to re-**solve** but we'll remember what happened last time so subsequent **solve**s will be faster.

---

[Continue to: 4. Operator Workflows](../4-Workflows/Contents.md#4-operator-workflows)