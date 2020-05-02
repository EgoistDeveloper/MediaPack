using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MediaPack.Models.Common.Entities
{
    public class AppSetting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        [Required]
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        [Required]
        public bool IsEditable { get; set; }
        public string DefaultValue { get; set; }
    }
}