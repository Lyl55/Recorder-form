using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        Bitmap bmp;
        Graphics gr;

        string outputPath = "";
        bool pathSelected = false;
        string finalVidName = "FinalVideo.mp4";
        ScreenRecorder screenRec = new ScreenRecorder(new Rectangle(), "");

        private void button1_Click(object sender, EventArgs e)
        {
            bool containsMP4 = finalVidName.Contains(".mp4");

            if (pathSelected && containsMP4)
            {
                screenRec.setVideoName(finalVidName);
                timer1.Start();
            
            }

            else
            {
               MessageBox.Show("You must select video name that ends in '.mp4' " +
                                "and you must select an output path", "Error");
                finalVidName = "FinalVideo.mp4";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse("192.168.43.155"), 9095));
            MessageBox.Show("Sent");
            screenRec.Stop();
            while (true)
            {
                var buffer = File.ReadAllBytes(@"C:\Test\video.mp4");
                socket.Send(buffer);
                Thread.Sleep(1000);
                timer1.Stop();
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                bmp = new Bitmap(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                gr = Graphics.FromImage(bmp);
                gr.CopyFromScreen(0, 0, 0, 0, new Size(bmp.Width, bmp.Height));
                pictureBox1.Image = bmp;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                screenRec.RecordVideo();
                label1.Text = screenRec.getElapsed();
            }
            catch (Exception mes)
            {
                MessageBox.Show(mes.Message);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse("192.168.43.155"), 9095));
            MessageBox.Show("Connected");
        }

        
        private void button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "Select an Output Folder";

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outputPath = @folderBrowser.SelectedPath;
                pathSelected = true;

                Rectangle bounds = Screen.FromControl(this).Bounds;
                screenRec = new ScreenRecorder(bounds, outputPath);
            }
            else
            {
                MessageBox.Show("Please select an output folder.", "Error");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
