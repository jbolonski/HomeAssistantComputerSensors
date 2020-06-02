using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using YamlDotNet;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.TypeInspectors;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Net.NetworkInformation;

namespace HomeAssistantComputerSensors
{
    class Program
    {
        static MqttClient mqttClient;
        static Configuration.BaseConfiguration configuration;
        static string testname = "hardware_memory";


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
            //SendComponentDelete(configuration.mqtt);

            Console.WriteLine("--done--");

        }

        static void SendComponentDelete(Configuration.Mqtt mqttconfig)
        {
            mqttClient = new MqttClient(mqttconfig.broker);

            Sensor.Computer computer = new Sensor.Computer();

            double mem = computer.GetMemoryUsage();

            Hass.Component.Sensor computersensor = new Hass.Component.Sensor(configuration.hass.uniqueid_base_prefix, testname);

            var configurationList = computersensor.GetComponentConfiguration();

            mqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;
            mqttClient.Connect(mqttconfig.clientid, mqttconfig.username, mqttconfig.password);

            foreach (var config in configurationList)
            {
                Console.WriteLine("Topic = {0}", config.Topic);
                mqttClient.Publish(config.Topic, Encoding.ASCII.GetBytes(""), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
        }

        static void SendComponentConfiguration(Configuration.Mqtt mqttconfig)
        {
            mqttClient = new MqttClient(mqttconfig.broker);

            Hass.Component.Sensor computersensor = new Hass.Component.Sensor(configuration.hass.uniqueid_base_prefix, testname);

            var configurationList = computersensor.GetComponentConfiguration();

            mqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;
            mqttClient.Connect(mqttconfig.clientid, mqttconfig.username, mqttconfig.password);

            foreach (var config in configurationList)
            {
                Console.WriteLine("StateName = {0}", config.StateName);
                Console.WriteLine("Topic = {0}", config.Topic);
                Console.WriteLine("Payload = {0}", config.Payload);

                mqttClient.Publish(config.Topic, Encoding.ASCII.GetBytes(config.Payload), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
        }

        static void SendTestMessage(Configuration.Mqtt mqttconfig)
        {
            mqttClient = new MqttClient(mqttconfig.broker);
            
            Sensor.Computer computer = new Sensor.Computer();
            
            Hass.Component.Sensor computersensor = new Hass.Component.Sensor(configuration.hass.uniqueid_base_prefix, testname);            
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
