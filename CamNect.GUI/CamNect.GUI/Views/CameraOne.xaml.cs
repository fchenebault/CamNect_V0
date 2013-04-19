﻿using System;
using System.Windows.Controls;
using CamNect.Kinect;
using CamNect.Camera;
using System.Collections.Generic;
using Microsoft.Kinect.Toolkit;
using System.Windows.Data;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect;
using System.Windows;
using ManagedUPnP;
using System.Windows.Media;
using System.IO;
using Newtonsoft.Json;


namespace CamNect.GUI.Views
{
    /// <summary>
    /// Logique d'interaction pour CameraOne.xaml
    /// </summary>
    public partial class CameraOne : UserControl
    {
        
        public static readonly DependencyProperty PageLeftEnabledProperty = DependencyProperty.Register(
            "PageLeftEnabled", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        /* Variables */
        private KinectMain kinect;
        public static List<CameraUtils> cameraList = new List<CameraUtils>();
        private static List<CamConfig> defaultConfig = new List<CamConfig>();
        private static int rank = 1;
        public KinectSensorChooser sensorChooser;

//        private static CameraPTZ cameraOne;
        public System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public System.Windows.Forms.Timer highlightTimer = new System.Windows.Forms.Timer();

        public CameraOne(KinectSensorChooser sensorChooser)
        {
            InitializeComponent();

            // Sensor initialisation
            this.kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion);
            this.sensorChooser = sensorChooser;

           
            // Use KinectMain class
            //this.buttons = new List<System.Windows.Controls.Button> { quitButton, buttonDown, buttonDownLeft, buttonDownRight, buttonLeft, buttonRight, buttonTop, buttonTopRight };
            //this.kinect = new KinectMain(this.sensorChooser.Kinect, buttons);

            /*
            kinect = new KinectMain(CameraOneGrid, kinectButton, buttons);
            kinectButton.Click += new RoutedEventHandler(this.kinect.curseur.kinectButton_Click);

            cameraOne = new CameraPTZ(new VlcControl(), CameraOnePlayer);

            Discovery disc = new Discovery(null, AddressFamilyFlags.IPv4, false);
            disc.DeviceAdded += new DeviceAddedEventHandler(discDeviceAdded);
            disc.Start();*/


            // Events for gestures
            kinect.gestureCamera.OnSwipeLeftEvent += new GestureCamera.SwipeLeftEvent(writeMessage);
            kinect.gestureCamera.OnSwipeRightEvent += new GestureCamera.SwipeRightEvent(writeMessage);
            kinect.gestureCamera.OnSwipeUpEvent += new GestureCamera.SwipeUpEvent(retourMenu);

            // Events for grip buttons
            buttonGripDown.isUseForPTZ = true;
            buttonGripDown.OnHandGrip += new KinectScrollViewer.HandGripEvent(goDown_onClick);
            buttonGripUp.isUseForPTZ = true;
            buttonGripUp.OnHandGrip += new KinectScrollViewer.HandGripEvent(goUp_onClick);
            buttonGripRight.isUseForPTZ = true;
            buttonGripLeft.OnHandGrip += new KinectScrollViewer.HandGripEvent(goLeft_onClick);
            buttonGripLeft.isUseForPTZ = true;
            buttonGripRight.OnHandGrip += new KinectScrollViewer.HandGripEvent(goRight_onClick);
            buttonGripDownLeft.isUseForPTZ = true;
            buttonGripDownLeft.OnHandGrip += new KinectScrollViewer.HandGripEvent(goDownLeft_onClick);
            buttonGripDownRight.isUseForPTZ = true;
            buttonGripDownRight.OnHandGrip += new KinectScrollViewer.HandGripEvent(goDownRight_onClick);
            buttonGripUpLeft.isUseForPTZ = true;
            buttonGripUpLeft.OnHandGrip += new KinectScrollViewer.HandGripEvent(goUpLeft_onClick);
            buttonGripUpRight.isUseForPTZ = true;
            buttonGripUpRight.OnHandGrip += new KinectScrollViewer.HandGripEvent(goUpRight_onClick);

           // video.Play();
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

        public void retourMenu()
        {
            Views.Menu Menu = new Views.Menu(this.kinect.sensorChooser);
            this.Content = Menu;
        }

        public void writeMessage()
        {
            message.Content = "Geste";
        }
       

        ////When the window is loaded
        private void CameraOne_Window_Loaded(Object sender, RoutedEventArgs e)
        {
            
        }

        public static void discDeviceAdded(object sender, DeviceAddedEventArgs a)
        {
            //cameraOne.initCamera("172.18.255.100");
            //System.Console.WriteLine(a.Device.RootHostAddresses[0].ToString());
            //cameraOne.Play();
            //a.Device.UniqueDeviceName.ToString();

            bool camExist = false;
            System.Console.WriteLine("deviceadded");

            /* On recherche d'abord une configuration sauvegardée*/
            foreach (CamConfig cfg in ConfigCamWindow.Ligne)
            {
                if (a.Device.SerialNumber.ToString() == cfg.Serie)
                {
                    camExist = true;

                    cfg.Plugged = true;
                   /* cfg.Fenetre = rank;
                    rank++;*/

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
                        System.Console.WriteLine(cfg.Modele);
                        cfg.Serie = a.Device.SerialNumber.ToString();
                        cfg.Plugged= true;
                        /*cfg.Rank = rank;
                        rank++;*/

                        ConfigCamWindow.AddCam(cfg);

                        cameraList.Add(new CameraUtils(a.Device.RootHostAddresses[0].ToString(), cfg));
                        break;
                    }
                }
            }

            ConfigCamWindow.CamRefresh();


            // ConfigCamWindow.getDg.Item
            //if (!camExist)


            //cameraOne.initCamera(a.Device.RootHostAddresses[0].ToString());
            //System.Console.WriteLine(a.Device.SerialNumber.ToString());

            //cameraOne.Play();
            /*if (cameraArray[0] is CameraPTZ)
            ((CameraPTZ)cameraArray[0]).zoomOn();*/

        }

        public void TimerStop(Object myObject, EventArgs myEventArgs)
        {
            this.timer.Stop();
            message.Content = null;
        }

        public void highlightTimer_Tick(object sender, System.EventArgs e)
        {
            this.highlightTimer.Stop();
            polygonUpLeft.IsEnabled = false;
            polygonDownLeft.IsEnabled = false;
            polygonUpRight.IsEnabled = false;
            polygonDownRight.IsEnabled = false;
            polygonDown.IsEnabled = false;
            polygonRight.IsEnabled = false;
            polygonUp.IsEnabled = false;
            polygonLeft.IsEnabled = false;
        }

        public void goRight_onClick()
        {
            // Highlighting corner polygon
            if (this.highlightTimer != null)
            {
                this.highlightTimer.Stop();
            }
            this.highlightTimer.Tick += new System.EventHandler(highlightTimer_Tick);
            this.highlightTimer.Interval = 40;
            this.highlightTimer.Start();
            polygonRight.IsEnabled = true;

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 2000;
                this.timer.Start();
                System.Console.WriteLine("Button Right");
                message.Content = "Button Right";
                //cameraOne.goRight();
            }
        }

        public void goLeft_onClick()
        {
            // Highlighting corner polygon
            if (this.highlightTimer != null)
            {
                this.highlightTimer.Stop();
            }
            this.highlightTimer.Tick += new System.EventHandler(highlightTimer_Tick);
            this.highlightTimer.Interval = 40;
            this.highlightTimer.Start();
            polygonLeft.IsEnabled = true;

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 2000;
                this.timer.Start();
                System.Console.WriteLine("Button Left");
                message.Content = "Button Left";
                //cameraOne.goLeft();
            }
        }

        public void goUp_onClick()
        {
            // Highlighting corner polygon
            if (this.highlightTimer != null)
            {
                this.highlightTimer.Stop();
            }
            this.highlightTimer.Tick += new System.EventHandler(highlightTimer_Tick);
            this.highlightTimer.Interval = 40;
            this.highlightTimer.Start();
            polygonUp.IsEnabled = true;

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 2000;
                this.timer.Start();
                System.Console.WriteLine("Button Top");
                message.Content = "Button Up";
                //cameraOne.goUp();
            }
        }

        public void goUpLeft_onClick()
        {
            // Highlighting corner polygon
            if (this.highlightTimer != null)
            {
                this.highlightTimer.Stop();
            }
            this.highlightTimer.Tick += new System.EventHandler(highlightTimer_Tick);
            this.highlightTimer.Interval = 40;
            this.highlightTimer.Start();
            polygonUpLeft.IsEnabled = true;
            
            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 3000;
                this.timer.Start();
                System.Console.WriteLine("Button TopLeft");
                message.Content = "Button UpLeft";
                //cameraOne.goUp();
                //cameraOne.goLeft();
            }
        }

        public void goUpRight_onClick()
        {
            // Highlighting corner polygon
            if (this.highlightTimer != null)
            {
                this.highlightTimer.Stop();
            }
            this.highlightTimer.Tick += new System.EventHandler(highlightTimer_Tick);
            this.highlightTimer.Interval = 40;
            this.highlightTimer.Start();
            polygonUpRight.IsEnabled = true;

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 3000;
                this.timer.Start();
                System.Console.WriteLine("Button TopRight");
                message.Content = "Button UpRight";
                //cameraOne.goUp();
                //cameraOne.goRight();
            }
            
        }

        public void goDown_onClick()
        {
            // Highlighting corner polygon
            if (this.highlightTimer != null)
            {
                this.highlightTimer.Stop();
            }
            this.highlightTimer.Tick += new System.EventHandler(highlightTimer_Tick);
            this.highlightTimer.Interval = 40;
            this.highlightTimer.Start();
            polygonDown.IsEnabled = true;

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 2000;
                this.timer.Start();
                System.Console.WriteLine("Button Down");
                message.Content = "Button Down";
                //cameraOne.goDown();
            }
        }

        public void goDownRight_onClick()
        {
            // Highlighting corner polygon
            if (this.highlightTimer != null)
            {
                this.highlightTimer.Stop();
            }
            this.highlightTimer.Tick += new System.EventHandler(highlightTimer_Tick);
            this.highlightTimer.Interval = 40;
            this.highlightTimer.Start();
            polygonDownRight.IsEnabled = true;

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 3000;
                this.timer.Start();
                System.Console.WriteLine("Button DownRight");
                message.Content = "Button DownRight";
                //cameraOne.goDown();
                //cameraOne.goRight();
            }
            
        }

        public void goDownLeft_onClick()
        {
            // Highlighting corner polygon
            if (this.highlightTimer != null)
            {
                this.highlightTimer.Stop();
            }
            this.highlightTimer.Tick += new System.EventHandler(highlightTimer_Tick);
            this.highlightTimer.Interval = 40;
            this.highlightTimer.Start();
            polygonDownLeft.IsEnabled = true;

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 3000;
                this.timer.Start();
                System.Console.WriteLine("Button DownLeft");
                message.Content = "Button  DownLeft";
                //cameraOne.goDown();
                //cameraOne.goLeft();
            }
        }

        public void gestureEvent(object sender, EventArgs e)
        {
            Skeleton[] skeletons = kinectRegion.skeletons;
            if (skeletons != null)
            {

                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingId == kinectRegion.PrimaryUserTrackingId)
                    {
                        kinect.gestureCamera.OnGesture(skeletons[i]);
                    }
                }
            }
        }

        public void quit_onClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void actionForGrip()
        {
            message.Content = "DragOver";
        }

    }
}
