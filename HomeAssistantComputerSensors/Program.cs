using HomeAssistantComputerSensors.Hass.Component;
using System;
using System.IO;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace HomeAssistantComputerSensors
{
    class Program
    {
        static MqttClient mqttClient;
        static Configuration.BaseConfiguration configuration;
        static System.Timers.Timer _timer;

        static void Main(string[] args)
        {            
            var configinput = File.ReadAllText("configuration.yaml");

            var deserializer = new YamlDotNet.Serialization.Deserializer();
            configuration = deserializer.Deserialize<Configuration.BaseConfiguration>(configinput);

            Console.WriteLine(configuration.startup_message);
            Console.WriteLine(configuration.hass.uniqueid_base_prefix);
            Console.WriteLine(configuration.mqtt.broker);
          
            SendComponentConfiguration(configuration.mqtt);
            SendTestMessage(configuration.mqtt);
            _timer = new System.Timers.Timer(30 * 1000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

            //SendComponentDelete(configuration.mqtt);

            Console.WriteLine("PRESS [ANY KEY] TO QUIT");
            Console.ReadKey();

            Console.WriteLine("--done--");

        }

        private static void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SendTestMessage(configuration.mqtt);
        }


        // Delete is the Configuration Topic with no Payload
        static void SendComponentDelete(Configuration.Mqtt mqttconfig)
        {
            mqttClient = new MqttClient(mqttconfig.broker);
            mqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;
            mqttClient.Connect(mqttconfig.clientid, mqttconfig.username, mqttconfig.password);

            Hass.Component.Sensor computersensor = new Hass.Component.Sensor(configuration.hass.uniqueid_base_prefix, "hardware_memory");
            var config = computersensor.GetComponentConfiguration();
            PrintConfig(config);
            mqttClient.Publish(config.Topic, Encoding.ASCII.GetBytes(""), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);            

        }

        static void SendComponentConfiguration(Configuration.Mqtt mqttconfig)
        {
            mqttClient = new MqttClient(mqttconfig.broker);
            mqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;
            mqttClient.Connect(mqttconfig.clientid, mqttconfig.username, mqttconfig.password);

            Hass.Component.Sensor computermemorysensor = new Hass.Component.Sensor(configuration.hass.uniqueid_base_prefix, "hardware_memory", "Mb");
            var config = computermemorysensor.GetComponentConfiguration();
            PrintConfig(config);

            mqttClient.Publish(config.Topic, Encoding.ASCII.GetBytes(config.Payload), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);            
        }

        static void PrintConfig(ComponentConfiguration config)
        {
            Console.WriteLine("StateName = {0}", config.StateName);
            Console.WriteLine("Topic = {0}", config.Topic);
            Console.WriteLine("Payload = {0}", config.Payload);
        }

        static void SendTestMessage(Configuration.Mqtt mqttconfig)
        {
            mqttClient = new MqttClient(mqttconfig.broker);
            
            Sensor.Computer computer = new Sensor.Computer();
            
            Hass.Component.Sensor computersensor = new Hass.Component.Sensor(configuration.hass.uniqueid_base_prefix, "hardware_memory");            
            computersensor.SetState( computer.GetMemoryUsage() );            

            string topic = computersensor.StateTopic;
            string payload = computersensor.Payload;
            Console.WriteLine(topic);
            Console.WriteLine(payload);

            mqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;
            mqttClient.Connect(mqttconfig.clientid, mqttconfig.username, mqttconfig.password);
            mqttClient.Publish(topic, Encoding.ASCII.GetBytes(payload), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);   
        }

        private static void MqttClient_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Console.WriteLine("msg Sent");
            ((MqttClient)sender).Disconnect();
        }
    }
}
