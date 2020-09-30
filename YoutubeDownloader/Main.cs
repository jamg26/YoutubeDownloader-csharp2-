using System;
using System.IO;
using System.Windows.Forms;
using VideoLibrary;
using System.Threading;
using System.Diagnostics;

namespace YoutubeDownloader
{
    public partial class Main : Form
    {
        public Thread t;
        public int length, counter;

        public Main()
        {
            InitializeComponent();
        }

        private void SaveVideoToDisk(string link)
        {
            txtStatus.Invoke((MethodInvoker)(() =>
            {
                btnDownload.Enabled = false;
            }));
            var youTube = YouTube.Default; // starting point for YouTube actions
            try
            {
                var video = youTube.GetVideo(link); // gets a Video object with info about the video

                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                filePath += @"\youtube\";

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                File.WriteAllBytes(filePath + video.FullName, video.GetBytes());

                txtStatus.Invoke((MethodInvoker)(() =>
                {
                    txtStatus.Text = $"{video.FullName} - done";
                    GC.Collect();
                    counter++;
                    if (counter == length)
                    {
                        txtStatus.Text = "Finished.";
                        btnDownload.Enabled = true;
                        Process.Start(filePath);
                    }
                }));
            }
            catch (Exception e)
            {
                txtStatus.Invoke((MethodInvoker)(() =>
                {
                    MessageBox.Show(e.Message);
                    txtStatus.Text = e.Message;
                    btnDownload.Enabled = true;
                    GC.Collect();
                }));
            }
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Created by: Jamuel Galicia\nIf you found bugs please report to me.");
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Text = "Youtube Downloader";
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            string[] links = txtUrl.Text.Split('\n');
            length = links.Length;
            counter = 0;
            Thread[] t = new Thread[length];
            for (int x = 0; x < links.Length; x++)
            {
                string value = links[x];
                t[x] = new Thread(() => SaveVideoToDisk($"{value}"));
                t[x].Start();
            }
            txtStatus.Text = "Downloading started.";
        }
    }
}