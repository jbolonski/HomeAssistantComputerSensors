using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace HomeAssistantComputerSensors.Hass.Component
{
    class ComponentBase
    {
        protected readonly string componentType = "";
        protected string device_class = "";

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
            componentType = this.GetType().Name.ToLower();
            device_class = componentType;
            SetState("");
        }

        public void SetState(object value)
        {
            SetState("value",value);
        }

        private void SetState(string key, object value)
        {
            if (states.ContainsKey(key))
            {
                states[key] = value;
            }
            else
            {
                states.Add(key, value);
            }
        }

        public override string ToString()
        {
            return componentType;
        }

        public string StateTopic
        {
            get
            {
                string topic = string.Format("homeassistant/{0}/{1}/state", componentType, unique_id);

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
  
        public List<ComponentConfiguration> GetComponentConfiguration()
        {
            List<ComponentConfiguration> Configurations = new List<ComponentConfiguration>();
            Payload payload = new Payload();

            foreach (var state in states)
            {
                string stateName = state.Key;

                string configTopic = String.Format("homeassistant/{0}/{1}{2}/config", componentType, this.unique_id, stateName);

                if (this.device_class.ToLower() != "none") { payload.Add("device_class", this.device_class); }

                payload.Add("name", string.Format("{0}_{1}", unique_id, stateName));
                payload.Add("state_topic", StateTopic);
                payload.Add("value_template", String.Format("{{{{ value_json.{0} }}}}", stateName));
                payload.Add("unique_id", String.Format("{0}{1}{2}", componentType, this.unique_id, stateName));

                ComponentConfiguration config = new ComponentConfiguration
                {
                    StateName = stateName,
                    Topic = configTopic,
                    Payload = JsonConvert.SerializeObject(payload.Values)
                };
                Configurations.Add(config);
            }

            return Configurations;
        }
        
    }
}
