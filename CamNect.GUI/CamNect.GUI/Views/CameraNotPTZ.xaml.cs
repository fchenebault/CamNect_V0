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
        private int i;
        private KinectMain kinect;
        public KinectSensorChooser sensorChooser;
        private List<CameraUtils> cameraListTMP;
        private int indice;
        private CameraUtils camera;
        private MjpegReader reader;
        private bool isButtonActive;
        private List<Image> listImages;

        public CameraNotPTZ(KinectSensorChooser sensorChooser, List<CameraUtils> cameraListTMP, int indice)
        {
            InitializeComponent();

            // Sensor initialisation
            this.kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion, true);
            //this.kinect.InitKinect(sensorChooserUi, kinectRegion);

            // Camera Initialisation
            this.camera = cameraListTMP[indice];
            this.cameraListTMP = cameraListTMP;
            this.indice = indice;
            reader = new MjpegReader(camera, CameraNotPTZPlayer, camera.Config.HighRes);


            kinect.gestureCamera.OnSwipeLeftEvent += new GestureCamera.SwipeLeftEvent(swipeLeftAction);
            kinect.gestureCamera.OnSwipeRightEvent += new GestureCamera.SwipeRightEvent(swipeRightAction);
            kinect.gestureCamera.OnSwipeUpEvent += new GestureCamera.SwipeUpEvent(retourMenu);

            // Light initialisation
            i = 0;
            camera.light(0);
            lightProgressBar.Orientation = Orientation.Vertical;
            lumiereEllipse.StrokeThickness = 100;
            lightPlus.Visibility = System.Windows.Visibility.Hidden;
            lightMoins.Visibility = System.Windows.Visibility.Hidden;
            lightmin.Opacity = 0.4;
            lightmax.Opacity = 0.4;

            // TileButtons init
            listImages = new List<Image> { dog, psst, click, intruder };
            foreach (Image image in this.listImages)
            {
                image.Opacity = 0.4;
            }
            
            // Events for grip buttons
            isButtonActive = false;
            backgroundGrip.isCameraOne = true;
            backgroundGrip.OnHandGrip += new KinectScrollViewer.HandGripEvent(activeButtons);
                      
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

        public void retourMenu()
        {
            this.kinect.gestureCamera.OnSwipeLeftEvent -= new GestureCamera.SwipeLeftEvent(swipeLeftAction);
            this.kinect.gestureCamera.OnSwipeRightEvent -= new GestureCamera.SwipeRightEvent(swipeRightAction);
            this.kinect.gestureCamera.OnSwipeUpEvent -= new GestureCamera.SwipeUpEvent(retourMenu);
            this.kinect.gestureCamera.stopEventGesture();
            backgroundGrip.OnHandGrip -= new KinectScrollViewer.HandGripEvent(activeButtons);
            camera.light(0);
            this.Content = null;
            reader.MjpegReaderStop();

            Views.Menu Menu = new Views.Menu(kinect.sensorChooser, true);
            this.Content = Menu;
        }

        public void swipeLeftAction()
        {
            this.kinect.gestureCamera.OnSwipeLeftEvent -= new GestureCamera.SwipeLeftEvent(swipeLeftAction);
            this.kinect.gestureCamera.OnSwipeRightEvent -= new GestureCamera.SwipeRightEvent(swipeRightAction);
            this.kinect.gestureCamera.OnSwipeUpEvent -= new GestureCamera.SwipeUpEvent(retourMenu);
            this.kinect.gestureCamera.stopEventGesture();
            backgroundGrip.OnHandGrip -= new KinectScrollViewer.HandGripEvent(activeButtons);

            int nb = cameraListTMP.Count;
            do {
            indice--;
            if (indice < 0)
                indice = nb - 1;
            } while (!cameraListTMP[indice].Config.Afficher);

            this.Content = null;
            reader.MjpegReaderStop();

            // Select if the camera is PTZ
            if (cameraListTMP[indice].Config.isPtz)
            {
                Views.CameraOne CameraOnePage = new Views.CameraOne(kinect.sensorChooser, cameraListTMP, indice);
                this.Content = CameraOnePage;
            }
            else
            {
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
            backgroundGrip.OnHandGrip -= new KinectScrollViewer.HandGripEvent(activeButtons);
            do {
            indice++;
            if (indice > nb - 1)
                indice = 0;
            } while (!cameraListTMP[indice].Config.Afficher);

            this.Content = null;
            reader.MjpegReaderStop();

            // Select if the camera is PTZ
            if (cameraListTMP[indice].Config.isPtz)
            {
                Views.CameraOne CameraOnePage = new Views.CameraOne(kinect.sensorChooser, cameraListTMP, indice);
                this.Content = CameraOnePage;
            }
            else
            {
                Views.CameraNotPTZ cameraNotPTZPage = new Views.CameraNotPTZ(kinect.sensorChooser, cameraListTMP, indice);
                this.Content = cameraNotPTZPage;
            }
        }

        public void quit_onClick(object sender, RoutedEventArgs e)
        {
            this.kinect.gestureCamera.OnSwipeLeftEvent -= new GestureCamera.SwipeLeftEvent(swipeLeftAction);
            this.kinect.gestureCamera.OnSwipeRightEvent -= new GestureCamera.SwipeRightEvent(swipeRightAction);
            this.kinect.gestureCamera.OnSwipeUpEvent -= new GestureCamera.SwipeUpEvent(retourMenu);
            this.kinect.gestureCamera.stopEventGesture();
            backgroundGrip.OnHandGrip -= new KinectScrollViewer.HandGripEvent(activeButtons);
            camera.light(0);
            this.Content = null;
            reader.MjpegReaderStop();

            Views.Menu Menu = new Views.Menu(kinect.sensorChooser, true);
            this.Content = Menu;
        }


        private void LightIncreaseButtonClick(object sender, RoutedEventArgs e)
        {
            if (i < 96)
            {
                i += 5;
                //lumiereEllipse.StrokeThickness = 100-i;
                lightProgressBar.Value = i;
            }
            camera.light(i);
            
        }

        private void LightDecreaseButtonClick(object sender, RoutedEventArgs e)
        {
            if (i > 4)
            {
                i -= 5;
                //lumiereEllipse.StrokeThickness = 100-i;
                lightProgressBar.Value = i;
            }
            camera.light(i);
            
        }

        private void sound1_onClick(object sender, RoutedEventArgs e)
        {
            camera.playMediaClip(6);
        }
        private void sound2_onClick(object sender, RoutedEventArgs e)
        {
            camera.playMediaClip(7);
        }
        private void sound3_onClick(object sender, RoutedEventArgs e)
        {
            camera.playMediaClip(8);
        }
        private void sound4_onClick(object sender, RoutedEventArgs e)
        {
            camera.playMediaClip(9);
        }

        public void activeButtons()
        {
            if (isButtonActive)
            {
                lightmin.Opacity = 0.4;
                lightmax.Opacity = 0.4;
                lightPlus.Visibility = System.Windows.Visibility.Hidden;
                lightMoins.Visibility = System.Windows.Visibility.Hidden;

                foreach (Image image in this.listImages)
                {
                    image.Opacity = 0.4;
                }

                isButtonActive = false;
            }
            else
            {
                lightmin.Opacity = 0.8;
                lightmax.Opacity = 0.8;
                lightPlus.Visibility = System.Windows.Visibility.Visible;
                lightMoins.Visibility = System.Windows.Visibility.Visible;

                foreach (Image image in this.listImages)
                {
                    image.Opacity = 0.8;
                }

                isButtonActive = true;
            }
        }

    }
}
