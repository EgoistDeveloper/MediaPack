using MediaPack.ViewModel.Channel;

namespace MediaPack.UI.Pages
{
    /// <summary>
    /// Interaction logic for Tv.xaml
    /// </summary>
    public partial class Channels : BasePage<ChannelsViewModel>
    {
        public Channels() : base()
        {
            InitializeComponent();
        }

        public Channels(ChannelsViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }
    }
}