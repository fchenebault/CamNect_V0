using System;
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
using System.Windows.Shapes;


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
        public List<Polygon> polygons;
        private List<KinectHoverButton> hoverButtons;
        private bool isButtonActive = false;
        
        private MjpegReader reader;
//        private static CameraPTZ cameraOne;
        public System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public System.Windows.Forms.Timer highlightTimer = new System.Windows.Forms.Timer();

        public CameraOne(KinectSensorChooser sensorChooser, CameraUtils camera)
        {
            InitializeComponent();
            polygons = new List<Polygon> { polygonDownLeft, polygonDown, polygonUp, polygonDownRight, polygonLeft, polygonRight, polygonUpLeft, polygonUpRight };
            hoverButtons = new List<KinectHoverButton> { buttonDown, buttonDownLeft, buttonDownRight, buttonLeft, buttonRight, buttonTop, buttonTopLeft, buttonTopRight };
            foreach (KinectHoverButton hoverButton in this.hoverButtons)
            {
                hoverButton.Visibility = System.Windows.Visibility.Hidden;
            }

            // Sensor initialisation
            this.kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion);
            this.sensorChooser = sensorChooser;

            reader = new MjpegReader(camera, CameraOnePlayer);
           
            // Events for gestures
            kinect.gestureCamera.OnSwipeLeftEvent += new GestureCamera.SwipeLeftEvent(writeMessage);
            kinect.gestureCamera.OnSwipeRightEvent += new GestureCamera.SwipeRightEvent(writeMessage);
            kinect.gestureCamera.OnSwipeUpEvent += new GestureCamera.SwipeUpEvent(retourMenu);

            // Events for grip buttons
            backgroundGrip.isCameraOne = true;
            backgroundGrip.OnHandGrip += new KinectScrollViewer.HandGripEvent(activeButtons);
           
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

            bool camExist = false;
            System.Console.WriteLine("deviceadded");

            /* On recherche d'abord une configuration sauvegardée*/
            foreach (CamConfig cfg in ConfigCamWindow.Ligne)
            {
                if (a.Device.SerialNumber.ToString() == cfg.Serie)
                {
                    camExist = true;

                    cfg.Plugged = true;
                    cfg.Fenetre = rank;
                    rank++;

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
                        cfg.Fenetre = rank;
                        rank++;

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
            polygonUpLeft.IsHitTestVisible = false;
            polygonDownLeft.IsHitTestVisible = false;
            polygonUpRight.IsHitTestVisible = false;
            polygonDownRight.IsHitTestVisible = false;
        }

        public void goRight_onClick(object sender, RoutedEventArgs e)
        {
            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 2000;
                this.timer.Start();
                System.Console.WriteLine("Button Right");
                message.Content = "Button Right";
            }
        }

        public void goLeft_onClick(object sender, RoutedEventArgs e)
        {
            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 2000;
                this.timer.Start();
                System.Console.WriteLine("Button Left");
                message.Content = "Button Left";
            }
        }

        public void goUp_onClick(object sender, RoutedEventArgs e)
        {
            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 2000;
                this.timer.Start();
                System.Console.WriteLine("Button Top");
                message.Content = "Button Up";
            }
        }

        public void goUpLeft_onClick(object sender, RoutedEventArgs e)
        {
            // Highlighting corner polygon
            if (this.highlightTimer != null)
            {
                this.highlightTimer.Stop();
            }
            this.highlightTimer.Tick += new System.EventHandler(highlightTimer_Tick);
            this.highlightTimer.Interval = 40;
            this.highlightTimer.Start();
            polygonUpLeft.IsHitTestVisible = true;

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 3000;
                this.timer.Start();
                System.Console.WriteLine("Button TopLeft");
                message.Content = "Button UpLeft";
            }
        }

        public void goUpRight_onClick(object sender, RoutedEventArgs e)
        {
            // Highlighting corner polygon
            if (this.highlightTimer != null)
            {
                this.highlightTimer.Stop();
            }
            this.highlightTimer.Tick += new System.EventHandler(highlightTimer_Tick);
            this.highlightTimer.Interval = 40;
            this.highlightTimer.Start();
            polygonUpRight.IsHitTestVisible = true;

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 3000;
                this.timer.Start();
                System.Console.WriteLine("Button TopRight");
                message.Content = "Button UpRight";
            }
            
        }

        public void goDown_onClick(object sender, RoutedEventArgs e)
        {
            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 2000;
                this.timer.Start();
                System.Console.WriteLine("Button Down");
                message.Content = "Button Down";
            }
        }

        public void goDownRight_onClick(object sender, RoutedEventArgs e)
        {
            // Highlighting corner polygon
            if (this.highlightTimer != null)
            {
                this.highlightTimer.Stop();
            }
            this.highlightTimer.Tick += new System.EventHandler(highlightTimer_Tick);
            this.highlightTimer.Interval = 40;
            this.highlightTimer.Start();
            polygonDownRight.IsHitTestVisible = true;

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 3000;
                this.timer.Start();
                System.Console.WriteLine("Button DownRight");
                message.Content = "Button DownRight";
            }
            
        }

        public void goDownLeft_onClick(object sender, RoutedEventArgs e)
        {
            // Highlighting corner polygon
            if (this.highlightTimer != null)
            {
                this.highlightTimer.Stop();
            }
            this.highlightTimer.Tick += new System.EventHandler(highlightTimer_Tick);
            this.highlightTimer.Interval = 40;
            this.highlightTimer.Start();
            polygonDownLeft.IsHitTestVisible = true;

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 3000;
                this.timer.Start();
                System.Console.WriteLine("Button DownLeft");
                message.Content = "Button  DownLeft";
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
               
        public void activeButtons()
        {
            if (isButtonActive)
            {
                foreach (Polygon polygon in this.polygons)
                {
                    polygon.IsEnabled = false;
                }
                foreach (KinectHoverButton hoverButton in this.hoverButtons)
                {
                    hoverButton.Visibility = System.Windows.Visibility.Hidden;
                }
                isButtonActive = false;
            }
            else
            {
                foreach (Polygon polygon in this.polygons)
                {
                    polygon.IsEnabled = true;
                }
                foreach (KinectHoverButton hoverButton in this.hoverButtons)
                {
                    hoverButton.Visibility = System.Windows.Visibility.Visible;
                }
                isButtonActive = true;
            }
        }

    }
}
