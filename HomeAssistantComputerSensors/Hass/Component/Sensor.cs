using System;
using System.Collections.Generic;
using System.Text;

namespace HomeAssistantComputerSensors.Hass.Component
{
    class Sensor : ComponentBase
    {
        private readonly new string componentname = "sensor";

        public Sensor(string object_id,string prefix) : base(object_id,prefix)
        {
        }

    }
}
