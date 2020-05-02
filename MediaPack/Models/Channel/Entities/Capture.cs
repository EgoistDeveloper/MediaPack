using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media.Imaging;

namespace MediaPack.Models.Channel.Entities
{
    public class Capture : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        [Required]
        public long ChannelId { get; set; }
        [Required]
        public string FileName { get; set; }
        public BitmapImage Thumbnail { get; set; }
        public long FileSize { get; set; }
        public DateTime CaptureDate { get; set; }
    }
}