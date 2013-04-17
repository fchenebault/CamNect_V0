using CamNect.Kinect;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Kinect.Toolkit;
using ManagedUPnP;

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
        DispatcherTimer dispatcherTimer = new DispatcherTimer();


        public Start()
        {
            InitializeComponent();

            // Sensor initialisation
            this.sensorChooser = new KinectSensorChooser();
            this.kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion);

            CameraOne.loadDatabase();
            configCamWin = new ConfigCamWindow();

            Discovery disc = new Discovery(null, AddressFamilyFlags.IPv4, false);
            disc.DeviceAdded += new DeviceAddedEventHandler(CameraOne.discDeviceAdded);
            disc.Start();
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
            Views.Menu MenuPage = new Menu(kinect.sensorChooser);
            this.Content = MenuPage;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            //Views.ConfigCam ConfigCamPage = new Views.ConfigCam();
           //Var ConfigCamWindow = new NewWindow();
            //this.Content = ConfigCamPage;
            configCamWin.Show();
            
        }

    }
}
