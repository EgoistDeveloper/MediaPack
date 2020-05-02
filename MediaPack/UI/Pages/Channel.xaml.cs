using MediaPack.ViewModel.Channel;

namespace MediaPack.UI.Pages
{
    /// <summary>
    /// Interaction logic for TvChannel.xaml
    /// </summary>
    public partial class Channel : BasePage<ChannelViewModel>
    {
        public Channel() : base()
        {
            InitializeComponent();
        }

        public Channel(ChannelViewModel vm) : base(vm)
        {
            InitializeComponent();
        }
    }
}