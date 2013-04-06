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
using System.Windows.Forms;
//using Microsoft.Samples.Kinect.SwipeGestureRecognizer;
using Microsoft.Kinect;
using Fizbin.Kinect.Gestures.Segments;
using Microsoft.Kinect.Toolkit;
using Fizbin.Kinect.Gestures;
//using Microsoft.Samples.Kinect.WpfViewers;

namespace Kinect_Architecture
{
    public class SkeletonManagement
    {
        public Skeleton skeleton { get; set; }
        public Stickman stickman { get; set; }
        public float distance {get; set;}

        public SkeletonManagement(SkeletonFrame skeletonFrame, int i)
        {
            // Récupère le skeleton "i" parmi les skeletons détéctés
            Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
            skeletonFrame.CopySkeletonDataTo(skeletonData);
            this.skeleton = skeletonData[i];

            this.distance = -1;
        }

       
        // Calcule la distance du skeleton et compare avec la plus proche connue
        public bool isSkeletonNearest(float nearestDistance)
        {
            // Find the distance squared.
            this.distance = (this.skeleton.Position.X * this.skeleton.Position.X) +
                (this.skeleton.Position.Y * this.skeleton.Position.Y) +
                (this.skeleton.Position.Z * this.skeleton.Position.Z);

            if (this.distance < nearestDistance)
                return true;
            else
                return false;
                
        }

        // Place une icone MAIN à l'emplacement de la main la plus haute
        public ColorImagePoint HandFocus(KinectSensor sensor, Image curseur)
        {
            CoordinateMapper cmLeft = new CoordinateMapper(sensor);
            CoordinateMapper cmRight = new CoordinateMapper(sensor);
            ColorImagePoint Left = cmLeft.MapSkeletonPointToColorPoint(this.skeleton.Joints[JointType.HandLeft].Position, ColorImageFormat.RgbResolution1280x960Fps12);
            ColorImagePoint Right = cmRight.MapSkeletonPointToColorPoint(this.skeleton.Joints[JointType.HandRight].Position, ColorImageFormat.RgbResolution1280x960Fps12);

            if (Right.Y < Left.Y)
            {
                curseur.Source = new BitmapImage(new Uri("Ressources/Images/hand_r.png", UriKind.Relative));
                return Right;
            }
            else
            {
                curseur.Source = new BitmapImage(new Uri("Ressources/Images/hand_l.png", UriKind.Relative));
                return Left;
            }
        }

     }
}
