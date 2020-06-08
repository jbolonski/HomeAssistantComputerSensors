namespace HomeAssistantComputerSensors.Configuration
{
    class Mqtt
    {
        public string clientid { get; set; }
        public string broker { get; set; }
        public int broker_port { get; set; }
        public string certificate { get; set; }

        public string username { get; set; }
        public string password { get; set; }
    }
}