﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MjpegProcessor;
using System.Windows.Media;
using System.Windows.Controls;
using System.Net;
using System.Windows.Media.Imaging;

namespace CamNect.Camera
{
    public class MjpegReader
    {
        // Variables
        private Image reader;
        private MjpegDecoder _mjpeg;

        public MjpegReader(CameraUtils camera, Image reader)
        {
            _mjpeg = new MjpegDecoder();
            this.reader = reader;
            String url = "http://" +camera.Ip + "/mjpg/video.mjpg";

            _mjpeg.FrameReady += mjpeg_FrameReady;
            _mjpeg.Error += mjpeg_Error;
            _mjpeg.ParseStream(new Uri(url), camera.Config.Id, camera.Config.Pass);
        }
       
        // In case of Errors
        void mjpeg_Error(object sender, ErrorEventArgs e)
        {
            Uri uri = new Uri("/Ressources/Images/warning_big.png", UriKind.Relative);
            ImageSource bi = new BitmapImage(uri);
            reader.Source = bi;
        }

        // For new video frame
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
