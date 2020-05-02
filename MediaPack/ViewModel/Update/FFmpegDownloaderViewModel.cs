using MediaPack.Data;
using MediaPack.Helpers;
using System;
using System.IO;
using System.Windows;
using System.Threading;
using System.Net;
using System.ComponentModel;
using System.IO.Compression;

namespace MediaPack.ViewModel.Update
{
    public class FFmpegDownloaderViewModel : WindowViewModel
    {
        public FFmpegDownloaderViewModel(Window window) : base(window)
        {
            mWindow = window;
            WindowMinimumHeight = 300;
            WindowMinimumWidth = 450;

            Title = "FFmpeg Güncelleme";
            ProgressStatus = "Dosyalar indirilecek";
            DownloadAddress = "https://ffmpeg.zeranoe.com/builds/win64/shared/ffmpeg-4.2.1-win64-shared.zip";

            DownloadFFmepeg();
        }

        public string DownloadAddress { get; set; }
        public string ProgressStatus { get; set; }
        public double ProgressSize { get; set; }

        public void DownloadFFmepeg()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                WebClient webclient = new WebClient();
                // we need progress changed event for progress
                webclient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgress);
                // when calls download complated
                webclient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleted);
                // set file download url, save folder and start downlaoding
                //string filedownlaodurl = DownloadAddress;
                // Anlık olarak indirilecek olan dosya ".exe"nin olduğu konuma kaydedilir
                webclient.DownloadFileAsync(new Uri(DownloadAddress), Path.GetFileName(DownloadAddress));
            }).Start();
        }

        /// <summary>
        /// WebClient Download Progress Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <see cref="https://msdn.microsoft.com/en-us/library/system.net.downloadprogresschangedeventargs"/>
        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            // download progress
            ProgressSize = e.ProgressPercentage;
            ProgressStatus = $"İndiriliyor... %{e.ProgressPercentage}";
            //// downloaded bytes
            //Downloaded.Text = e.BytesReceived.ToString();
            //// total bytes
            //Total.Text = e.TotalBytesToReceive.ToString();
            //// download percentage as string
            //DownloadPercentage.Text = e.ProgressPercentage.ToString("{0}%");
        }

        /// <summary>
        /// WebClient AsyncCompleted Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <see cref="https://msdn.microsoft.com/tr-tr/library/system.componentmodel.asynccompletedeventhandler"/>
        private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (File.Exists($"{Settings.CurrentDirectory}\\{Path.GetFileName(DownloadAddress)}"))
            {
                using var stream = File.OpenRead(Path.GetFileName(DownloadAddress));
                using var zipArchive = new ZipArchive(stream);

                var zipArchiveHelpers = new ZipArchiveHelpers();
                zipArchiveHelpers.ExtractToDirectory(zipArchive, Settings.CurrentDirectory, true);

                Directory.Move(
                    $"{Settings.CurrentDirectory}\\{Path.GetFileNameWithoutExtension(DownloadAddress)}",
                    $"{Settings.CurrentDirectory}\\FFmpeg"
                );

                MessageBox.Show("İndirme ve kurulum işlemi tamamlandı.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);

                mWindow.Close();
            }
            else
            {
                MessageBox.Show("İndirme işlemi bitti ancak dosya dulunamadı.", "Hata!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}