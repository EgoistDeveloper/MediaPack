using System.ComponentModel;
using System.Windows.Input;

namespace MediaPack.Models.Channel
{
    public class ChannelItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand EditTvChannelCommand { get; set; }
        public ICommand DeleteTvChannelCommand { get; set; }

        public Entities.Channel Channel { get; set; }
    }
}
