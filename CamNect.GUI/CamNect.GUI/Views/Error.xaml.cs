using CamNect.Kinect;
using Microsoft.Kinect.Toolkit;
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
using System.Windows.Shapes;

namespace CamNect.GUI.Views
{
    /// <summary>
    /// Logique d'interaction pour Error.xaml
    /// </summary>
    public partial class Error : UserControl
    {
        /* Variables */
        private KinectMain kinect;
        public KinectSensorChooser sensorChooser;

        public Error(KinectSensorChooser sensorChooser)
        {
            InitializeComponent();

            // Sensor initialisation
            this.sensorChooser = sensorChooser;
            this.kinect = new KinectMain(sensorChooser, sensorChooserUi, kinectRegion,false);
 

        }



        private void Quit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {

        }

    }
}
