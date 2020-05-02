using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MediaPack.Models.Channel.Entities
{
    public class SpendTime : INotifyPropertyChanged
    {
        public SpendTime()
        {
            AddedDate = DateTime.Now;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        [Required]
        public long ChannelId { get; set; }
        [Required]
        public DateTime AddedDate { get; set; }
        [Required]
        public TimeSpan Spendtime { get; set; }
    }
}