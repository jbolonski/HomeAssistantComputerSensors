using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace HomeAssistantComputerSensors.Hass.Component
{
    class ComponentBase
    {
        protected readonly string componentname = "base";

        private readonly Dictionary<string, object> states = new Dictionary<string, object>();

        public string object_id { get; set; }

        public string unique_id { get; set; }

        public ComponentBase(string objectId) : this("",objectId)
        {            
        }

        public ComponentBase(string objectIdPrefix,string objectId)
        {
            object_id = objectId;
            unique_id = string.Concat(objectIdPrefix,objectId);
        }

        public void SetState(string key, object value)
        {
            if( states.ContainsKey(key) )
            {
                states[key] = value; 
            } else
            {
                states.Add(key, value);
            }
        }

        public override string ToString()
        {
            return componentname;
        }

        public string StateTopic
        {
            get
            {
                string topic = string.Format("homeassistant/{0}/{1}/state", componentname, object_id);

                return topic;
            }
        }

        public string Payload
        {
            get
            {
                return JsonConvert.SerializeObject(states);

            }
        }

        public string DeleteComponentPayload => "";

        public List<ComponentConfiguration> GetComponentConfiguration()
        {
            List<ComponentConfiguration> Configurations = new List<ComponentConfiguration>();
            Payload payload = new Payload();            

            foreach (var state in this.states)
            {
                string stateName = state.Key;
                string componentType = this.GetType().Name.ToLower();

                payload.Add("device_class", componentType);
                payload.Add("name", this.object_id);
                payload.Add("state_topic", String.Format("homeassistant/{0}/{1}/state", componentType, this.object_id));
                payload.Add("value_template",String.Format("{{{{ value_json.{0} }}}}", stateName) );

                ComponentConfiguration config = new ComponentConfiguration
                {
                    StateName = stateName,
                    Topic = String.Format("homeassistant/{0}/{1}{2}/config", componentType, this.object_id, stateName),
                    Payload = JsonConvert.SerializeObject(payload.Values)
                };
                Configurations.Add(config);
            }

            return Configurations;
        }
        
    }
}
