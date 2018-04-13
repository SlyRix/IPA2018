using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using uPLibrary.Networking.M2Mqtt;
using System.Linq;
using System.Diagnostics;

namespace CloudClient_Simulator.Model
{
    public class MqttConnection
    {
        private List<MqttClient> mqttClients;
        private Thread ConnectThread;
        private static Random random = new Random();
        private MainWindow m_Mainwindow;

        private float _intervaltime = 1;
        private uint _device_Nr = 5;
        private string _topic = "";
        private string _message = "";
        private string _host = "";
        private int _port = 8000;
        private string _username = "";
        private string _password = "";


        public float Intervaltime { get => _intervaltime; set => _intervaltime = value; }
        public uint Device_Nr { get => _device_Nr; set => _device_Nr = value; }
        public string Topic { get => _topic; set => _topic = value; }
        public string MessageText { get => _message; set => _message = value; }
        public string Host { get => _host; set => _host = value; }
        public int Port { get => _port; set => _port = value; }
        public string Password { get => _password; set => _password = value; }
        public string Username { get => _username; set => _username = value; }

        public MqttConnection(MainWindow frame)
        {
            m_Mainwindow = frame;
        }

        /// <summary>
        /// CheckError and start new Thread to Connect
        /// </summary>
        public void MQTTConnection_start()
        {
            try
            {
                CheckError();
                ConnectThread = new Thread(Connect);
                ConnectThread.Start();
            }
            catch(Exception e)
            {
                m_Mainwindow.Label_error.Content = e.Message;
                m_Mainwindow.StartSpinner.Visibility = Visibility.Hidden;
                m_Mainwindow.Start_btn.IsEnabled = true;
                m_Mainwindow.Stop_btn.IsEnabled = false;
                m_Mainwindow.Label_Status.Content = "";
            }
        }
        /// <summary>
        /// Connect to Mqtt Brocker and push Message
        /// </summary>
        public void Connect()
        {
            try
            {
                mqttClients = new List<MqttClient>();
                Stopwatch timer = new Stopwatch();
                float proc_time = 0; //Process time
                string random_str = RandomString(10);

                for (int id = 1; id <= Device_Nr; id++)
                {
                    IMqttNetworkChannel channel = new WebSocketMqttNetworkChannel("ws://" + Host +":"+ Port + "/mqtt", true);
                    mqttClients.Add(new MqttClient(Host, Port , false, MqttSslProtocols.None, null, null, channel));
                }
                Username = "guest";
                Password = "guest";
                foreach (MqttClient mqttClient in mqttClients)
                { 
                    mqttClient.Connect(random_str + "-" + (mqttClients.IndexOf(mqttClient)+1).ToString(), Username, Password);
                    proc_time += (100 / (float)Device_Nr);
                    updateStatus("Connecting " + Convert.ToInt32(proc_time) + "%");

                }
                updateStatus("Pushing");
                
                while (true)
                {
                    uint fails = 0;
                    timer.Start();
                    foreach (MqttClient mqttClient in mqttClients)
                    {
                        if(!mqttClient.IsConnected)
                        {
                            fails++;
                            m_Mainwindow.Label_error.Dispatcher.BeginInvoke(new Action(() => m_Mainwindow.Label_error.Content = fails + " Devices failes to Connect"));
                            if (fails == Device_Nr)
                            {
                                throw new System.ArgumentException("Failed Pushing");
                            }
                            
                        }
                        string[] CliendID;
                        CliendID = mqttClient.ClientId.Split('-');
                        mqttClient.Publish(Topic.Replace("<ID>", CliendID[1]), Encoding.UTF8.GetBytes(MessageText.Replace("<ID>", CliendID[1])), 2, true);
                        //Console.WriteLine(MessageText.Replace("<ID>", CliendID[1]));
                    }
                    timer.Stop();
                    
                    float remaining_time = ((Intervaltime * 1000) - timer.ElapsedMilliseconds);

                    if (remaining_time > 0)
                    {
                        Thread.Sleep(Convert.ToInt32(remaining_time));
                    }
                    else
                    {
                        Console.WriteLine("Intervall time Failed");
                    }
                    timer.Reset();

                }
            }
            catch (Exception e)
            {
                if (e.Message != "Der Thread wurde abgebrochen.") 
                    m_Mainwindow.Label_error.Dispatcher.BeginInvoke(new Action(() => m_Mainwindow.Label_error.Content = e.Message));
                    m_Mainwindow.Stop_btn.Dispatcher.BeginInvoke(new Action(() => m_Mainwindow.Stop_btn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent))));
            }
        }
        /// <summary>
        /// Disconnect all Connected Devices
        /// </summary>
        public void Disconnect()
        {
            foreach (MqttClient mqttClient in mqttClients)
            {
                mqttClient.Disconnect();
            }
            //ConnectThread.Join();
            ConnectThread.Abort();
        }

        /// <summary>
        /// Check if all GUI fields are filled in
        /// </summary>
        public void CheckError()
        {
            if (string.IsNullOrWhiteSpace(m_Mainwindow.MessageBox.Text))
            {
                throw new System.ArgumentException("Message Box is empty");
            }
            if (string.IsNullOrWhiteSpace(m_Mainwindow.TopicBox.Text))
            {
                throw new System.ArgumentException("Topic Box is empty");
            }
            if (m_Mainwindow.DeviceNr.Value == null)
            {
                throw new System.ArgumentException("Number of Devices is empty");
            }
            if (m_Mainwindow.IntervalNr.Value == null)
            {
                throw new System.ArgumentException("Interval is empty");
            }
        }

        public void updateStatus(string status)
        {
            m_Mainwindow.Label_Status.Dispatcher.BeginInvoke(new Action(() => m_Mainwindow.Label_Status.Content = status));
        }

        /// <summary> Generate a random string </summary>
        /// <returns> random string </returns>
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        ~ MqttConnection()
        {

        }
    }
}
