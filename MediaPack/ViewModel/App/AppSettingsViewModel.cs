using GalaSoft.MvvmLight;
using MediaPack.Data;
using MediaPack.Dialogs.Channel;
using MediaPack.Helpers;
using MediaPack.Models.Common.Entities;
using MediaPack.ViewModel.Channel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using static MediaPack.DI.DI;

namespace MediaPack.ViewModel.App
{
    public class AppSettingsViewModel : ViewModelBase
    {
        public AppSettingsViewModel()
        {
            AppSettings = new ObservableCollection<AppSetting>();

            SaveSettingCommand = new RelayParameterizedCommand(SaveSetting);

            ShowImportChannelCommand = new RelayCommand(p => ShowImportChannels());
            ShowExportChannelCommand = new RelayCommand(p => ShowExportChannel());

            LoadAppSettings();
        }

        #region Commands

        public ICommand SaveSettingCommand { get; set; }
        public ICommand ShowImportChannelCommand { get; set; }
        public ICommand ShowExportChannelCommand { get; set; }

        #endregion


        #region Public Properties

        public ObservableCollection<AppSetting> AppSettings { get; set; }

        #endregion


        #region Methods

        public void LoadAppSettings()
        {
            using var db = new AppDbContext();

            AppSettings = db.AppSettings
            .OrderBy(x => x.SettingValue)
            .ToObservableCollection();
        }

        public void SaveSetting(object sender)
        {
            var appSetting = (sender as TextBox).DataContext as AppSetting;

            using var db = new AppDbContext();

            db.AppSettings.Update(appSetting);
            db.SaveChanges();
        }

        public void ShowImportChannels()
        {
            var dialog = new InsertChannelsDialog();
            dialog.ShowDialogWindow(new ImportChannelsViewModel(dialog));
        }

        public void ShowExportChannel()
        {
            var dialog = new ExportChannelsDialog();
            dialog.ShowDialogWindow(new ExportChannelsViewModel(dialog));
        }

        #endregion
    }
}