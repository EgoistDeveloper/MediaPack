using System.Windows;
using System.Windows.Input;

namespace MediaPack.ViewModel.App
{
    public class DeleteDialogViewModel : WindowViewModel
    {
        public DeleteDialogViewModel(Window window, string title, string itemTitle) : base(window)
        {
            WindowMinimumHeight = 350;
            WindowMinimumWidth = 550;

            Title = title;
            ItemTitle = itemTitle;

            CloseCommand = new RelayCommand(p => CloseWindow());
            DeleteCommand = new RelayCommand((p) => Delete());
        }

        public string ItemTitle { get; set; }

        public bool Result { get; set; }

        private void CloseWindow()
        {
            mWindow.Close();
        }

        public ICommand DeleteCommand { get; set; }

        public void Delete()
        {
            Result = true;
            CloseWindow();
        }
    }
}