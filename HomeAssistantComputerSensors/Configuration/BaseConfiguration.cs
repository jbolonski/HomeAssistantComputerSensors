﻿using System.Collections.Generic;

namespace HomeAssistantComputerSensors.Configuration
{
    class BaseConfiguration
    {
        public string startup_message { get; set; }

        public Mqtt mqtt { get; set; }
        public Hass hass { get; set; }

        public List<Service> services { get; set; }
    }
}
