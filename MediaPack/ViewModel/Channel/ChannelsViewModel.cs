using GalaSoft.MvvmLight;
using MediaPack.Data;
using MediaPack.Dialogs;
using MediaPack.Dialogs.Channel;
using MediaPack.Helpers;
using MediaPack.Models.Channel.Enums;
using MediaPack.Models.Common;
using MediaPack.ViewModel.App;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using static MediaPack.DI.DI;

namespace MediaPack.ViewModel.Channel
{
    public class ChannelsViewModel : ViewModelBase
    {
        public ChannelsViewModel()
        {
            TvChannels = new ObservableCollection<Models.Channel.Entities.Channel>();
            FavoriteTvChannels = new ObservableCollection<Models.Channel.Entities.Channel>();

            AddTvChannelCommand = new RelayCommand(p => AddTvChannel());
            ShowTvChannelCommand = new RelayParameterizedCommand(ShowTvChannel);
            EditTvChannelCommand = new RelayParameterizedCommand(EditTvChannel);
            DeleteTvChannelCommand = new RelayParameterizedCommand(DeleteTvChannel);

            GoToPageCommand = new RelayParameterizedCommand(GoToPage);
            SearchChangedCommand = new RelayParameterizedCommand(SearchChanged);
            PaginationChangedCommand = new RelayCommand(p => 
            {
                CurrentPage = 1;
                LoadTvChannels();
            });

            GoToPageFavoriteCommand = new RelayParameterizedCommand(GoToPageFavorite);
            SearchFavoriteChangedCommand = new RelayParameterizedCommand(SearchChangedFavorite);
            PaginationFavoriteChangedCommand = new RelayCommand(p =>
            {
                FavoriteCurrentPage = 1;
                LoadFavoriteTvChannels();
            });

            SetFavoriteChannelCommand = new RelayParameterizedCommand(SetFavoriteChannel);

            //LoadTvChannels();
            //LoadFavoriteTvChannels();
        }

        #region Commands
        public ICommand AddTvChannelCommand { get; set; }
        public ICommand ShowTvChannelCommand { get; set; }
        public ICommand EditTvChannelCommand { get; set; }
        public ICommand DeleteTvChannelCommand { get; set; }

        public ICommand GoToPageCommand { get; set; }
        public ICommand PaginationChangedCommand { get; set; }
        public ICommand SearchChangedCommand { get; set; }

        public ICommand GoToPageFavoriteCommand { get; set; }
        public ICommand PaginationFavoriteChangedCommand { get; set; }
        public ICommand SearchFavoriteChangedCommand { get; set; }

        public ICommand SetFavoriteChannelCommand { get; set; }

        #endregion


        #region Public Properties

        public ChannelType ChannelType { get; set; }
        public ObservableCollection<Models.Channel.Entities.Channel> TvChannels { get; set; }
        public ObservableCollection<Models.Channel.Entities.Channel> FavoriteTvChannels { get; set; }

        #endregion


        #region Pagination
        public Pagination Pagination { get; set; }
        public int PageLimit { get; set; } = 24;
        public int CurrentPage { get; set; } = 1;
        public string SearchTerm { get; set; }

        #endregion

        #region Pagination for Favorites

        public Pagination FavoritePagination { get; set; }
        public int FavoritePageLimit { get; set; } = 24;
        public int FavoriteCurrentPage { get; set; } = 1;
        public string FavoriteSearchTerm { get; set; }

        #endregion


        #region Methods

        public void AddTvChannel()
        {
            var dialog = new AddUpdateChannelDialog();

            dialog.Closing += (sender, args) =>
            {
                if (dialog.DataContext is AddUpdateChannelViewModel vm)
                {
                    if (vm.Channel != null && !TvChannels.Any(x => x == vm.Channel))
                    {
                        TvChannels.Insert(0, vm.Channel);
                    }
                    else
                    {
                        for (int i = 0; i < TvChannels.Count; i++)
                        {
                            if (TvChannels[i].Id == vm.Channel.Id)
                            {
                                TvChannels[i] = vm.Channel;
                                break;
                            }
                        }
                    }
                }
            };

            dialog.ShowDialogWindow(new AddUpdateChannelViewModel(dialog));
        }

        public void EditTvChannel(object sender)
        {
            var channel = (Models.Channel.Entities.Channel)((Button)sender).DataContext;
            var dialog = new AddUpdateChannelDialog();

            dialog.Closing += (sender, args) =>
            {
                if (dialog.DataContext is AddUpdateChannelViewModel vm)
                {
                    channel = vm.Channel;
                }
            };

            dialog.ShowDialogWindow(new AddUpdateChannelViewModel(dialog, channel));
        }

        public void DeleteTvChannel(object sender)
        {
            var channel = (Models.Channel.Entities.Channel)((Button)sender).DataContext;

            var dialog = new DeleteDialog();

            dialog.Closing += (send, args) =>
            {
                if (dialog.DataContext is DeleteDialogViewModel vm && vm.Result)
                {
                    using var db = new AppDbContext();

                    if (channel.Logo != null)
                    {
                        var fileName = channel.Logo.UriSource.OriginalString;
                        File.Delete(fileName);
                    }

                    db.Channels.Remove(channel);
                    db.SaveChanges();

                    TvChannels.Remove(channel);

                    if (FavoriteTvChannels.Any(x => x.Id == channel.Id))
                    {
                        FavoriteTvChannels.Remove(channel);
                    }

                    Pagination.TotalItems -= 1;
                    Pagination.PageSize -= 1;
                }
            };

            dialog.ShowDialogWindow(new DeleteDialogViewModel(dialog, "Kanalı Sil", channel.Name));
        }

        public void LoadTvChannels()
        {
            using var db = new AppDbContext();

            var totalSize = db.Channels
            .Where(x => EF.Functions.Like(x.Name, $"%{SearchTerm}%"))
            .Where(x => x.ChannelType == ChannelType)
            .Count();
            totalSize = totalSize > 0 ? totalSize : 1;

            Pagination = new Pagination(totalSize, CurrentPage, PageLimit, 10);

            TvChannels = db.Channels
            .Where(x => EF.Functions.Like(x.Name, $"%{SearchTerm}%"))
            .Where(x => x.ChannelType == ChannelType)
            .Select(x => new Models.Channel.Entities.Channel 
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                M3U8Address = x.M3U8Address,
                IsFavorite = x.IsFavorite,
                Country = x.Country,
                ChannelType = x.ChannelType,
                Logo = File.Exists(@$"{Settings.CurrentDirectory}\{x.Logo}") ? 
                       @$"{Settings.CurrentDirectory}\{x.Logo}".PathToBitmapImage() : null
            })
            .OrderBy(x => x.Name)
            .Skip((CurrentPage - 1) * PageLimit)
            .Take(PageLimit)
            .ToObservableCollection();
        }

        public void LoadFavoriteTvChannels()
        {
            using var db = new AppDbContext();

            var totalSize = db.Channels
            .Where(x => EF.Functions.Like(x.Name, $"%{FavoriteSearchTerm}%"))
            .Where(x => x.ChannelType == ChannelType)
            .Where(x => x.IsFavorite)
            .Count();
            totalSize = totalSize > 0 ? totalSize : 1;

            FavoritePagination = new Pagination(totalSize, FavoriteCurrentPage, FavoritePageLimit, 10);

            FavoriteTvChannels = db.Channels
            .Where(x => EF.Functions.Like(x.Name, $"%{FavoriteSearchTerm}%"))
            .Where(x => x.ChannelType == ChannelType)
            .Where(x => x.IsFavorite)
            .Select(x => new Models.Channel.Entities.Channel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                M3U8Address = x.M3U8Address,
                IsFavorite = x.IsFavorite,
                Country = x.Country,
                ChannelType = x.ChannelType,
                Logo = File.Exists(@$"{Settings.CurrentDirectory}\{x.Logo}") ?
                       @$"{Settings.CurrentDirectory}\{x.Logo}".PathToBitmapImage() : null
            })
            .OrderBy(x => x.Name)
            .Skip((FavoriteCurrentPage - 1) * FavoritePageLimit)
            .Take(FavoritePageLimit)
            .ToObservableCollection();
        }

        public void ShowTvChannel(object sender)
        {
            if (sender == null && !(sender is Button)) return;
            var button = sender as Button;

            if (!(button.DataContext is Models.Channel.Entities.Channel channel)) return;

            ViewModelApplication.CurrentChannel = channel;

            ViewModelApplication.GoToPage(ApplicationPage.TvChannel);
        }

        public void GoToPage(object sender)
        {
            var page = (Models.Common.Page)(sender as Button).DataContext;

            CurrentPage = page.PageNumber;
            LoadTvChannels();
        }

        public void SearchChanged(object sender)
        {
            SearchTerm = (sender as TextBox).Text;
            CurrentPage = 1;
            LoadTvChannels();
        }

        public void GoToPageFavorite(object sender)
        {
            var page = (Models.Common.Page)(sender as Button).DataContext;

            FavoriteCurrentPage = page.PageNumber;
            LoadFavoriteTvChannels();
        }

        public void SearchChangedFavorite(object sender)
        {
            FavoriteSearchTerm = (sender as TextBox).Text;
            FavoriteCurrentPage = 1;
            LoadFavoriteTvChannels();
        }

        public void SetFavoriteChannel(object sender)
        {
            var channel = ((sender as CheckBox).DataContext as Models.Channel.Entities.Channel);

            using var db = new AppDbContext();
            db.Channels.Update(channel);
            db.SaveChanges();

            if (channel.IsFavorite && !FavoriteTvChannels.Any(x => x.Id == channel.Id))
            {
                FavoriteTvChannels.Insert(0, channel);
            }
            else if (!channel.IsFavorite && FavoriteTvChannels.Any(x => x.Id == channel.Id))
            {
                FavoriteTvChannels.Remove(channel);
            }
        }

        #endregion
    }
}