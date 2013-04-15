using System;
using System.Windows.Controls;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;


namespace CamNect.Camera
{
    public class CameraUtils
    {
        private String ip = null;
        private String videoUrl;
        private CamConfig config;
        // protected VlcPlayer vlcPlayer;

        public CameraUtils(string ip, CamConfig cfg)
        {
            this.ip = ip;
            this.videoUrl = "http://" + cfg.Id + ":" + cfg.Pass + "@" + ip + cfg.Uri;
            this.config = cfg;
        }

        public CamConfig Config
        {
            get{return config;}

        }

        public void goLeft()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?move=left", config.Id, config.Pass);
        }

        public void goRight()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?move=right", config.Id, config.Pass);
        }

        public void goUp()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?move=up", config.Id, config.Pass);
        }

        public void goDown()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?move=down", config.Id, config.Pass);
        }

        public void zoomOn()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?rzoom=300", config.Id, config.Pass);
        }

        public void zoomOff()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?rzoom=-300", config.Id, config.Pass);
        }
    }

}
