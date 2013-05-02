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

        /* Variables */
        // Kinect
        private KinectMain kinect;
        public KinectSensorChooser sensorChooser;

        // Camera
        private MjpegReader reader;
        private List<CameraUtils> cameraListTMP;
        private int indice;
        private CameraUtils camera;

        // View
        public List<Polygon> polygons;
        private List<KinectHoverButton> hoverButtons;
        private bool isButtonActive = false;
        public System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public System.Windows.Forms.Timer highlightTimer = new System.Windows.Forms.Timer();
        private Double xZoom = -1;

        public CameraOne(KinectSensorChooser sensorChooser, List<CameraUtils> cameraListTMP, int indice)
        {
            InitializeComponent();

            // XAML Initialisation
            polygons = new List<Polygon> { polygonOverDownLeft, polygonDown, polygonUp, polygonOverDownRight, polygonLeft, polygonRight, polygonOverUpLeft, polygonOverUpRight, polygonFlecheDown, polygonFlecheDownLeft, polygonFlecheDownRight, polygonFlecheLeft, polygonFlecheRight, polygonFlecheUp, polygonFlecheUpLeft, polygonFlecheUpRight };
            hoverButtons = new List<KinectHoverButton> { buttonDown, buttonDownLeft, buttonDownRight, buttonLeft, buttonRight, buttonTop, buttonTopLeft, buttonTopRight };

            foreach (KinectHoverButton hoverButton in this.hoverButtons)
            {
                hoverButton.Visibility = System.Windows.Visibility.Hidden;
            }

            // Sensor initialisation
            this.kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion);
            this.sensorChooser = sensorChooser;

            // Camera Initialisation
            this.camera = cameraListTMP[indice];
            this.cameraListTMP = cameraListTMP;
            this.indice = indice;
            reader = new MjpegReader(camera, CameraOnePlayer, camera.Config.HighRes);

            // Events for gestures
            kinect.gestureCamera.OnSwipeLeftEvent += new GestureCamera.SwipeLeftEvent(swipeLeftAction);
            kinect.gestureCamera.OnSwipeRightEvent += new GestureCamera.SwipeRightEvent(swipeRightAction);
            kinect.gestureCamera.OnSwipeUpEvent += new GestureCamera.SwipeUpEvent(retourMenu);

            // Events for grip buttons
            backgroundGrip.isCameraOne = true;
            backgroundGrip.OnHandGrip += new KinectScrollViewer.HandGripEvent(activeButtons);
        }


        public void retourMenu()
        {
            this.Content = null;
            reader.MjpegReaderStop();

            Views.Menu Menu = new Views.Menu(this.kinect.sensorChooser);
            this.Content = Menu;
        }


        public void swipeLeftAction()
        {
            int nb = cameraListTMP.Count;
            indice--;
            if (indice < 0)
                indice = nb - 1;


            // Select if the camera is PTZ
            if (cameraListTMP[indice].Config.isPtz)
            {
                this.Content = null;
                reader.MjpegReaderStop();
                Views.CameraOne CameraOnePage = new Views.CameraOne(this.sensorChooser, cameraListTMP, indice);
                this.Content = CameraOnePage;
            }
            else
            {
                this.Content = null;
                reader.MjpegReaderStop();
                Views.CameraNotPTZ cameraNotPTZPage = new Views.CameraNotPTZ(this.sensorChooser, cameraListTMP, indice);
                this.Content = cameraNotPTZPage;
            }

        }


        public void swipeRightAction()
        {
            // Variables
            int nb = cameraListTMP.Count;
            indice++;
            if (indice > nb - 1)
            {
                indice = 0;
            }

            // Select if the camera is PTZ
            if (cameraListTMP[indice].Config.isPtz)
            {
                this.Content = null;
                reader.MjpegReaderStop();
                Views.CameraOne CameraOnePage = new Views.CameraOne(this.sensorChooser, cameraListTMP, indice);
                this.Content = CameraOnePage;
            }
            else
            {
                this.Content = null;
                reader.MjpegReaderStop();
                Views.CameraNotPTZ cameraNotPTZPage = new Views.CameraNotPTZ(this.sensorChooser, cameraListTMP, indice);
                this.Content = cameraNotPTZPage;
            }
        }


        public void TimerStop(Object myObject, EventArgs myEventArgs)
        {
            this.timer.Stop();
            message.Content = null;
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
                this.camera.goDownLeft();
            }
        }


        public void gestureEvent(object sender, EventArgs e)
        {
            try
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
            catch (ExecutionEngineException exception)
            {
                System.Console.WriteLine("Erreur on Gesture Event - Class CameraOne" + exception.ToString());
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
                if (hand1.Y < (kinectRegion.Height) * 0.7 && hand1.Y > (kinectRegion.Height) * 0.3 && hand2.Y < (kinectRegion.Height) * 0.7 && hand2.Y > (kinectRegion.Height) * 0.3)
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
                    //            im_handL.Visibility = System.Windows.Visibility.Visible;
                    //           im_handR.Visibility = System.Windows.Visibility.Visible;
                    im_modeZoom.Visibility = System.Windows.Visibility.Visible;
                    text_modeZoom.Visibility = System.Windows.Visibility.Visible;
                    return true;

                }
            }
            kinectRegion.Visibility = System.Windows.Visibility.Visible;
            //     im_handL.Visibility = System.Windows.Visibility.Hidden;
            //      im_handR.Visibility = System.Windows.Visibility.Hidden;
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

            //   Canvas.SetTop(im_handL,handL.Y);
            //   Canvas.SetTop(im_handR, handR.Y);

            Double diff = System.Math.Abs((handL.X + handR.X - kinectRegion.Width));
            // Coordonnée en X 'centré'
            if (diff < kinectRegion.Width * 0.4)
            {
                Double xEcart = handR.X - handL.X;
                //  Canvas.SetLeft(im_handL, CameraOneCanvas.Width / 2 - System.Math.Abs(xEcart / 2));
                //  Canvas.SetLeft(im_handR, System.Math.Abs(xEcart / 2) + CameraOneCanvas.Width / 2);

                if (xZoom == 0)
                {
                    xZoom = xEcart;
                    return;
                }
                else if (System.Math.Abs(xEcart - xZoom) > kinectRegion.Width * 0.5)
                {
                    // System.Console.WriteLine("mode zoom");
                    this.timer.Tick += new EventHandler(this.TimerStop);
                    if (!this.timer.Enabled)
                    {
                        this.timer.Interval = 100;
                        this.timer.Start();
                        camera.zoomOnOff(4 * (xEcart - xZoom));

                        System.Console.WriteLine("je fais un zoom de " + 4 * (xEcart - xZoom));
                        xZoom = xEcart;
                    }
                    return;
                }
            }
            return;
        }

        public void quit_onClick(object sender, RoutedEventArgs e)
        {
            this.Content = null;
            reader.MjpegReaderStop();

            Views.Menu Menu = new Views.Menu(this.kinect.sensorChooser);
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
