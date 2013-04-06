using System;
using System.Windows.Controls;
using CamNect.Kinect;
using CamNect.Camera;
using System.Collections.Generic;
using Microsoft.Kinect.Toolkit;
using System.Windows.Data;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect;
using System.Windows;
using ManagedUPnP;

namespace CamNect.GUI.Views
{
    /// <summary>
    /// Logique d'interaction pour CameraOne.xaml
    /// </summary>
    public partial class CameraOne : UserControl
    {

        /* Variables */
        public List<Button> buttons;
        private static CameraPTZ cameraOne;
        private readonly KinectSensorChooser sensorChooser;
        public System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public CameraOne(KinectSensorChooser sensorChooser)
        {
            InitializeComponent();

            // initialize the sensor chooser and UI
            this.sensorChooser = sensorChooser;
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();

            // Bind the sensor chooser's current sensor to the KinectRegion
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);

            // Use KinectMain class
            this.buttons = new List<System.Windows.Controls.Button> { quitButton, buttonDown, buttonDownLeft, buttonDownRight, buttonLeft, buttonRight, buttonTop, buttonTopRight };
            //this.kinect = new KinectMain(this.sensorChooser.Kinect, buttons);

            /*
            kinect = new KinectMain(CameraOneGrid, kinectButton, buttons);
            kinectButton.Click += new RoutedEventHandler(this.kinect.curseur.kinectButton_Click);

            cameraOne = new CameraPTZ(new VlcControl(), CameraOnePlayer);

            Discovery disc = new Discovery(null, AddressFamilyFlags.IPv4, false);
            disc.DeviceAdded += new DeviceAddedEventHandler(discDeviceAdded);
            disc.Start();

            kinect.gestureCamera.OnSwipeLeftEvent += new GestureCamera.SwipeLeftEvent(moveToMenu);
            kinect.gestureCamera.OnSwipeRightEvent += new GestureCamera.SwipeRightEvent(moveToMenu);   */
        }

        /// <summary>
        /// Called when the KinectSensorChooser gets a new sensor
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="args">event arguments</param>
        private static void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
        }


        ////When the window is loaded
        private void CameraOne_Window_Loaded(Object sender, RoutedEventArgs e)
        {
            
        }

        public static void discDeviceAdded(object sender, DeviceAddedEventArgs a)
        {
            cameraOne.initCamera("172.18.255.100");
            System.Console.WriteLine(a.Device.RootHostAddresses[0].ToString());
            cameraOne.Play();
            if (a.Device.FriendlyName.Contains("AXIS 214"))
            {
                cameraOne.initCamera(a.Device.RootHostAddresses[0].ToString());
                cameraOne.Play();
                /*if (cameraArray[0] is CameraPTZ)
                ((CameraPTZ)cameraArray[0]).zoomOn();*/
            }
        }

        public void TimerStop(Object myObject, EventArgs myEventArgs)
        {
            this.timer.Stop();
            message.Content = null;
        }

        public void button_Right(object sender, RoutedEventArgs e)
        {
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 2000;
                this.timer.Start();
                System.Console.WriteLine("Button Right");
                message.Content = "Button Right";
                cameraOne.goRight();
            }
        }

        public void button_Left(object sender, RoutedEventArgs e)
        {
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 2000;
                this.timer.Start();
                System.Console.WriteLine("Button Left");
                message.Content = "Button Left";
                cameraOne.goLeft();
            }
        }

        public void button_Top(object sender, RoutedEventArgs e)
        {
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 2000;
                this.timer.Start();
                System.Console.WriteLine("Button Top");
                message.Content = "Button Top";
                //cameraOne.goUp();
            }
        }

        public void button_TopLeft(object sender, RoutedEventArgs e)
        {
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 3000;
                this.timer.Start();
                System.Console.WriteLine("Button TopLeft");
                message.Content = "Button TopLeft";
                cameraOne.goUp();
                cameraOne.goLeft();
            }
        }

        public void button_TopRight(object sender, RoutedEventArgs e)
        {
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 3000;
                this.timer.Start();
                System.Console.WriteLine("Button TopRight");
                message.Content = "Button TopRight";
                cameraOne.goUp();
                cameraOne.goRight();
            }
            
        }

        public void button_Down(object sender, RoutedEventArgs e)
        {
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 2000;
                this.timer.Start();
                System.Console.WriteLine("Button Down");
                message.Content = "Button Down";
                cameraOne.goDown();
            }
        }

        public void button_DownRight(object sender, RoutedEventArgs e)
        {
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 3000;
                this.timer.Start();
                System.Console.WriteLine("Button DownRight");
                message.Content = "Button DownRight";
                cameraOne.goDown();
                cameraOne.goRight();
            }
            
        }

        public void button_DownLeft(object sender, RoutedEventArgs e)
        {
            this.timer.Tick += new EventHandler(this.TimerStop);
            if (!this.timer.Enabled)
            {
                this.timer.Interval = 3000;
                this.timer.Start();
                System.Console.WriteLine("Button DownLeft");
                message.Content = "Button  DownLeft";
                cameraOne.goDown();
                cameraOne.goLeft();
            }
        }

        public void quitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void button_Menu(object sender, RoutedEventArgs e)
        {
            /*Views.Menu MenuPage = new Views.Menu();
            this.Content = MenuPage;*/
        }

        private void button_Screenshot(object sender, RoutedEventArgs e)
        {
            cameraOne.takeScreenshot();
        }

        private void button_Start(object sender, RoutedEventArgs e)
        {
            cameraOne.startCapture();
        }

        private void button_Stop(object sender, RoutedEventArgs e)
        {
            cameraOne.stopCapture();
        }
    }
}
