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

namespace HomeAssistantComputerSensors
{
    class Program
    {
        static MqttClient mqttClient;
        static Configuration.BaseConfiguration configuration;

        static async Task Main(string[] args)
        {            
            var configinput = File.ReadAllText("configuration.yaml");

            var deserializer = new YamlDotNet.Serialization.Deserializer();
            configuration = deserializer.Deserialize<Configuration.BaseConfiguration>(configinput);

            Console.WriteLine(configuration.startup_message);
            Console.WriteLine(configuration.hass.uniqueid_base_prefix);
            Console.WriteLine(configuration.mqtt.broker);

            
            SendComponentConfiguration(configuration.mqtt);
            await SendTestMessage(configuration.mqtt);

            Console.WriteLine("--done--");

        }


        static void SendComponentConfiguration(Configuration.Mqtt mqttconfig)
        {
            mqttClient = new MqttClient(mqttconfig.broker);

            Sensor.Computer computer = new Sensor.Computer();

            double mem = computer.GetMemoryUsage();

            Hass.Component.Sensor computersensor = new Hass.Component.Sensor(configuration.hass.uniqueid_base_prefix, "computer_sensor");
            computersensor.SetState("memory", mem);
            computersensor.SetState("hostname", Dns.GetHostName());

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

        static async Task SendTestMessage(Configuration.Mqtt mqttconfig)
        {
            mqttClient = new MqttClient(mqttconfig.broker);
            
            Sensor.Computer computer = new Sensor.Computer();
            
            //double cpu = await computer.GetCpuUsage();
            double mem = computer.GetMemoryUsage();

            Hass.Component.Sensor computersensor = new Hass.Component.Sensor(configuration.hass.uniqueid_base_prefix,"computer_sensor");            
            computersensor.SetState("memory", mem);
            computersensor.SetState("hostname", Dns.GetHostName() );

            string topic = computersensor.StateTopic;
            string payload = computersensor.Payload;


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
