using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MjpegProcessor;
using System.Windows.Media;
using System.Windows.Controls;
using System.Net;

namespace CamNect.Camera
{
    public class MjpegReader
    {
        // Variables
        private Image reader;
        private MjpegDecoder _mjpeg;

        public MjpegReader(CamConfig cfg, String ip, Image reader)
        {
            _mjpeg = new MjpegDecoder();
            this.reader = reader;
            String url = "http://" + ip + "/mjpg/video.mjpg";

            _mjpeg.FrameReady += mjpeg_FrameReady;
            _mjpeg.ParseStream(new Uri(url), cfg.Id, cfg.Pass);
        }

        private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
        {
             reader.Source = e.BitmapImage;
        }

        public void MjpegReaderStop()
        {
            _mjpeg.StopStream();
        }

    }
}
