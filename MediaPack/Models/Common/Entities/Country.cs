using MediaPack.Models.Channel.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MediaPack.Models.Common.Entities
{
    public class Country : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }

        public string Name { get; set; }
        public string PhoneCode { get; set; }
        public string Alpha2code { get; set; }
        public string Alpha3code { get; set; }
        public BitmapImage Flag { get; set; }
    }
}
