using System;
using System.Windows.Controls;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;


namespace CamNect.Camera
{
    public abstract class Camera
    {
        protected String ip = null;
        protected String url = null;
        protected VlcPlayer vlcPlayer;
        protected bool connected = false;

        public abstract void initCamera(String ip);
        //public abstract void Rec();

        
        public Camera(VlcControl control, Image imgPlayer)
        {
            vlcPlayer = new VlcPlayer(control, imgPlayer);
        }

        public void Play()
        {
            vlcPlayer.Control.Play(vlcPlayer.Media);
        }

        public void Stop()
        {
            vlcPlayer.Control.Stop();
        }

        public void takeScreenshot()
        {
            vlcPlayer.Control.TakeSnapshot("Screenshots/Screen" + DateTime.Now.Ticks.ToString() + ".png", 800, 600);
        }

        public void startCapture()
        {
            vlcPlayer.Media.AddOption(":sout=#transcode{vcodec=mpeg4,vb=1024,scale=1,acodec=mp3,ab=128,channels=2,samplerate=44100}:duplicate{dst=file{access=file,mux=ts,dst=Record/video"+DateTime.Now.Ticks.ToString()+".avi},dst=display}");
            vlcPlayer.Control.Play(vlcPlayer.Media);

        }

        public void stopCapture()
        {
            vlcPlayer.Media.AddOption(":sout=#transcode{vcodec=mpeg4,vb=1024,scale=1,acodec=mp3,ab=128,channels=2,samplerate=44100}:duplicate{dst=display}");
            vlcPlayer.Control.Play(vlcPlayer.Media);
        }

        public VlcPlayer VlcPlayer
        {
            get { return vlcPlayer; }
        }
    }


    public class CameraPTZ : Camera
    {

        public CameraPTZ(VlcControl control, Image imgPlayer) : base(control, imgPlayer) { }

        public override void initCamera(String ip)
        {
            this.ip = ip;
            url = "rtsp://root:root@" + ip + "/mpeg4/media.amp";
            vlcPlayer.Media = new PathMedia(url);

            connected = true;
        }

        public void goLeft()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?move=left");
        }

        public void goRight()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?move=right");
        }

        public void goUp()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?move=up");
        }

        public void goDown()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?move=down");
        }

        public void zoomOn()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?rzoom=300");
        }

        public void zoomOff()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?rzoom=-300");
        }
    }



    public class CameraSTD : Camera
    {

        public CameraSTD(VlcControl control, Image imgPlayer) : base(control, imgPlayer) { }

        public override void initCamera(String ip)
        {
            this.ip = ip;
            url = "rtsp://root:root@" + ip + "/axis-media/media.amp?videocodec=jpeg";
            vlcPlayer.Media = new PathMedia(url);
            connected = true;
        }

    }
}
