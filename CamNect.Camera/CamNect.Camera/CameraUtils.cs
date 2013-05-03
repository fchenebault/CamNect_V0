using System;
using System.Windows.Controls;


namespace CamNect.Camera
{
    public class CameraUtils
    {
        private String ip = null;
        private String videoUrl;
        private CamConfig config;
        private MjpegReader reader;

        // protected VlcPlayer vlcPlayer;

        public CameraUtils(string ip, CamConfig cfg)
        {
            this.ip = ip;
            this.videoUrl = "http://" + cfg.Id + ":" + cfg.Pass + "@" + ip + cfg.Uri;
            this.config = cfg;
            this.reader = new MjpegReader();
        }

        public void startReader(Image imageReader)
        {
            reader.MjpegReaderStop();
            reader.setReader(imageReader, config, ip);  
        }

        public CamConfig Config
        {
            get{return config;}
        }

        public String VideoUrl
        {
            get { return ip; }
        }

        public String Ip
        {
            get { return ip;}
        }

        public void light(int value)
        {
            HttpReq.HttpGet("http://" + ip + "/axis-cgi/lightcontrol.cgi?level=" + value, config.Id, config.Pass);
        }

        public void playMediaClip(int idMediaClip)
        {
            // idMediaClip 
            // 6 : Click
            // 7 : Psst
            // 8 : Intruder
            // 9 : Dog barking
            HttpReq.HttpGet("http://" + ip + "/axis-cgi/mediaclip.cgi?action=play&clip=" + idMediaClip, config.Id, config.Pass);
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

        public void goUpLeft()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?move=upleft", config.Id, config.Pass);
        }

        public void goDownLeft()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?move=downleft", config.Id, config.Pass);
        }

        public void goUpRight()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?move=upright", config.Id, config.Pass);
        }

        public void goDownRight()
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?move=downright", config.Id, config.Pass);
        }

        public void zoomOnOff(Double z)
        {
            String result;
            result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?rzoom="+z, config.Id, config.Pass);
        }

      //  public void zoomOff()
     //   {
     //       String result;
     //       result = HttpReq.HttpGet("http://" + ip + "/axis-cgi/com/ptz.cgi?rzoom=-300", config.Id, config.Pass);
      //  }
    }

}
