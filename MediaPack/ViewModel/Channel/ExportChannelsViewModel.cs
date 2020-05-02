using MediaPack.Data;
using MediaPack.Dialogs;
using MediaPack.Helpers;
using MediaPack.Models.Channel.Enums;
using MediaPack.ViewModel.App;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace MediaPack.ViewModel.Channel
{
    public class ExportChannelsViewModel : WindowViewModel
    {
        public ExportChannelsViewModel(Window window) : base(window)
        {
            WindowMinimumHeight = 200;
            ChannelTypes = Enum.GetValues(typeof(ChannelType)).Cast<ChannelType>().ToObservableCollection();
            SelectedChannelType = ChannelType.Tv;

            ExportChannelsCommand = new RelayCommand(p => ExportChannels());
        }

        #region Commands

        public ICommand ExportChannelsCommand { get; set; }

        #endregion


        #region Public Properties

        public ObservableCollection<ChannelType> ChannelTypes { get; set; }
        public ChannelType SelectedChannelType { get; set; }

        #endregion


        #region Methods
        
        public void ExportChannels()
        {
            using var db = new AppDbContext();

            var channels = db.Channels.Where(x => x.ChannelType == SelectedChannelType).ToList();

            if (channels.Count > 0)
            {
                var exportChannelList = new List<Models.Common.Channel>();

                foreach (var channel in channels)
                {
                    exportChannelList.Add(new Models.Common.Channel 
                    { 
                        Id = channel.Id,
                        Country = channel.Country,
                        ChannelType = channel.ChannelType.ToString(),
                        Name = channel.Name,
                        Description = channel.Description,
                        M3U8Address = channel.M3U8Address
                    });
                }

                string jsonOutput = JsonConvert.SerializeObject(exportChannelList, Formatting.Indented);

                if (!string.IsNullOrEmpty(jsonOutput))
                {
                    var folderBrowserDialog = new FolderBrowserDialog
                    {
                        Description = "Klasör Seçiniz",
                        RootFolder = Environment.SpecialFolder.Desktop
                    };

                    var result = folderBrowserDialog.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
                    {
                        var date = $"{DateTime.Now:yyyy-dd-M-HH-mm-ss}";
                        var outputFileName = $"{SelectedChannelType}_Channels-{date}.json";
                        var savePath = $@"{folderBrowserDialog.SelectedPath}\{outputFileName}";

                        File.WriteAllText(savePath, jsonOutput);

                        if (File.Exists(savePath))
                        {
                            var dialog = new MessageDialog();

                            dialog.Closing += (s, e) =>
                            {
                                mWindow.Close();
                            };

                            dialog.ShowDialogWindow(new MessageDialogViewModel(dialog,
                                "Kanallar Aktarıldı",
                                $"{exportChannelList.Count} kanal dışarı ktarıldı. Dosya ismi: {outputFileName}"),
                                mWindow);
                        }
                    }
                }
            }
        }

        #endregion
    }
}