using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using Microsoft.Kinect;
using Fizbin.Kinect.Gestures.Segments;
using Microsoft.Kinect.Toolkit;
using Fizbin.Kinect.Gestures;
using Kinect_Architecture;
using Coding4Fun.Kinect.Wpf.Controls;
using System.Windows.Threading;
using System.Windows.Media.Animation;


public class Curseur
{


    private List<Image> imageButtons;
    public System.Windows.Controls.Button selected;
    public List<System.Windows.Controls.Button> buttons;
    public Point hand;

    private bool isLeft;
    private bool isTrackable;
    public Timer timer = new Timer();
    public int currentX;
    public int currentY;
    public int handWidth;
    public int handHeight;

    public Curseur()
    {
        this.buttons = null;
        this.imageButtons = new List<Image>();
    }

    public Curseur(List<System.Windows.Controls.Button> buttons)
    {
        this.buttons = buttons;
        this.imageButtons = new List<Image>();
    }

    //track and display hand
    public void TrackHand(KinectSensor sensor, Skeleton skeleton)
    {

        int leftX, leftY, rightX, rightY;
        Joint leftHandJoint = skeleton.Joints[JointType.HandLeft];
        Joint rightHandJoint = skeleton.Joints[JointType.HandRight];

        float leftZ = leftHandJoint.Position.Z;
        float rightZ = rightHandJoint.Position.Z;

        ScaleXY(skeleton, false, leftHandJoint, out leftX, out leftY);
        ScaleXY(skeleton, true, rightHandJoint, out rightX, out rightY);

        if (leftHandJoint.TrackingState == JointTrackingState.Tracked && leftZ < rightZ && leftY < SystemParameters.PrimaryScreenHeight)
        {
            this.isTrackable = true;
            this.isLeft = true;

            currentX = leftX;
            currentY = leftY;
        }
        else if (rightHandJoint.TrackingState == JointTrackingState.Tracked && rightY < SystemParameters.PrimaryScreenHeight)
        {
            this.isTrackable = true;
            this.isLeft = false;

            currentX = rightX;
            currentY = rightY;
        }
        else
        {
            this.isTrackable = false;

            currentX = -1;
            currentY = -1;
        }

        hand.X = currentX;
        hand.Y = currentY;
        if (this.buttons != null)
            if (isHandOver(hand, buttons))
            {
                selected.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, selected));
            }

    }

    //detect if hand is overlapping over any button
    private bool isHandOver(Point hand, List<System.Windows.Controls.Button> buttonslist)
    {
        foreach (System.Windows.Controls.Button target in buttonslist)
        {
            Point targetTopLeft = new Point(Canvas.GetLeft(target), Canvas.GetTop(target));
            if (hand.X > targetTopLeft.X &&
                hand.X < targetTopLeft.X + target.Width &&
                hand.Y > targetTopLeft.Y &&
                hand.Y < targetTopLeft.Y + target.Height)
            {
                selected = target;
                return true;
            }
        }
        return false;
    }


    // Used to set whether the hand is the left or right hand. True = Left, False = Right.
    public bool IsLeft
    {
        get
        {
            return this.isLeft;
        }

        set
        {
            this.isLeft = value;
        }
    }

    // Used to set whether the hand is trackable.
    public bool IsTrackable
    {
        get
        {
            return this.isTrackable;
        }

        set
        {
            this.isTrackable = value;
        }
    }

    private static double ScaleY(Joint joint)
    {
        double y = ((SystemParameters.PrimaryScreenHeight / 0.4) * -joint.Position.Y) +
                   (SystemParameters.PrimaryScreenHeight / 2);
        return y;
    }

    private void ScaleXY(Skeleton skeleton, bool rightHand, Joint joint, out int scaledX, out int scaledY)
    {


        double screenWidth = SystemParameters.PrimaryScreenWidth;
        double screenHeight = SystemParameters.PrimaryScreenHeight;

        double x = 0;
        double y = ScaleY(joint);

        // if rightHand then place shouldCenter on left of screen
        // else place shouldCenter on right of screen
        if (rightHand)
        {
            x = (joint.Position.X - skeleton.Joints[JointType.ShoulderCenter].Position.X) * screenWidth * 2;
        }
        else
        {
            x = screenWidth - ((skeleton.Joints[JointType.ShoulderCenter].Position.X - joint.Position.X) * (screenWidth * 2));
        }


        if (x < 0)
        {
            x = 0;
        }
        else if (x >= screenWidth - handWidth)
        {
            x = screenWidth - handWidth;
        }

        if (y < 0)
        {
            y = 0;
        }
        else if (y >= screenHeight - handHeight)
        {
            y = screenHeight - handHeight;
        }

        scaledX = (int)x;
        scaledY = (int)y;

    }

    // Event when kinectButton TimeInterval ends
    public void kinectButton_Click(object sender, RoutedEventArgs e)
    {
        selected.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, selected));
    }
}
