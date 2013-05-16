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
        private static CameraUtils[] cameraArray;
        private KinectTileButton[] kinectButtonArray;
        private Image[] imageArray;
        private MjpegReader[] readerArray;
        int nbCamera;
        private static ConfigCamWindow configCamWin;

        public Menu(KinectSensorChooser sensorChooser)
        {
            InitializeComponent();

            // Sensor initialisation
            this.sensorChooser = sensorChooser;
            kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion);

            // Configuration panel initialisation
            configCamWin = Start.configCamWin;
            configCamWin.Deactivated += onCloseConfig;

            // Variables initialisation
            nbCamera = CameraOne.cameraList.Count;
            cameraArray = new CamNect.Camera.CameraUtils[nbCamera];
            kinectButtonArray = new KinectTileButton[nbCamera];
            imageArray = new Image[nbCamera];
            readerArray = new MjpegReader[nbCamera];

            // Initialise the number of images depending on camera(s)
            InitCam();
            message.Content = CameraOne.cameraList.Count;
        }


        public void InitCam()
        {
            wrapPanel.Children.Clear();
         //   configCamWin = Start.configCamWin;
            for (int i = 0; i < CameraOne.cameraList.Count; i++)
            {
                if (CameraOne.cameraList[i].Config.Afficher && CameraOne.cameraList[i].Config.Plugged)
                {
                    kinectButtonArray[i] = new KinectTileButton();
                    kinectButtonArray[i].Width = 800;
                    kinectButtonArray[i].Height = 600;
                    imageArray[i] = new Image();
                    imageArray[i].Height = 600;
                    imageArray[i].Width = 800;
                    kinectButtonArray[i].Content = imageArray[i];
                    kinectButtonArray[i].Click += KinectTileButtonClick;
                    wrapPanel.Children.Add(kinectButtonArray[i]);
                    kinectButtonArray[i].Label = CameraOne.cameraList[i].Config.Nom;
                    this.readerArray[i] = new MjpegReader(CameraOne.cameraList[i], imageArray[i], CameraOne.cameraList[i].Config.MediumRes);
                }
                else
                {
                    nbCamera--;
                }
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


        private void KinectTileButtonClick(object sender, RoutedEventArgs e)
        {
            KinectTileButton boutonClick = (KinectTileButton)sender;
            int i, j = 0;
            // Find the selected camera
            for (i = 0; i < kinectButtonArray.Length; i++)
            {
                if (boutonClick == kinectButtonArray[i])
                {
                    j = i;
                }
            }

            // Select if the camera is PTZ
            if (CameraOne.cameraList[j].Config.isPtz)
            {
                this.Content = null;
                cleanStreamViews();

                Views.CameraOne CameraOnePage = new Views.CameraOne(this.sensorChooser, CameraOne.cameraList, j);
                this.Content = CameraOnePage;
            }
            else
            {
                this.Content = null;
                cleanStreamViews();

                Views.CameraNotPTZ cameraNotPTZPage = new Views.CameraNotPTZ(this.sensorChooser, CameraOne.cameraList, j);
                this.Content = cameraNotPTZPage;
            }
       }

        private void quitOnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void onCloseConfig(object sender, EventArgs e)
        {          
        /*    this.Content = null;
            cameraArray = null;
            kinectButtonArray = null;
            imageArray = null;

            cleanStreamViews();
            Views.Menu Menu = new Views.Menu(this.sensorChooser);
            this.Content = Menu;*/
          //  cleanStreamViews();
          //  InitCam();

            this.Content = null;
            cameraArray = null;
            kinectButtonArray = null;
            imageArray = null;

            cleanStreamViews();
            readerArray = null;

            Views.Menu Menu = new Views.Menu(this.sensorChooser);
            this.Content = Menu;
        }


        private void reloadOnClick(object sender, RoutedEventArgs e)
        {
            this.Content = null;
            cameraArray = null;
            kinectButtonArray = null;
            imageArray = null;

            cleanStreamViews();
            readerArray = null;

            Views.Menu Menu = new Views.Menu(this.sensorChooser);
            this.Content = Menu;
        }

        private void configOnClick(object sender, RoutedEventArgs e)
        {
            configCamWin.Show();
        }

        private void cleanStreamViews()
        {
            for (int i = 0; i < readerArray.Length; i++)
            {
             if(this.readerArray[i]!=null)
                    this.readerArray[i].MjpegReaderStop();
                
            }
        }

    }
}
