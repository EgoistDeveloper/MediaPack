using MediaPack.Models.Channel.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media.Imaging;

namespace MediaPack.Models.Channel.Entities
{
    public class Channel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        public string Country { get; set; }
        public long CategoryId { get; set; }
        [Required]
        public ChannelType ChannelType { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsFavorite { get; set; }
        public string Description { get; set; }
        public string M3U8Address { get; set; }
        public BitmapImage Logo { get; set; }
    }
}