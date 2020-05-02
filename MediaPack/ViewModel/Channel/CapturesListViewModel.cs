using MediaPack.Data;
using MediaPack.Dialogs;
using MediaPack.Helpers;
using MediaPack.Models.Channel.Entities;
using MediaPack.ViewModel.App;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPack.ViewModel.Channel
{
    public class CapturesListViewModel : WindowViewModel
    {
        public CapturesListViewModel(Window window, Models.Channel.Entities.Channel channel) : base(window)
        {
            mWindow = window;
            WindowMinimumHeight = 850;
            WindowMinimumWidth = 800;

            Title = $"{channel.Name} Kanalına Ait Kayıtlar";
            Channel = channel;

            Captures = new ObservableCollection<Capture>();

            GoToPageCommand = new RelayParameterizedCommand(GoToPage);
            SearchChangedCommand = new RelayParameterizedCommand(SearchChanged);
            OpenAndSelectCaptureCommand = new RelayParameterizedCommand(OpenAndSelectCapture);
            DeleteCaptureCommand = new RelayParameterizedCommand(DeleteCapture);

            LoadCaptures();
        }

        #region Commands

        public ICommand AddLogoCommand { get; set; }
        public ICommand AddCategoryCommand { get; set; }
        public ICommand OpenAndSelectCaptureCommand { get; set; }
        public ICommand DeleteCaptureCommand { get; set; }

        public ICommand GoToPageCommand { get; set; }
        public ICommand SearchChangedCommand { get; set; }

        #endregion


        #region Public Properties

        public Models.Channel.Entities.Channel Channel { get; set; }
        public ObservableCollection<Capture> Captures { get; set; }

        #endregion


        #region Pagination
        public Pagination Pagination { get; set; }
        public int PageLimit { get; set; } = 20;
        public int CurrentPage { get; set; } = 1;
        public string SearchTerm { get; set; }

        #endregion


        #region Methods

        public void LoadCaptures()
        {
            using var db = new AppDbContext();

            var totalSize = db.Captures
            .Where(x => EF.Functions.Like(x.FileName, $"%{SearchTerm}%"))
            .Where(x => x.ChannelId == Channel.Id)
            .Count();
            totalSize = totalSize > 0 ? totalSize : 1;

            Pagination = new Pagination(totalSize, CurrentPage, PageLimit, 10);

            Captures = db.Captures
            .Where(x => EF.Functions.Like(x.FileName, $"%{SearchTerm}%"))
            .Where(x => x.ChannelId == Channel.Id)
            .OrderBy(x => x.CaptureDate)
            .Skip((CurrentPage - 1) * PageLimit)
            .Take(PageLimit)
            .ToObservableCollection();
        }

        public void OpenAndSelectCapture(object sender)
        {
            var capture = (Capture)((Button)sender).DataContext;
            var fullPath = $@"{Settings.CurrentDirectory}\{capture.FileName}";

            if (!File.Exists(fullPath))
            {
                var dialog = new MessageDialog();
                dialog.ShowDialogWindow(new MessageDialogViewModel(dialog, 
                    "Dosya Yok!", 
                    "Böyle bir dosya bulunamadı."),
                    mWindow);
            }
            else
            {
                Process.Start("explorer.exe", $"/select, \"{fullPath}\"");
            }
        }

        public void GoToPage(object sender)
        {
            var page = (Models.Common.Page)(sender as Button).DataContext;

            CurrentPage = page.PageNumber;
            LoadCaptures();
        }

        public void SearchChanged(object sender)
        {
            SearchTerm = (sender as TextBox).Text;
            CurrentPage = 1;
            LoadCaptures();
        }

        public void DeleteCapture(object sender)
        {
            var capture = (Capture)((Button)sender).DataContext;
            var thumbFile = $@"{Settings.CurrentDirectory}\{capture.Thumbnail}";
            var captureFile = $@"{Settings.CurrentDirectory}\{capture.FileName}";

            if (File.Exists(thumbFile))
            {
                File.Delete(thumbFile);
            }

            if (File.Exists(captureFile))
            {
                File.Delete(captureFile);
            }

            using var db = new AppDbContext();

            db.Captures.Remove(capture);
            db.SaveChanges();

            Captures.Remove(capture);

            Pagination.TotalItems -= 1;
            Pagination.PageSize -= 1;
        }

        #endregion
    }
}
