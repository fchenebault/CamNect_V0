using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fizbin.Kinect.Gestures;
using System.ComponentModel;
using Microsoft.Kinect;



    abstract class Gesture
    {
        /* Variables */
        public GestureController gestureController;
        private string _gesture; // Needed to identify the gesture
        public event PropertyChangedEventHandler PropertyChanged;
        public String gestureKind
        {
            get { return _gesture; }
            set
            {
                if (_gesture == value)
                {
                    return;
                }

                _gesture = value;

                if (this.PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Gesture"));
                }
             }
        }

        public bool isGestureOn;
        

        /* This method initialises all the gestures needed in the special case */
        abstract public void InitGesture();

        /* This method implements gesture definitions (association between a name and a gesture) */ 
        abstract public void OnGestureRecognized(object sender, GestureEventArgs e);

        /* This method is to update the gestureController */
        public void OnGesture(Skeleton skeleton)
        {
            gestureController.UpdateAllGestures(skeleton);
        }
    }

    //private void OnGestureRecognized(object sender, GestureEventArgs e)
    //{
    //    switch (e.GestureName)
    //    {
    //        case "Menu":
    //            Gesture = "Menu";
    //            break;
    //        case "WaveRight":
    //            Gesture = "Wave Right";
    //            break;
    //        case "WaveLeft":
    //            Gesture = "Wave Left";
    //            break;
    //        case "JoinedHands":
    //            Gesture = "Joined Hands";
    //            break;
    //        case "SwipeLeft":
    //            Gesture = "Swipe Left";
    //            HighlightSkeleton(skeletonFocus);
    //            System.Console.WriteLine("Left");
    //            SkeletCanvas.Background = Brushes.DarkMagenta;
    //            break;
    //        case "SwipeRight":
    //            Gesture = "Swipe Right";
    //            HighlightSkeleton(skeletonFocus);
    //            SkeletCanvas.Background = Brushes.Lavender;
    //            break;
    //        case "SwipeUp":
    //            Gesture = "Swipe Up";
    //            HighlightSkeleton(skeletonFocus);
    //            SkeletCanvas.Background = Brushes.IndianRed;
    //            break;
    //        case "SwipeDown":
    //            Gesture = "Swipe Down";
    //            HighlightSkeleton(skeletonFocus);
    //            SkeletCanvas.Background = Brushes.Blue;
    //            break;
    //        case "ZoomIn":
    //            Gesture = "Zoom In";
    //            HighlightSkeleton(skeletonFocus);
    //            SkeletCanvas.Background = Brushes.Gold;
    //            break;
    //        case "ZoomOut":
    //            Gesture = "Zoom Out";
    //            HighlightSkeleton(skeletonFocus);
    //            SkeletCanvas.Background = Brushes.Green;
    //            break;

    //        default:
    //            break;
    //    }

    //}