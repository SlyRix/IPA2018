//------------------------------------------------------------------------------
//  Copyright (C) Siemens Schweiz AG 2018.
//  Licensed under the Siemens Inner Source License, see LICENSE.md.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using CloudClient_Simulator.Model;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Windows.Controls.Primitives;

namespace CloudClient_Simulator
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public const string VERSION = "1.0b";
        MqttConnection mqttconnection;

        public MainWindow()
        {
            InitializeComponent();
            Label_error.Content = "";
            Label_Version.Content = "V" + VERSION;
            Stop_btn.IsEnabled = false;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void close_img_MouseEnter(object sender, MouseEventArgs e)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("images/icons8-delete-64 (1).png", UriKind.Relative);
            logo.EndInit();
            close_img.Source = logo;
        }

        private void close_img_MouseLeave(object sender, MouseEventArgs e)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("Images/icons8-delete-64.png", UriKind.Relative);
            logo.EndInit();
            close_img.Source = logo;
        }

        private void mini_img_MouseEnter(object sender, MouseEventArgs e)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("Images/icons8-minimize-window-64 (3).png", UriKind.Relative);
            logo.EndInit();
            mini_img.Source = logo;
        }

        private void mini_img_MouseLeave(object sender, MouseEventArgs e)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("Images/icons8-minimize-window-64 (2).png", UriKind.Relative);
            logo.EndInit();
            mini_img.Source = logo;
        }

        private void maxi_img_MouseEnter(object sender, MouseEventArgs e)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("Images/icons8-maximize-window-64 (1).png", UriKind.Relative);
            logo.EndInit();
            maxi_img.Source = logo;
        }

        private void maxi_img_MouseLeave(object sender, MouseEventArgs e)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("Images/icons8-maximize-window-64.png", UriKind.Relative);
            logo.EndInit();
            maxi_img.Source = logo;
        }

        private void maxi_img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void mini_img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void close_img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void Start_btn_Click(object sender, RoutedEventArgs e)
        {
            mqttconnection = new MqttConnection(this);
            mqttconnection.Host = URL_Box.Text;
            mqttconnection.Port = Convert.ToInt32(Port_Box.Text);
            mqttconnection.Topic = TopicBox.Text;
            mqttconnection.MessageText = MessageBox.Text;
            mqttconnection.Intervaltime = (float)IntervalNr.Value;
            mqttconnection.Device_Nr = Convert.ToUInt32(DeviceNr.Value);
            StartSpinner.Visibility = Visibility.Visible;
            Start_btn.IsEnabled = false;
            Stop_btn.IsEnabled = true;
            Label_error.Content = "";
            Label_Status.Content = "Connecting    ";
            mqttconnection.MQTTConnection_start();
        }

        private void Stop_btn_Click(object sender, RoutedEventArgs e)
        {
            StartSpinner.Visibility = Visibility.Hidden;
            Thread Disconnect = new Thread(mqttconnection.Disconnect);
            Disconnect.Start();
            Start_btn.IsEnabled = true;
            Stop_btn.IsEnabled = false;
            Label_Status.Content = "";
        }

        ~ MainWindow()
        {

        }


    }
}
