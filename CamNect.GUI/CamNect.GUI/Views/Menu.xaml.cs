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
using MjpegProcessor;

namespace CamNect.GUI.Views
{
    /// <summary>
    /// Logique d'interaction pour Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {

        /* Variables */
        private KinectMain kinect;
        public KinectSensorChooser sensorChooser;
        private static CamNect.Camera.CameraUtils[] cameraArray = new CamNect.Camera.CameraUtils[3];
        MjpegDecoder _mjpeg;

        public Menu(KinectSensorChooser sensorChooser)
        {
            InitializeComponent();

            // Sensor initialisation
            this.sensorChooser = sensorChooser;
            this.kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion);
            _mjpeg = new MjpegDecoder();
            _mjpeg.FrameReady += mjpeg_FrameReady;
            _mjpeg.ParseStream(new Uri("http://172.18.255.101/mjpg/video.mjpg"),"root","root");
            cameraOne.Label = CameraOne.cameraList[0].Config.Nom;

                //cameraArray[0] = new CameraPTZ(new Vlc.DotNet.Wpf.VlcControl(), cameraOne);
            // Use KinectMain class
            //this.kinect = new KinectMain(this.sensorChooser.Kinect);
            

           

            /*Instruction.Text = "Select a CCTV";
            this.buttons = new List<System.Windows.Controls.Button> { buttonOne, buttonTwo, buttonThree };
            kinect = new KinectMain(MenuGrid, kinectButton, buttons);
            kinectButton.Click += new RoutedEventHandler(this.kinect.curseur.kinectButton_Click);

            /*cameraArray[0] = new CameraPTZ(new VlcControl(), cameraOne);
            cameraArray[1] = new CameraSTD(new VlcControl(), cameraTwo);
            cameraArray[2] = new CameraSTD(new VlcControl(), cameraThree);

            Discovery disc = new Discovery(null, AddressFamilyFlags.IPv4, false);
            disc.DeviceAdded += new DeviceAddedEventHandler(discDeviceAdded);
            disc.Start();/*

            kinect.gestureCamera.OnSwipeLeftEvent += new GestureCamera.SwipeLeftEvent(moveToCameraOne);
            kinect.gestureCamera.OnSwipeRightEvent += new GestureCamera.SwipeRightEvent(moveToCameraOne);*/

        }

    //    private void Start_Click(object sender, RoutedEventArgs e)
   //     {
    //        _mjpeg.ParseStream(new Uri("http://172.18.255.100/mjpg/video.mjpg"));
    //    }

        private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
        {
            test.Source = e.BitmapImage;
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

        private void moveToCameraOne()
        {
            Views.CameraOne CameraPage = new Views.CameraOne(this.sensorChooser);
            this.Content = CameraPage;
        }

        ////When the window is loaded
        private void Menu_Window_Loaded(Object sender, RoutedEventArgs e)
        {
           
        }


        public void button_Two(object sender, RoutedEventArgs e)
        {
            Views.CameraOne CameraOnePage = new Views.CameraOne(this.sensorChooser);
            this.Content = CameraOnePage;
        }


        private void KinectTileButtonClick(object sender, RoutedEventArgs e)
        {
            message.Content = "click" ;
        }

    }
}
