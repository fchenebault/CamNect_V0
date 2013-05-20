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
        private static ConfigCamWindow configCamWin;
        private static List<CameraUtils> cameraList;
        private static List<MjpegReader> readerList;
        private static List<KinectTileButton> kinectButtonList;
        private static List<Image> imageList;


        public Menu(KinectSensorChooser sensorChooser, Boolean firstCreation)
        {
            InitializeComponent();

            // Sensor initialisation
            this.kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion,false);
         //   this.kinect.InitKinect(sensorChooserUi, kinectRegion);

            // Configuration panel initialisation
            configCamWin = Start.configCamWin;
            configCamWin.Deactivated += onCloseConfig;

            if (firstCreation)
            {
                // Variables initialisation
                cameraList = new List<CameraUtils>();
                readerList = new List<MjpegReader>();
                imageList = new List<Image>();
                kinectButtonList = new List<KinectTileButton>();


                // Initialise the number of images depending on camera(s)
                InitCam();
            }
            else
            {
                refreshList();
                foreach (KinectTileButton button in kinectButtonList)
                {
                    wrapPanel.Children.Add(button);
                    button.Click += KinectTileButtonClick;
                }
            }
        }


        public void InitCam()
        {
            wrapPanel.Children.Clear();

            foreach (CameraUtils camera in CameraOne.cameraList)
            {
                // init param
                KinectTileButton kinectTileButton = new KinectTileButton();
                kinectTileButton.Width = 800;
                kinectTileButton.Height = 600;
                kinectTileButton.Tag = camera.Config.Serie;
                Image image = new Image();
                image.Width = 800;
                image.Height = 600;
                kinectTileButton.Content = image;
                kinectTileButton.Click += KinectTileButtonClick;
                wrapPanel.Children.Add(kinectTileButton);
                MjpegReader reader = new MjpegReader(camera, image, camera.Config.MediumRes);

                if (!camera.Config.Afficher)
                {
                    kinectTileButton.Visibility = System.Windows.Visibility.Collapsed;
                }
                
                imageList.Add(image);
                kinectButtonList.Add(kinectTileButton);
                cameraList.Add(camera);
                readerList.Add(reader);
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
            int i = 0, j = 0;

            // Find the selected camera
            foreach (CameraUtils camera in cameraList)
            {
                String tag = (String)boutonClick.Tag;
                String serie = camera.Config.Serie;

                if (tag.Equals(serie))
                {
                    j = i;
                }

                i++;
            }

            foreach (MjpegReader reader in readerList)
            {
                reader.MjpegReaderStop();
            }

            // Select if the camera is PTZ
            if (CameraOne.cameraList[j].Config.isPtz)
            {
                this.Content = null;

                wrapPanel.Children.Clear();
                Views.CameraOne CameraOnePage = new Views.CameraOne(kinect.sensorChooser, CameraOne.cameraList, j);
                this.Content = CameraOnePage;
            }
            else
            {
                this.Content = null;

                wrapPanel.Children.Clear();
                Views.CameraNotPTZ cameraNotPTZPage = new Views.CameraNotPTZ(kinect.sensorChooser, CameraOne.cameraList, j);
                this.Content = cameraNotPTZPage;
            }
        }

        private void quitOnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void onCloseConfig(object sender, EventArgs e)
        {
            refreshList();
        }


        private void reloadOnClick(object sender, RoutedEventArgs e)
        {
            refreshList();
            
        }

        private void refreshList()
        {
            foreach (CameraUtils camera in CameraOne.cameraList)
            {
                Boolean cameraIsNew = true;

                foreach (CameraUtils cameraOld in cameraList)
                {
                    if (camera.Config.Serie == cameraOld.Config.Serie)
                    {
                        cameraIsNew = false;

                        if (camera.Config.Afficher)
                        {
                            foreach (KinectTileButton button in kinectButtonList)
                            {
                                String tag = (String)button.Tag;
                                String serie = (String)camera.Config.Serie;

                                if (tag.Equals(serie))
                                {
                                    button.Visibility = System.Windows.Visibility.Visible;
                                }
                            }
                        }
                        else
                        {
                            foreach (KinectTileButton button in kinectButtonList)
                            {
                                String tag = (String)button.Tag;
                                String serie = (String)camera.Config.Serie;

                                if (tag.Equals(serie))
                                {
                                    button.Visibility = System.Windows.Visibility.Collapsed;
                                }
                            }
                        }
                    }
                }

                if (cameraIsNew)
                {
                    // init param
                    KinectTileButton kinectTileButton = new KinectTileButton();
                    kinectTileButton.Width = 800;
                    kinectTileButton.Height = 600;
                    kinectTileButton.Tag = camera.Config.Serie;
                    Image image = new Image();
                    image.Width = 800;
                    image.Height = 600;
                    kinectTileButton.Content = image;
                    kinectTileButton.Click += KinectTileButtonClick;
                    wrapPanel.Children.Add(kinectTileButton);
                    MjpegReader reader = new MjpegReader(camera, image, camera.Config.MediumRes);

                    if (!camera.Config.Afficher)
                    {
                        kinectTileButton.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    imageList.Add(image);
                    kinectButtonList.Add(kinectTileButton);
                    cameraList.Add(camera);
                    readerList.Add(reader);
                }
            }
        }

        private void configOnClick(object sender, RoutedEventArgs e)
        {
            configCamWin.Show();
        }

        

    }
}
