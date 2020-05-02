using Microsoft.Win32;
using MediaPack.Data;
using MediaPack.Dialogs.Channel;
using MediaPack.Helpers;
using MediaPack.Models.Channel.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MediaPack.Models.Channel.Enums;
using System.Threading;
using MediaPack.Models.Common.Entities;
using System.Windows.Controls;

namespace MediaPack.ViewModel.Channel
{
    public class ImportChannelsViewModel : WindowViewModel
    {
        public ImportChannelsViewModel(Window window) : base(window)
        {
            mWindow = window;
            WindowMinimumHeight = 900;
            WindowMinimumWidth = 700;

            Title = "Toplu Kanal Ekleme";
            InsertingStatus = "Çıktı dosyasını seçiniz...";

            InsertPlaylistCommand = new RelayCommand(p => InsertChannels());
            CancelOperationCommand = new RelayCommand(p => CancelOperation());
            SaveChannelsCommand = new RelayCommand(p => SaveChannels());
            RemoveChannelCommand = new RelayParameterizedCommand(RemoveChannel);

            Channels = new ObservableCollection<Models.Channel.Entities.Channel>();
            Countries = new ObservableCollection<Country>();
            ChannelTypes = Enum.GetValues(typeof(ChannelType)).Cast<ChannelType>().ToObservableCollection();
            SelectedChannelType = ChannelType.Tv;

            LoadCountries();
        }

        #region Commands

        public ICommand InsertPlaylistCommand { get; set; }
        public ICommand CancelOperationCommand { get; set; }
        public ICommand SaveChannelsCommand { get; set; }
        public ICommand RemoveChannelCommand { get; set; }

        #endregion


        #region Public Properties

        public ObservableCollection<Models.Channel.Entities.Channel> Channels { get; set; }
        public ObservableCollection<ChannelType> ChannelTypes { get; set; }
        public ChannelType SelectedChannelType { get; set; }

        public ObservableCollection<Country> Countries { get; set; }
        public Country SelectedCountry { get; set; }

        public string InsertingStatus { get; set; }
        public bool IsInserting { get; set; }
        public bool IsListed { get; set; }
        public bool IsSaved { get; set; }

        #endregion


        #region Methods

        public void LoadCountries()
        {
            using var db = new AppDbContext();

            Countries = db.Countries.ToObservableCollection();

            SelectedCountry = Countries.Single(x => x.Alpha3code == "TUR");
        }

        public void CancelOperation()
        {
            IsInserting = false;
            IsListed = false;
            IsSaved = false;

            InsertingStatus = "İçe aktarmak için JSON dosyasını seçiniz...";

            Channels.Clear();
        }

        public void RemoveChannel(object sender)
        {
            if (!(sender is Button button)) return;

            var channel = (Models.Channel.Entities.Channel)button.DataContext;

            Channels.Remove(channel);
        }

        public void SaveChannels()
        {
            if (Channels.Count > 0)
            {
                using var db = new AppDbContext();

                foreach (var channel in Channels)
                {
                    if (db.Channels.Any(x => x.Name == channel.Name && x.Country == SelectedCountry.Alpha3code))
                    {
                        db.Channels.Update(channel);
                    }
                    else
                    {
                        channel.Country = SelectedCountry.Alpha3code;

                        db.Channels.Add(channel);
                    }

                    db.SaveChanges();
                }

                IsInserting = false;
                IsListed = false;
                IsSaved = true;
                Channels.Clear();

                InsertingStatus = $"{Channels.Count} tane kanal eklendi ve güncellendi.";
            }
            else
            {
                InsertingStatus = "Eklenecek veya güncellenecek bir kanal bulunamadı.";
            }
        }

        public void InsertChannels()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select m3u8 Playlist File",
                Filter = "JSON Files (*.json)|*.json|m3u8 Files (*.m3u8)|*.m3u8",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var ext = Path.GetExtension(openFileDialog.FileName);

                if (ext == ".m3u8")
                {
                    var extM3U = m3uParser.M3U.ParseFromFile(openFileDialog.FileName);

                    if (extM3U.Medias.Count() > 0)
                    {
                        new Thread(() =>
                        {
                            Thread.CurrentThread.IsBackground = true;

                            IsInserting = true;

                            foreach (var media in extM3U.Medias)
                            {
                                var channelName = media.Title.RawTitle ?? media.Title.InnerTitle;

                                if (!string.IsNullOrWhiteSpace(channelName))
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        Channels.Add(new Models.Channel.Entities.Channel
                                        {
                                            ChannelType = ChannelType.Tv,
                                            M3U8Address = media.MediaFile,
                                            Name = channelName
                                        });
                                    });
                                }
                            }

                            InsertingStatus = $"{Channels.Count} adet Kanal bulundu.";
                            IsInserting = false;
                            IsListed = true;
                        }).Start();
                    }
                    else
                    {
                        InsertingStatus = "İçe aktarılacak uygun bir kanal bulunamadı.";
                    }
                }
                else if (ext == ".json")
                {
                    var channelsData = File.ReadAllText(openFileDialog.FileName);
                    var jsonObjChannels = JsonConvert
                        .DeserializeObject<ObservableCollection<Models.Channel.Entities.Channel>>
                        (channelsData);

                    if (jsonObjChannels.Count > 0)
                    {
                        Channels = jsonObjChannels;
                        InsertingStatus = $"{Channels.Count} adet Kanal bulundu.";
                        IsInserting = false;
                        IsListed = true;
                    }
                }
            }
        }

        #endregion
    }
}