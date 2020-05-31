using System;
using System.Collections.Generic;
using System.Text;

namespace HomeAssistantComputerSensors.Hass.Component
{
    class Sensor : ComponentBase
    {
        public Sensor(string object_id,string prefix) : base(object_id,prefix)
        {
            // 'none'  is used for generic sensors
            device_class = "none";
        }

    }
}
