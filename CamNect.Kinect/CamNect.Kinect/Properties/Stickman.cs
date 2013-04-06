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
    public class Stickman
    {

        public Skeleton skeleton { get; set; }
        public Brush brush { get; set; }
        public int thickness { get; set; }
        public Canvas StickMen { get; set; }


        private static readonly JointType[][] SkeletonSegmentRuns = new JointType[][]
        {
            new JointType[] 
            { 
                JointType.Head, JointType.ShoulderCenter, JointType.HipCenter 
            },
            new JointType[] 
            { 
                JointType.HandLeft, JointType.WristLeft, JointType.ElbowLeft, JointType.ShoulderLeft,
                JointType.ShoulderCenter,
                JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight
            },
            new JointType[]
            {
                JointType.FootLeft, JointType.AnkleLeft, JointType.KneeLeft, JointType.HipLeft,
                JointType.HipCenter,
                JointType.HipRight, JointType.KneeRight, JointType.AnkleRight, JointType.FootRight
            }
        };


        public Stickman(Skeleton skeleton, Canvas StickMen)
        {
            this.skeleton = skeleton;
            this.brush = Brushes.WhiteSmoke;
            this.thickness = 7;
            this.StickMen = StickMen;
        }

        // Coordonnées du centre du Stickman
        public Point GetJointPoint(JointType jointType)
        {
            var joint = this.skeleton.Joints[jointType];

            // Points are centered on the StickMen canvas and scaled according to its height allowing
            // approximately +/- 1.5m from center line.
            var point = new Point
            {
                X = (this.StickMen.Width / 2) + (this.StickMen.Height * joint.Position.X / 3),
                Y = (this.StickMen.Width / 2) - (this.StickMen.Height * joint.Position.Y / 3)
            };

            return point;
        }

        // DESSIN DU STICKMEN
        public void DrawStickMan(Brush brush, int thickness)
        {
            foreach (var run in SkeletonSegmentRuns)
            {
                var next = this.GetJointPoint(run[0]);
                for (var i = 1; i < run.Length; i++)
                {
                    var prev = next;
                    next = this.GetJointPoint(run[i]);

                    var line = new Line
                    {
                        Stroke = brush,
                        StrokeThickness = thickness,
                        X1 = prev.X,
                        Y1 = prev.Y,
                        X2 = next.X,
                        Y2 = next.Y,
                        StrokeEndLineCap = PenLineCap.Round,
                        StrokeStartLineCap = PenLineCap.Round
                    };

                    this.StickMen.Children.Add(line);
                }
            }
        }


    }


}
