using Microsoft.Win32;
using MediaPack.Data;
using MediaPack.Dialogs.Channel;
using MediaPack.Helpers;
using MediaPack.Models.Channel.Entities;
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
using MediaPack.Models.Common.Entities;
using MediaPack.Models.Channel.Enums;

namespace MediaPack.ViewModel.Channel
{
    public class AddUpdateChannelViewModel : WindowViewModel
    {
        public AddUpdateChannelViewModel(Window window, Models.Channel.Entities.Channel channel = null) : base(window)
        {
            mWindow = window;
            WindowMinimumHeight = 650;
            WindowMinimumWidth = 800;

            Title = channel != null ? $"Kanal Güncelle: {channel.Name}" : "Kanal Ekle";
            Channel = channel ?? new Models.Channel.Entities.Channel();

            ChannelTypes = Enum.GetValues(typeof(Models.Channel.Enums.ChannelType)).Cast<Models.Channel.Enums.ChannelType>().ToObservableCollection();


            CloseCommand = new RelayCommand(p =>
            {
                AddOrUpdate();

                mWindow.Close();
            });

            AddLogoCommand = new RelayCommand(p => AddLogo());
            AddCategoryCommand = new RelayCommand(p => AddCategory());

            Countries = new ObservableCollection<Country>();
            Categories = new ObservableCollection<Category>();
            ChannelTypes = Enum.GetValues(typeof(ChannelType)).Cast<ChannelType>().ToObservableCollection();

            LoadCountries();
            LoadCategories();
        }

        public Models.Channel.Entities.Channel Channel { get; set; }
        public ObservableCollection<Country> Countries { get; set; }
        public Country SelectedCountry { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
        public Category SelectedCategory { get; set; }
        public ObservableCollection<ChannelType> ChannelTypes { get; set; }

        public ICommand AddLogoCommand { get; set; }
        public ICommand AddCategoryCommand { get; set; }

        public void AddLogo()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Kanal İçin Logo Seçin",
                Filter = Settings.ImageFilter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (openFileDialog?.ShowDialog() == true)
            {
                Channel.Logo = openFileDialog.FileName.PathToBitmapImage();
            }
        }

        public void LoadCountries()
        {
            using var db = new AppDbContext();

            Countries = db.Countries.ToObservableCollection();

            SelectedCountry = Countries.FirstOrDefault(x => x.Alpha3code == Channel.Country);
        }

        public void LoadCategories()
        {
            using var db = new AppDbContext();

            Categories = db.Categories.OrderBy(x => x.Title).ToObservableCollection();

            Categories.Insert(0, new Category
            {
                Id = 0,
                Title = "Kategori Seçiniz"
            });
            SelectedCategory = Categories.First();
        }

        public void AddCategory()
        {
            var dialog = new AddUpdateCategoryDialog();

            dialog.Closing += (sender, args) =>
            {
                if (dialog.DataContext is AddUpdateCategoryViewModel vm)
                {
                    if (vm.Category != null && !Categories.Any(x => x == vm.Category))
                    {
                        Categories.Insert(0, vm.Category);
                    }
                    else
                    {
                        for (int i = 0; i < Categories.Count; i++)
                        {
                            if (Categories[i].Id == vm.Category.Id)
                            {
                                Categories[i] = vm.Category;
                                break;
                            }
                        }
                    }
                }
            };

            dialog.ShowDialogWindow(new AddUpdateCategoryViewModel(dialog), mWindow);
        }

        public void AddOrUpdate()
        {
            if (Channel != null && Channel != new Models.Channel.Entities.Channel())
            {
                if (SelectedCountry != null)
                {
                    Channel.Country = SelectedCountry.Alpha3code;
                }

                if (SelectedCategory != null && SelectedCategory.Id > 0)
                {
                    Channel.CategoryId = SelectedCategory.Id;
                }

                if (Channel.Logo != null)
                {
                    var fileName = @$"{Settings.CurrentDirectory}\Resources\Tv-Channels\Logos\{Channel.Id}.png";

                    if (fileName != Channel.Logo.UriSource.OriginalString)
                    {
                        Channel.Logo.UriSource.OriginalString.PathToBitmapImage().SaveImage(fileName);
                        Channel.Logo = @$"Resources\Channels\Logos\{Channel.Id}.png".PathToBitmapImage();
                    }
                }

                using var db = new AppDbContext();

                var isExists = db.Channels.Any(x => x.Name == Channel.Name);

                if (Channel.Id > 0 && isExists)
                {
                    db.Channels.Update(Channel);
                }
                else if (Channel.Id < 1 && !isExists)
                {
                    db.Channels.Add(Channel);
                }

                db.SaveChanges();
            }
        }
    }
}