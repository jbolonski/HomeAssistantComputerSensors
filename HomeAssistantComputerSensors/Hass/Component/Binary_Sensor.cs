using System;
using System.Collections.Generic;
using System.Text;

namespace HomeAssistantComputerSensors.Hass.Component
{
    class Binary_Sensor : ComponentBase
    {
        private readonly new string componentname="binary_sensor";

        public Binary_Sensor(string object_id) : base(object_id)
        {
        }
    }
}
