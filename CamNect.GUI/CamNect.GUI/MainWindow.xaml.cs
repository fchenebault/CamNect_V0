using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System.Windows;
using System.Windows.Data;
using Microsoft.Kinect;
using System;
using CamNect.Kinect;
using CamNect.Camera;
using System.Collections.ObjectModel;

namespace CamNect.GUI
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        /* Variables */
        private KinectMain kinect;
        public KinectSensorChooser sensorChooser;

        public MainWindow()
        {
            // XAML initialisation
            InitializeComponent();
            Instruction.Text = "Push the start button";

            // Sensor initialisation
            this.sensorChooser = new KinectSensorChooser();
            this.kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion);

            // Add gestures events
            //kinect.gestureCamera.OnSwipeLeftEvent += new GestureCamera.SwipeLeftEvent(writeMessage);
        }

        //Test the size screen
        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            // get the main screen size
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;

            // if the main screen is not 1920 x 1080 then warn the user it is not the optimal experience 
            if (width != 1920 || height != 1080)
            {
                MessageBoxResult continueResult = MessageBox.Show("This screen is not 1920 x 1080.\nThis sample has been optimized for a screen resolution of 1920 x 1080.\nDo you wish to continue?", "Suboptimal Screen Resolution", MessageBoxButton.YesNo);
                if (continueResult == MessageBoxResult.No)
                {
                    this.Close();
                }
            }
        }


        public void button_Start(object sender, RoutedEventArgs e)
        {
            Views.Menu MenuPage = new Views.Menu(kinect.sensorChooser);
            this.Content = MenuPage;

        }

        public void quitButton_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.Application.Current.Shutdown();

        }


    }
}