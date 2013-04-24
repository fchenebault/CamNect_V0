using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CamNect.Kinect;
using CamNect.Camera;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect;

namespace CamNect.GUI.Views
{
    /// <summary>
    /// Logique d'interaction pour cameraNotPTZ.xaml
    /// </summary>
    public partial class CameraNotPTZ : UserControl
    {

        // Variables
        private KinectMain kinect;
        public KinectSensorChooser sensorChooser;
        private CameraUtils camera;
        private MjpegReader reader;

        public CameraNotPTZ(KinectSensorChooser sensorChooser, CameraUtils camera)
        {
            InitializeComponent();

            // Sensor initialisation
            this.kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion);
            this.sensorChooser = sensorChooser;

            // Camera Initialisation
            reader = new MjpegReader(camera, CameraNotPTZPlayer);
            this.camera = camera;
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
    }
}
