using HAL.ATI.Control;
using HAL.ATI.Control.Subsystems;
using HAL.Objects.Sensors.Force;
using System;
using System.Collections.Generic;
using System.Text;

namespace HAL.Documentation.ATI
{
     class ATIControllerHelper
    {
        public ATIControllerHelper(ForceSensor6Dof forceSensor, NetBoxManager netBoxManager)
        {
            AtiController = new ATIController();
            AtiController.AddControlledObject(forceSensor);
            AtiController.SubsystemManager.Add(netBoxManager);
        }

        public ATIController AtiController { get; }
    }
}
