using FFmpeg.AutoGen;
using MediaPack.Data;
using MediaPack.Dialogs.Update;
using MediaPack.Helpers;
using MediaPack.Models.Channel.Entities;
using MediaPack.Models.Channel.Enums;
using MediaPack.ViewModel.Update;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Unosquare.FFME;

namespace MediaPack
{
    public class Setup
    {
        public Setup()
        {
            ResourceDirectories = new List<string>
            {
                @$"{Settings.CurrentDirectory}\Resources\Channels\Logos",
                @$"{Settings.CurrentDirectory}\Records\Tv",
                @$"{Settings.CurrentDirectory}\Records\Radio",
                @$"{Settings.CurrentDirectory}\Records\Thumbnails"
            };

            CheckResourcesFolder();

            if (FFmpegCheck())
            {
                FFmpegSetup();
            }

            InsertRadios();
        }

        public List<string> ResourceDirectories { get; set; }

        public bool FFmpegCheck()
        {
            var status = false;

            if (!Directory.Exists($"{Settings.CurrentDirectory}\\FFmpeg\\bin"))
            {
                var message = MessageBox.Show("FFmpeg dosyaları bulunamadı, videoların oynatılabilmesi için gerekli. İndirmek istiyor musunuz?","FFmpeg Eksik", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (message == MessageBoxResult.Yes)
                {
                    var dialog = new FFmpegDownloaderDialog();

                    dialog.Closing += (sender, args) =>
                    {
                        if (!Directory.Exists($"{Settings.CurrentDirectory}\\FFmpeg\\bin"))
                        {
                            status = true;
                        }
                        else
                        {
                            MessageBox.Show("FFmpeg dosyalarını bulamadık. Uygulamadan çıkılıyor.", "Uygulama Sonlandırılıyor", MessageBoxButton.OK, MessageBoxImage.Information);
                            Application.Current.Shutdown();
                        }
                    };

                    dialog.ShowDialogWindow(new FFmpegDownloaderViewModel(dialog));
                }
                else
                {
                    MessageBox.Show("FFmpeg dosyalarını almayı redettiniz. Uygulamadan çıkılıyor", "Uygulama Sonlandırılıyor", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown();
                }
            }
            else
            {
                status = true;
            }

            return status;
        }

        public async void FFmpegSetup()
        {
            Library.FFmpegDirectory = $"{Settings.CurrentDirectory}\\FFmpeg\\bin";
            Library.FFmpegLoadModeFlags = FFmpegLoadMode.MinimumFeatures;
            Library.EnableWpfMultiThreadedVideo = !Debugger.IsAttached;
            Library.EnableWpfMultiThreadedVideo = true;
            await Library.LoadFFmpegAsync();
        }

        public void CheckResourcesFolder()
        {
            if (ResourceDirectories.Count > 0)
            {
                foreach (var dir in ResourceDirectories)
                {
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }
            }
        }

        public void InsertRadios()
        {
            //using var db = new AppDbContext();

            //var channels = db.Channels
            //.Where(x => x.ChannelType == ChannelType.Radio)
            //.Select(x => new Models.Channel.Entities.Channel
            //{
            //    Id = x.Id,
            //    Name = x.Name,
            //    Description = x.Description,
            //    M3U8Address = x.M3U8Address,
            //    IsFavorite = x.IsFavorite,
            //    Country = x.Country,
            //    ChannelType = x.ChannelType,
            //    Logo = File.Exists(@$"{Settings.CurrentDirectory}\{x.Logo}") ?
            //           @$"{Settings.CurrentDirectory}\{x.Logo}".PathToBitmapImage() : null
            //})
            //.OrderBy(x => x.Name)
            //.ToObservableCollection();

            //foreach (var item in channels)
            //{
            //    if (item != null && item.Country == null)
            //    {
            //        item.Country = "TUR";
            //        db.Channels.Update(item);
            //        db.SaveChanges();
            //    }
            //}
        }
    }
}
