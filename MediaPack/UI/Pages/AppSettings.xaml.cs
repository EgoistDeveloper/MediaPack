using MediaPack.ViewModel.App;

namespace MediaPack.UI.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class AppSettings : BasePage<AppSettingsViewModel>
    {
        public AppSettings()
        {
            InitializeComponent();
        }

        public AppSettings(AppSettingsViewModel vm) : base(vm)
        {
            InitializeComponent();
        }
    }
}