using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Accord.Video.FFMPEG;

namespace Recorder
{
    class ScreenRecorder
    {
        private Rectangle bounds;
        private string outputPath = "";
        private string tempPath = "";
        private int fileCount = 1;
        private List<string> inputImageSequence = new List<string>();
        private string videoName = "video.mp4";
        private string finalName = "FinalVideo.mp4";


        Stopwatch watch = new Stopwatch();
        public ScreenRecorder(Rectangle b, string outPath)
        {

            CreateTempFolder("tempScreenCaps");

            bounds = b;
            outputPath = outPath;
        }


        private void CreateTempFolder(string name)
        {
            if (Directory.Exists("D://"))
            {
                string pathName = $"D://{name}";
                Directory.CreateDirectory(pathName);
                tempPath = pathName;
            }
            else
            {
                string pathName = $"C://Documents//{name}";
                Directory.CreateDirectory(pathName);
                tempPath = pathName;
            }
        }


        public void setVideoName(string name)
        {
            finalName = name;
        }


        public string getElapsed()
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}", watch.Elapsed.Hours, watch.Elapsed.Minutes, watch.Elapsed.Seconds);
        }


        public void RecordVideo()
        {

            watch.Start();

            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {

                    g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                }

                string name = tempPath + "//screenshot-" + fileCount + ".png";
                bitmap.Save(name, ImageFormat.Png);
                inputImageSequence.Add(name);
                fileCount++;
                bitmap.Dispose();
            }
        }
        private void SaveVideo(int width, int height, int frameRate)
        {
            int c = 0;
            using (VideoFileWriter vFWriter = new VideoFileWriter())
            {
                vFWriter.Open(outputPath + "//" + videoName, width, height, frameRate, VideoCodec.MPEG4);
                foreach (string imageLocation in inputImageSequence)
                {
                    Bitmap imageFrame = System.Drawing.Image.FromFile(imageLocation) as Bitmap;
                    vFWriter.WriteVideoFrame(imageFrame);
                    imageFrame.Dispose();
                }
                vFWriter.Close();
                File.Copy(Path.Combine(outputPath, videoName), @"C:\Test\video.mp4");
            }
        }

        public void Stop()
        {
            watch.Stop();
            int width = bounds.Width;
            int height = bounds.Height;
            int frameRate = 10;
            SaveVideo(width, height, frameRate);
        }
    }
}
