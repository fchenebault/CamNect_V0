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
        //private static int fenetre = ConfigCamWindow.maxFenetre;
        public KinectSensorChooser sensorChooser;
        public List<Polygon> polygons;
        private List<KinectHoverButton> hoverButtons;
        private bool isButtonActive = false;
        private MjpegReader reader;
//        private static CameraPTZ cameraOne;
        public System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public System.Windows.Forms.Timer highlightTimer = new System.Windows.Forms.Timer();

        private List<CameraUtils> cameraListTMP;
        private int indice;
        private CameraUtils camera;

        private Double xZoom=-1;

        public CameraOne(KinectSensorChooser sensorChooser, List<CameraUtils> cameraListTMP, int indice)
        {
            InitializeComponent();
            polygons = new List<Polygon> { polygonOverDownLeft, polygonDown, polygonUp, polygonOverDownRight, polygonLeft, polygonRight, polygonOverUpLeft, polygonOverUpRight, polygonFlecheDown, polygonFlecheDownLeft, polygonFlecheDownRight, polygonFlecheLeft, polygonFlecheRight, polygonFlecheUp, polygonFlecheUpLeft, polygonFlecheUpRight };
            hoverButtons = new List<KinectHoverButton> { buttonDown, buttonDownLeft, buttonDownRight, buttonLeft, buttonRight, buttonTop, buttonTopLeft, buttonTopRight };
            foreach (KinectHoverButton hoverButton in this.hoverButtons)
            {
                hoverButton.Visibility = System.Windows.Visibility.Hidden;
            }

            // Sensor initialisation
            this.kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion,true);
           // this.kinect.InitKinect(sensorChooserUi, kinectRegion);
       //     this.sensorChooser = sensorChooser;

            // Camera Initialisation
            this.camera = cameraListTMP[indice];
            this.cameraListTMP = cameraListTMP;
            this.indice = indice;
            reader = new MjpegReader(camera, CameraOnePlayer, camera.Config.HighRes);


            // Events for gestures
            this.kinect.gestureCamera.OnSwipeLeftEvent += new GestureCamera.SwipeLeftEvent(swipeLeftAction);
            this.kinect.gestureCamera.OnSwipeRightEvent += new GestureCamera.SwipeRightEvent(swipeRightAction);
            this.kinect.gestureCamera.OnSwipeUpEvent += new GestureCamera.SwipeUpEvent(retourMenu);

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
            this.Content = null;
            reader.MjpegReaderStop();

            this.kinect.gestureCamera.OnSwipeLeftEvent -= new GestureCamera.SwipeLeftEvent(swipeLeftAction);
            this.kinect.gestureCamera.OnSwipeRightEvent -= new GestureCamera.SwipeRightEvent(swipeRightAction);
            this.kinect.gestureCamera.OnSwipeUpEvent -= new GestureCamera.SwipeUpEvent(retourMenu);
            this.kinect.gestureCamera.stopEventGesture();

            Views.Menu Menu = new Views.Menu(kinect.sensorChooser, true);
            this.Content = Menu;


        }

        public void swipeLeftAction()
        {
            this.kinect.gestureCamera.OnSwipeLeftEvent -= new GestureCamera.SwipeLeftEvent(swipeLeftAction);
            this.kinect.gestureCamera.OnSwipeRightEvent -= new GestureCamera.SwipeRightEvent(swipeRightAction);
            this.kinect.gestureCamera.OnSwipeUpEvent -= new GestureCamera.SwipeUpEvent(retourMenu);
            this.kinect.gestureCamera.stopEventGesture();
            int nb = cameraListTMP.Count;
            do {
            indice--;
            if (indice < 0)
                indice = nb - 1;
            } while (!cameraListTMP[indice].Config.Afficher);

            // Select if the camera is PTZ
            if (cameraListTMP[indice].Config.isPtz)
            {
                this.Content = null;
                reader.MjpegReaderStop();
                Views.CameraOne CameraOnePage = new Views.CameraOne(kinect.sensorChooser, cameraListTMP, indice);
                this.Content = CameraOnePage;
            }
            else
            {
                this.Content = null;
                reader.MjpegReaderStop();
                Views.CameraNotPTZ cameraNotPTZPage = new Views.CameraNotPTZ(kinect.sensorChooser, cameraListTMP, indice);
                this.Content = cameraNotPTZPage;
            }

        }

        public void swipeRightAction()
        {
            this.kinect.gestureCamera.OnSwipeLeftEvent -= new GestureCamera.SwipeLeftEvent(swipeLeftAction);
            this.kinect.gestureCamera.OnSwipeRightEvent -= new GestureCamera.SwipeRightEvent(swipeRightAction);
            this.kinect.gestureCamera.OnSwipeUpEvent -= new GestureCamera.SwipeUpEvent(retourMenu);
            this.kinect.gestureCamera.stopEventGesture();
            int nb = cameraListTMP.Count;
            do
            {
                indice++;
                if (indice > nb - 1)
                    indice = 0;
            } while (!cameraListTMP[indice].Config.Afficher);

            // Select if the camera is PTZ
            if (cameraListTMP[indice].Config.isPtz)
            {
                this.Content = null;
                reader.MjpegReaderStop();
                Views.CameraOne CameraOnePage = new Views.CameraOne(kinect.sensorChooser, cameraListTMP, indice);
                this.Content = CameraOnePage;
            }
            else
            {
                this.Content = null;
                reader.MjpegReaderStop();
                Views.CameraNotPTZ cameraNotPTZPage = new Views.CameraNotPTZ(kinect.sensorChooser, cameraListTMP, indice);
                this.Content = cameraNotPTZPage;
            }
        }
       

        ////When the window is loaded
        private void CameraOne_Window_Loaded(Object sender, RoutedEventArgs e)
        {
            
        }
       
        public static void discDeviceAdded(object sender, DeviceAddedEventArgs a)
        {

            bool camExist = false;
            System.Console.WriteLine("deviceadded");
           // System.Console.WriteLine(fenetre);
            System.Console.WriteLine(a.Device.ModelName);
            /* On recherche d'abord une configuration sauvegardée*/
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
                        //fenetre = ConfigCamWindow.maxFenetre++;
                        System.Console.WriteLine("-- NOUVELLE CAMERA --");
                        System.Console.WriteLine(cfg.Modele);
                        System.Console.WriteLine(a.Device.SerialNumber.ToString());

                        cfg.Serie = a.Device.SerialNumber.ToString();
                        cfg.Plugged= true;
                        //cfg.Fenetre = fenetre;

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


        public void TimerStop(Object myObject, EventArgs myEventArgs)
        {
            this.timer.Stop();
        }

        private void highlightTimer_Tick(object sender, System.EventArgs e)
        {
            this.highlightTimer.Stop();
            polygonUpLeft.IsHitTestVisible = false;
            polygonDownLeft.IsHitTestVisible = false;
            polygonUpRight.IsHitTestVisible = false;
            polygonDownRight.IsHitTestVisible = false;
        }

        private void highlightPolygon(Polygon polygon)
        {
            // Highlighting corner polygon
            if (this.highlightTimer != null)
            {
                this.highlightTimer.Stop();
            }
            this.highlightTimer.Tick += new System.EventHandler(highlightTimer_Tick);
            this.highlightTimer.Interval = 40;
            this.highlightTimer.Start();
            polygon.IsHitTestVisible = true;
        }

        public void goRight_onClick(object sender, RoutedEventArgs e)
        {
            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 1000;
                this.timer.Start();
                System.Console.WriteLine("Button Right");
                this.camera.goRight();
            }
        }

        public void goLeft_onClick(object sender, RoutedEventArgs e)
        {
            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 1000;
                this.timer.Start();
                System.Console.WriteLine("Button Left");
                this.camera.goLeft();
            }
        }

        public void goUp_onClick(object sender, RoutedEventArgs e)
        {
            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 1000;
                this.timer.Start();
                System.Console.WriteLine("Button Top");
                this.camera.goUp();
            }
        }

        public void goUpLeft_onClick(object sender, RoutedEventArgs e)
        {
            highlightPolygon(polygonUpLeft);

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 1000;
                this.timer.Start();
                System.Console.WriteLine("Button TopLeft");
                this.camera.goUpLeft();
            }
        }

        public void goUpRight_onClick(object sender, RoutedEventArgs e)
        {
            highlightPolygon(polygonUpRight);

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 1000;
                this.timer.Start();
                System.Console.WriteLine("Button TopRight");
                this.camera.goUpRight();
            }
            
        }

        public void goDown_onClick(object sender, RoutedEventArgs e)
        {
            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 1000;
                this.timer.Start();
                System.Console.WriteLine("Button Down");
                this.camera.goDown();
            }
        }

        public void goDownRight_onClick(object sender, RoutedEventArgs e)
        {
            highlightPolygon(polygonDownRight);

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 1000;
                this.timer.Start();
                System.Console.WriteLine("Button DownRight");
                this.camera.goDownRight();
            }
            
        }

        public void goDownLeft_onClick(object sender, RoutedEventArgs e)
        {
            highlightPolygon(polygonDownLeft);

            // Execute action
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 1000;
                this.timer.Start();
                System.Console.WriteLine("Button DownLeft");
                this.camera.goDownLeft();
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
                    if (isModeZoom())
                    {
                        ZoomDezoom();
                    }
                }
            }
        }

        public bool isModeZoom()
        {
            // Verifie que les 2 mains soient trackées
            if (kinect.handsTracked.Count > 1)
            {
              
                Point hand1 = kinect.handsTracked[0].GetPosition(kinectRegion);
                Point hand2 = kinect.handsTracked[1].GetPosition(kinectRegion);

                //Vérifie si les 2 mains sont à hauteur du centre de l'image (Peut etre trouver un autre élément) marge de 1/5
                if (hand1.Y < (kinectRegion.Height) * 0.8 && hand1.Y > (kinectRegion.Height) * 0.2 && hand2.Y < (kinectRegion.Height) * 0.8 && hand2.Y > (kinectRegion.Height) * 0.2)
                { //On desactive les boutons si necessaire
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
                    kinectRegion.Visibility = System.Windows.Visibility.Hidden;
                    im_modeZoom.Visibility = System.Windows.Visibility.Visible;
                    text_modeZoom.Visibility = System.Windows.Visibility.Visible;
                    return true;

                }
            }
            kinectRegion.Visibility = System.Windows.Visibility.Visible;
            im_modeZoom.Visibility = System.Windows.Visibility.Hidden;
            text_modeZoom.Visibility = System.Windows.Visibility.Hidden;
            xZoom = 0;
            return false;
        }

        //Methode pour un zoom/dezoom 'fluide'
        public void ZoomDezoom()
        {
            Point handR = new Point();
            Point handL = new Point();
            
            if (kinect.handsTracked[0].HandType == HandType.Right)
            {
                handR = kinect.handsTracked[0].GetPosition(kinectRegion);
                handL = kinect.handsTracked[1].GetPosition(kinectRegion);
            }
            else
            {
                handL = kinect.handsTracked[0].GetPosition(kinectRegion);
                handR = kinect.handsTracked[1].GetPosition(kinectRegion);
            }

            Double diff = System.Math.Abs((handL.X + handR.X - kinectRegion.Width));
            // Coordonnée en X 'centré'
            if (diff < kinectRegion.Width*0.4)
            {
                Double xEcart = handR.X - handL.X;


                if (xZoom == 0)
                {
                    xZoom = xEcart;
                    return;
                }
                else if (System.Math.Abs(xEcart - xZoom) > kinectRegion.Width * 0.5)
                {

                    this.timer.Tick += new EventHandler(this.TimerStop);
                    if (!this.timer.Enabled)
                    {
                        this.timer.Interval = 100;
                        this.timer.Start();
                        camera.zoomOnOff(4*(xEcart - xZoom));

                        System.Console.WriteLine("je fais un zoom de " + 4*(xEcart - xZoom));
                        xZoom = xEcart;
                    }
                    return;
                }
            }
            return;
        }

        public void quit_onClick(object sender, RoutedEventArgs e)
        {
            this.kinect.gestureCamera.OnSwipeLeftEvent -= new GestureCamera.SwipeLeftEvent(swipeLeftAction);
            this.kinect.gestureCamera.OnSwipeRightEvent -= new GestureCamera.SwipeRightEvent(swipeRightAction);
            this.kinect.gestureCamera.OnSwipeUpEvent -= new GestureCamera.SwipeUpEvent(retourMenu);
            this.kinect.gestureCamera.stopEventGesture();
            this.Content = null;
            reader.MjpegReaderStop();

            Views.Menu Menu = new Views.Menu(kinect.sensorChooser, true);
            this.Content = Menu;
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
