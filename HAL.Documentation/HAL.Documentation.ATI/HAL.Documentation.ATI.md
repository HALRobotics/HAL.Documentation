**Title:** HAL.Documentation.ATI

**Summary:** Get data from the ATI force sensor

**Packages:**

 * *HAL.ATI* allows to acquire data from the ATI force sensor. Other force sensor can be used with the corresponding library

* *HAL.ABB* is used to get the robot's positions. This data is used to correct the force sensor value.
* *HAL.Documentation.Base* contains the monitor. It allows to synchronise the acquisition from the the controller and the sensor.

**Details:** The ATI is a 6 degree of freedom force sensor. When attached a robotic arm it will be influenced by the gravity depending on its orientation. Those forces can be compensated knowing the orientation of the sensor, hence of the robot flange position in the world. The data from the robot are monitored with the ATI data to provide this corretion. this example shows how to get both raw and corretected data. 