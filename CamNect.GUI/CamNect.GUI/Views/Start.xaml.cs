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
using System.IO;
using Newtonsoft.Json;
using System.Windows.Shapes;
using System.Collections.Generic;

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
        private static List<CamConfig> defaultConfig;
        public static List<CameraUtils> cameraList;

        public Start()
        {
            InitializeComponent();

            // Sensor initialisation
            this.sensorChooser = new KinectSensorChooser();
            this.kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion);

            // camera initialisation
            defaultConfig = new List<CamConfig>();
            cameraList = new List<CameraUtils>();
            loadDatabase();
            configCamWin = new ConfigCamWindow();
            configCamWin.Deactivated += OnCloseConfig;

            Discovery disc = new Discovery(null, AddressFamilyFlags.IPv4, false);
            disc.DeviceAdded += new DeviceAddedEventHandler(discDeviceAdded);
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
            Views.Menu MenuPage = new Menu(kinect.sensorChooser, cameraList, configCamWin);
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

        public static void loadDatabase()
        {
            String json = null;

            if (!File.Exists("../../Ressources/Config/defaultconfig.json"))
            {
                StreamWriter jsonfile = new StreamWriter("../../Ressources/Config/defaultconfig.json", false);
                jsonfile.WriteLine("[]");
                jsonfile.Close();
            }
            else
            {
                json = File.ReadAllText("../../Ressources/Config/defaultconfig.json");
                defaultConfig = JsonConvert.DeserializeObject<List<CamConfig>>(json);
            }
        }

        public static void discDeviceAdded(object sender, DeviceAddedEventArgs a)
        {

            bool camExist = false;

            /* Find a configuration */
            foreach (CamConfig cfg in ConfigCamWindow.ligne)
            {
                if (a.Device.SerialNumber.ToString() == cfg.Serie)
                {
                    camExist = true;

                    cfg.Plugged = true;

                    cameraList.Add(new CameraUtils(a.Device.RootHostAddresses[0].ToString(), cfg));
                    break;
                }
            }

            if (!camExist)
            {
                foreach (CamConfig cfg in defaultConfig)
                {
                    if (a.Device.FriendlyName.Contains(cfg.Modele))
                    {
                        System.Console.WriteLine("-- NOUVELLE CAMERA --");
                        System.Console.WriteLine(cfg.Modele);
                        System.Console.WriteLine(a.Device.SerialNumber.ToString());

                        cfg.Serie = a.Device.SerialNumber.ToString();
                        cfg.Plugged = true;

                        CamConfig cfgAux = new CamConfig();
                        cfgAux.Clone(cfg);

                        ConfigCamWindow.AddCam(cfgAux);

                        cameraList.Add(new CameraUtils(a.Device.RootHostAddresses[0].ToString(), cfgAux));
                        break;
                    }
                }
            }

            ConfigCamWindow.CamRefresh();

        }


    }
}
