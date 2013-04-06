using Fizbin.Kinect.Gestures;
using Fizbin.Kinect.Gestures.Segments;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    class GestureCamera : Gesture
    {

        public GestureCamera()
        {
            gestureController = new GestureController();
            gestureController.GestureRecognized += OnGestureRecognized;
            gestureKind = "none";
            InitGesture();
        }

        public override void InitGesture()
        {
            // Add SwipeLeft
            IRelativeGestureSegment[] swipeleftSegments = new IRelativeGestureSegment[3];
            swipeleftSegments[0] = new SwipeLeftSegment1();
            swipeleftSegments[1] = new SwipeLeftSegment2();
            swipeleftSegments[2] = new SwipeLeftSegment3();
            gestureController.AddGesture("SwipeLeft", swipeleftSegments);

            // Add SwipeRight
            IRelativeGestureSegment[] swiperightSegments = new IRelativeGestureSegment[3];
            swiperightSegments[0] = new SwipeRightSegment1();
            swiperightSegments[1] = new SwipeRightSegment2();
            swiperightSegments[2] = new SwipeRightSegment3();
            gestureController.AddGesture("SwipeRight", swiperightSegments);

            IRelativeGestureSegment[] swipeUpSegments = new IRelativeGestureSegment[3];
            swipeUpSegments[0] = new SwipeUpSegment1();
            swipeUpSegments[1] = new SwipeUpSegment2();
            swipeUpSegments[2] = new SwipeUpSegment3();
            gestureController.AddGesture("SwipeUp", swipeUpSegments);

            IRelativeGestureSegment[] swipeDownSegments = new IRelativeGestureSegment[3];
            swipeDownSegments[0] = new SwipeDownSegment1();
            swipeDownSegments[1] = new SwipeDownSegment2();
            swipeDownSegments[2] = new SwipeDownSegment3();
            gestureController.AddGesture("SwipeDown", swipeDownSegments);
         }

        public override void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            switch (e.GestureName)
            {
                case "SwipeLeft":
                    gestureKind = "Swipe Left";
                    Console.WriteLine("Left");
                    this.isGestureOn = true;
                    OnSwipeLeftEvent();
                    break;
                case "SwipeRight":
                    gestureKind = "Swipe Right";
                    Console.WriteLine("Right");
                    this.isGestureOn = true;
                    OnSwipeRightEvent();
                    break;
                case "SwipeUp":
                    gestureKind = "Swipe Up";
                    Console.WriteLine("Up");
                    this.isGestureOn = true;
                    break;
                case "SwipeDown":
                    gestureKind = "Swipe Down";
                    Console.WriteLine("Down");
                    this.isGestureOn = true;
                    break;
                default:
                    //Debug.WriteLine("Recognized gesture not existing");
                    break;
            }
        }

        public delegate void SwipeLeftEvent();
        public event SwipeLeftEvent OnSwipeLeftEvent;

        public delegate void SwipeRightEvent();
        public event SwipeRightEvent OnSwipeRightEvent;   
    }
