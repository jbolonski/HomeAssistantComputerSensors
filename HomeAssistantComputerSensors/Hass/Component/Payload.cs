using System;
using System.Collections.Generic;
using System.Text;

namespace HomeAssistantComputerSensors.Hass.Component
{
    class Payload
    {
        public Dictionary<string, object> Values { get; set; } = new Dictionary<string, object>();

        public void Add(string key, object value)
        {
            if (Values.ContainsKey(key))
            {
                Values[key] = value;
            }
            else
            {
                Values.Add(key, value);
            }
        }
    }
}
