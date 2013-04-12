using CamNect.Kinect;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CamNect.GUI.Views
{
    /// <summary>
    /// Logique d'interaction pour Start.xaml
    /// </summary>
    public partial class Start : Window
    {
        /* Variables */
        private KinectMain kinect;


        public Start()
        {
            // Sensor initialisation
            this.kinect = new KinectMain(sensorChooserUi, kinectRegion);
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            // Timer to wait for the other view
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(3000);
            dispatcherTimer.Start();
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //Views.Menu MenuPage = new Menu(kinect.sensorChooser);
            //this.Content = MenuPage;
        }

    }
}
