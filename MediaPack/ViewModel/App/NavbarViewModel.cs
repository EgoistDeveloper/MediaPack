using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using MediaPack.Models.Common;
using static MediaPack.DI.DI;

namespace MediaPack.ViewModel.App
{
    public class NavbarViewModel : ViewModelBase
    {
        public NavbarViewModel()
        {
            NavbarItems = new ObservableCollection<NavbarItem>()
            {
                new NavbarItem()
                {
                    ApplicationPage = ApplicationPage.Tv,
                    IconData = (System.Windows.Application.Current.FindResource("Monitor") as Path)?.Data,
                    IsChecked = true,
                    Title = "Televizyon"
                },
                new NavbarItem()
                {
                    ApplicationPage = ApplicationPage.Radio,
                    IconData = (System.Windows.Application.Current.FindResource("Radio") as Path)?.Data,
                    IsChecked = false,
                    Title = "Radyo"
                },
                //new NavbarItem()
                //{
                //    ApplicationPage = ApplicationPage.Settings,
                //    IconData = (System.Windows.Application.Current.FindResource("SettingsOutline") as Path)?.Data,
                //    IsChecked = false,
                //    Title = "Ayarlar"
                //}
            };

            GoToCommand = new RelayParameterizedCommand(GoTo);
            GoToSettingsCommand = new RelayCommand(p => GoToSettings());
        }

        public ObservableCollection<NavbarItem> NavbarItems { get; set; }

        public ICommand GoToCommand { get; set; }
        public ICommand GoToSettingsCommand { get; set; }

        public void GoTo(object sender)
        {
            if (sender == null || !(sender is ToggleButton toggleButtonbutton)) return;

            if (!(toggleButtonbutton.DataContext is NavbarItem navbarItem)) return;

            foreach (var item in NavbarItems)
            {
                item.IsChecked = false;
            }

            navbarItem.IsChecked = true;

            if (ViewModelApplication.CurrentPage != navbarItem.ApplicationPage)
            {
                ViewModelApplication.GoToPage(navbarItem.ApplicationPage);
            }
        }

        public void GoToSettings()
        {
            ViewModelApplication.GoToPage(ApplicationPage.Settings);
        }
    }
}
