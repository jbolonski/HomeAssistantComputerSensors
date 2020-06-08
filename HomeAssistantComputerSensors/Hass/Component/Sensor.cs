namespace HomeAssistantComputerSensors.Hass.Component
{
    class Sensor : ComponentBase
    {
        public Sensor(string object_id,string prefix) : this(object_id,prefix,"")
        {
        }

        public Sensor(string object_id, string prefix, string unitOfMeasurement) : base(object_id, prefix,unitOfMeasurement)
        {
            // 'none'  is used for generic sensors
            device_class = "none";
        }

    }
}
