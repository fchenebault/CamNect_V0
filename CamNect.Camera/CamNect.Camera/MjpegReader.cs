using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MjpegProcessor;
using System.Windows.Media;
using System.Windows.Controls;
using System.Net;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CamNect.Camera
{
    public class MjpegReader
    {
        // Variables
        private Image reader;
        private MjpegDecoder _mjpeg;
        private CameraUtils camera;
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MjpegReader(CameraUtils camera, Image reader, String resolution)
        {
            _mjpeg = new MjpegDecoder();
            this.reader = reader;
            this.camera = camera;
            //String url = camera.VideoUrl + "?resolution=" + resolution;

            String url = "http://" + camera.Ip + "/mjpg/video.mjpg?resolution=" + resolution;

            _mjpeg.FrameReady += mjpeg_FrameReady;
            _mjpeg.Error += mjpeg_Error;
            _mjpeg.ParseStream(new Uri(url), camera.Config.Id, camera.Config.Pass);

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(3000);
        }

        // In case of Errors
        void mjpeg_Error(object sender, ErrorEventArgs e)
        {
            Uri uri = new Uri("/ressources/images/warning_big.png", UriKind.Relative);
            ImageSource bi = new BitmapImage(uri);
            reader.Source = bi;
        }

        // For new video frame
        private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
        {
            reader.Source = e.BitmapImage;
            dispatcherTimer.Stop();
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            /*
            dispatcherTimer.Stop();
            _mjpeg.StopStream();
            Uri uri = new Uri("/Ressources/Images/warning_big.png", UriKind.Relative);
            ImageSource bi = new BitmapImage(uri);

            this.camera.Config.Plugged = false;
            reader.Source = bi;
             * */
        }

        public void MjpegReaderStop()
        {
            _mjpeg.StopStream();
        }

    }
}