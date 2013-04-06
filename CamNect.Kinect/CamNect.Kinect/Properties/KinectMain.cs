using Microsoft.Kinect;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Collections.Generic;
using Coding4Fun.Kinect.Wpf.Controls;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;


namespace Kinect_Architecture
{
    class KinectMain
    {
        private KinectSensor sensor;
        private KinectSensorChooser sensorChooser;
        private SkeletonManagement[] SkeletonManagementData;
        public Curseur curseur;
        public GestureCamera gestureCamera;
        

        private int nearestId;
        private DateTime highlightTime;
        private int highlightId;

        public KinectMain(KinectSensor sensor)
        {
            this.sensor = sensor;
            curseur = new Curseur();
            gestureCamera = new GestureCamera();
            SkeletonManagementData = new SkeletonManagement[0];
            nearestId = -1;
            highlightId = -1;
            highlightTime = DateTime.MinValue;
            this.sensor.SkeletonFrameReady += this.sensor_SkeletonFramesReady;

        }

        public KinectMain(KinectSensor sensor, List<Button> buttons)
        {
            this.sensor = sensor;
            curseur = new Curseur(buttons);
            gestureCamera = new GestureCamera();
            SkeletonManagementData = new SkeletonManagement[0];
            nearestId = -1;
            highlightId = -1;
            highlightTime = DateTime.MinValue;
            this.sensor.SkeletonFrameReady += this.sensor_SkeletonFramesReady;
           
        }

        private void HighlightSkeleton(Skeleton skeleton)
        {
            // Set the highlight time to be a short time from now.
            highlightTime = DateTime.UtcNow + TimeSpan.FromSeconds(0.5);

            // Record the ID of the skeleton.
            highlightId = skeleton.TrackingId;
        }

        //For Skeleton
        public void sensor_SkeletonFramesReady(Object sender, SkeletonFrameReadyEventArgs e)
        {
            int i;

            Skeleton skeletonFocus;
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {

                    if ((this.SkeletonManagementData.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        // Actualise le tableau de SkeletonManagement en fonction du nombre de Skeleton
                        SkeletonManagementData = new SkeletonManagement[skeletonFrame.SkeletonArrayLength];
                    }

                    for (i = 0; i < SkeletonManagementData.Length; i++)
                    {
                        // Initialise les skeletons du tableau skeletonManagementData à partir des skeletons détectés
                        SkeletonManagementData[i] = new SkeletonManagement(skeletonFrame, i);
                    }

                    /////////////////////// SELECTION SKELETON //////////////////////

                    // Assume no nearest skeleton and that the nearest skeleton is a long way away.
                    float nearestDistance = (float)double.MaxValue;

                    for (i = 0; i < SkeletonManagementData.Length; i++)
                    {
                        // Only consider tracked skeletons.
                        if (SkeletonManagementData[i].skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            if (SkeletonManagementData[i].isSkeletonNearest(nearestDistance))
                            {
                                // Récupère la plus proche distance
                                nearestDistance = SkeletonManagementData[i].distance;

                                // Récupère l'id du skeleton correspondant
                                nearestId = SkeletonManagementData[i].skeleton.TrackingId;
                            }
                        }
                    }

                    for (i = 0; i < SkeletonManagementData.Length; i++)
                    {



                        ////////////////////////// CURSEUR ///////////////////////////////

                        if (SkeletonManagementData[i].skeleton.TrackingId == nearestId)
                        {
                            skeletonFocus = SkeletonManagementData[i].skeleton;


                            this.curseur.TrackHand(sensor, skeletonFocus);




                            gestureCamera.OnGesture(SkeletonManagementData[i].skeleton);
                            nearestId = SkeletonManagementData[i].skeleton.TrackingId;


                        }
                    }
                }
            }
        }


        //When your window is loaded
        public void InitKinect()
        {
            
        }

 
    }



}
