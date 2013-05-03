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


    public partial class Menu : UserControl
    {

        /* Variables */
        private KinectMain kinect;
        public KinectSensorChooser sensorChooser;
        private static CameraUtils[] cameraArray;
        private KinectTileButton[] kinectButtonArray;
        private Image[] imageArray;
        int nbCamera;
        private ConfigCamWindow configCamWin;
        private List<CameraUtils> cameraList;

        public Menu(KinectSensorChooser sensorChooser)
        {
            InitializeComponent();

            // Sensor initialisation
            this.sensorChooser = sensorChooser;
            kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion);

            //Configuration panel initialisation
            configCamWin = new ConfigCamWindow();
            configCamWin = Views.Start.configCamWin;
            configCamWin.Closed += onCloseConfig;

            CameraDiscovery.Instance().CameraListModified += Menu_CameraListModified;

            // Initialise the number of images depending on camera(s)
           InitCam(CameraDiscovery.Instance().CameraList);
            
        }

        private void Menu_CameraListModified(List<CameraUtils> cameraList)
        {
            InitCam(cameraList);
        }


        public Menu(KinectSensorChooser sensorChooser, ConfigCamWindow configCamWinArg)
        {
            InitializeComponent();

            // Sensor initialisation
            this.sensorChooser = sensorChooser;
            kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion);

            // Configuration panel initialisation
            configCamWin = configCamWinArg;
            configCamWin.Closed += onCloseConfig;

            //configCamWin = configCamWinArg;
            //configCamWin.Closed += onCloseConfig;
            CameraDiscovery.Instance().CameraListModified += Menu_CameraListModified;

            // Initialise the number of images depending on camera(s)
            InitCam(CameraDiscovery.Instance().CameraList);
        }


        public void InitCam(List<CameraUtils> cameraList)
        {
            this.cameraList = cameraList;
            // Clear out content ScrollViewer
            this.wrapPanel.Children.Clear();

            // ReInit variables (attention à l'ordre)
            nbCamera = cameraList.Count;
            kinectButtonArray = new KinectTileButton[nbCamera];
            imageArray = new Image[nbCamera];
            cameraArray = new CamNect.Camera.CameraUtils[nbCamera];

            // Message
            message.Content = cameraList.Count;

            for (int i = 0; i < cameraList.Count; i++)
            {
               if (cameraList[i].Config.Afficher)
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
                    kinectButtonArray[i].Label = cameraList[i].Config.Nom;
                    cameraList[i].startReader(imageArray[i]);
                }
                else
                {
                    nbCamera--;
                }
            }
        }

        private void InitMenu()
        {

        }

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
            if (cameraList[j].Config.isPtz)
            {
                this.Content = null;
                Views.CameraOne CameraOnePage = new Views.CameraOne(this.sensorChooser, cameraList, j);
                this.Content = CameraOnePage;
            }
            else
            {
                this.Content = null;
                Views.CameraNotPTZ cameraNotPTZPage = new Views.CameraNotPTZ(this.sensorChooser, cameraList, j);
                this.Content = cameraNotPTZPage;
            }
       }

        private void quitOnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void onCloseConfig(object sender, EventArgs e)
        {
            //this.Content = null;
            //cameraArray = null;
            //kinectButtonArray = null;
            //imageArray = null;

            //cleanStreamViews();

            //Views.Menu Menu = new Views.Menu(this.sensorChooser);
            //this.Content = Menu;

            bool listEnd = false;

            while (!listEnd)
            {
                listEnd = true;

                foreach (CameraUtils cam in cameraList)
                {
                    if (cam.Config.Plugged == false)
                    {
                        cameraList.Remove(cam);
                        listEnd = false;
                        break;
                    }
                }
            }

            CameraDiscovery.Instance().Start();
        }


        private void reloadOnClick(object sender, RoutedEventArgs e)
        {
            CameraDiscovery.Instance().Start();
            //this.Content = null;
            //cameraArray = null;
            //kinectButtonArray = null;
            //imageArray = null;
            Console.WriteLine(".............. Start search");
            while (CameraDiscovery.Instance().isSearching)
            {
                
            }
            Console.WriteLine("............... Search complete");

            bool listEnd = false;
            while (!listEnd) {
                listEnd = true;

                foreach (CameraUtils cam in cameraList)
                {
                    if (cam.Config.Plugged == false)
                    {
                        cameraList.Remove(cam);
                        listEnd = false;
                        break;
                    }
                }
            }
        }

        private void configOnClick(object sender, RoutedEventArgs e)
        {
            configCamWin.Show();
        }

    }
}
