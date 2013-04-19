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
using System.Windows.Shapes;

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
        private MjpegReader[] readerArray = new MjpegReader[CameraOne.cameraList.Count];

        public Menu(KinectSensorChooser sensorChooser)
        {
            InitializeComponent();
            
            // Sensor initialisation
            this.sensorChooser = sensorChooser;
            this.kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion);

            InitCam();
        }


        public void InitCam()
        {
            // Pour chaque caméra de la liste, on crée la zone d'affichage et lance le player.          
            KinectTileButton[] kinectButtonArray = new KinectTileButton[CameraOne.cameraList.Count];
            System.Console.WriteLine("nb de cam: "+CameraOne.cameraList.Count.ToString());

            for (int i = 0;i < CameraOne.cameraList.Count; i++) 
            {                
                kinectButtonArray[i] = new KinectTileButton();
                kinectButtonArray[i].Width = 800;
                kinectButtonArray[i].Height = 530;          
                Image image = new Image();
                image.Height = 600;
                image.Width = 800;
                kinectButtonArray[i].Content = image;
                kinectButtonArray[i].Label = CameraOne.cameraList[i].Config.Nom;
                kinectButtonArray[i].Click += KinectTileButtonClick;
                
                wrapPanel.Children.Add(kinectButtonArray[i]);
                this.readerArray[i] = new MjpegReader(CameraOne.cameraList[i], image);
         
            }       

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
            message.Content = ((KinectTileButton)sender).Label; 
            
        }

    }
}
