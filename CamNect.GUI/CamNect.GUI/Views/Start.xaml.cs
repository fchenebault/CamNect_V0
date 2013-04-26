using CamNect.Kinect;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Kinect.Toolkit;
using ManagedUPnP;
using System.Collections.ObjectModel;
using CamNect.Camera;

namespace CamNect.GUI.Views
{
    /// <summary>
    /// Logique d'interaction pour Start.xaml
    /// </summary>
    public partial class Start : Window
    {
        /* Variables */
        private KinectMain kinect;
        public KinectSensorChooser sensorChooser;
        public static ConfigCamWindow configCamWin;
        public static int maxFenetre;
        DispatcherTimer dispatcherTimer;

        public Start()
        {
            InitializeComponent();

            // Sensor initialisation
            this.sensorChooser = new KinectSensorChooser();
            this.kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion);

            CameraOne.loadDatabase();
            configCamWin = new ConfigCamWindow();
            configCamWin.Closed += OnCloseConfig;

            Discovery disc = new Discovery(null, AddressFamilyFlags.IPv4, false);
            disc.DeviceAdded += new DeviceAddedEventHandler(CameraOne.discDeviceAdded);
            disc.DeviceRemoved += new DeviceRemovedEventHandler(CameraOne.discDeviceRemoved);
            disc.Start();

            // Timer 
             dispatcherTimer = new DispatcherTimer();
        }

        private void OnCloseConfig(object sender, EventArgs e)
        {
            this.Content = null;
            Views.Menu MenuPage = new Menu(kinect.sensorChooser);
            this.Content = MenuPage;
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            // Timer to wait for the other view
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(7000);
            dispatcherTimer.Start();
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            this.Content = null;
            Views.Menu MenuPage = new Menu(kinect.sensorChooser);
            this.Content = MenuPage;
        }

        private void Config(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            configCamWin.Show();
        }

        private void quitOnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

    }
}
