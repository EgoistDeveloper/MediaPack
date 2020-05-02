using GalaSoft.MvvmLight;
using MediaPack.Data;
using MediaPack.Models.Common;
using MediaPack.ViewModel.Base;
using System.Windows.Forms.Integration;
using Unosquare.FFME;

namespace MediaPack.ViewModel.App
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        public ApplicationViewModel()
        {
            using var db = new AppDbContext();

            //AppSettings = new AppSettings();
        }

        #region Properties

        public Models.Channel.Entities.Channel CurrentChannel { get; set; }
        public ApplicationPage CurrentPage { get; set; }

        public ViewModelBase CurrentPageViewModel { get; set; }

        //public AppSettings AppSettings { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Navigates to the specified page
        /// </summary>
        /// <param name="page">The page to go to</param>
        /// <param name="viewModel">The view model, if any, to set explicitly to the new page</param>
        public void GoToPage(ApplicationPage page, ViewModelBase viewModel = null)
        {
            CurrentPageViewModel = viewModel;

            var different = CurrentPage != page;

            CurrentPage = page;

            if (!different)
                OnPropertyChanged(nameof(CurrentPage));
        }

        #endregion
    }
}