using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System;
using System.Windows.Data;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Kinect.Toolkit.Controls;
using System.Collections.ObjectModel;

namespace CamNect.Kinect
{
    public class KinectMain
    {
        // Variables
        public KinectSensorChooser sensorChooser;
        public GestureCamera gestureCamera;
        public Skeleton skeletonFocus;
        public ReadOnlyCollection<HandPointer> handsTracked;
        public HandPointer primaryHand;


        // Constructor
        public KinectMain(KinectSensorChooser sensorChooser, KinectSensorChooserUI sensorChooserUi, KinectRegion kinectRegion)
        {
            this.sensorChooser = sensorChooser;
            InitKinect(sensorChooserUi, kinectRegion);
            gestureCamera = new GestureCamera();
            handsTracked = kinectRegion.HandPointers;
        }

        public bool AreHandsTracked()
        {
            if (handsTracked.Count > 0)
                return true;
            else
                return false;
        }

        public HandPointer getPrimaryHand(ReadOnlyCollection<HandPointer> handsTracked)
        {
            primaryHand = null;
            foreach (HandPointer hand in handsTracked)
            {
                if (hand.IsPrimaryHandOfUser)
                    primaryHand = hand;
            }
            return this.primaryHand;
        }

        // Initialise the Kinect
        public void InitKinect(KinectSensorChooserUI sensorChooserUi, KinectRegion kinectRegion)
        {
            // initialize the sensor chooser and UI
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();

            // Bind the sensor chooser's current sensor to the KinectRegion
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
        }


        // Check the Kinect's status
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
    }
}
