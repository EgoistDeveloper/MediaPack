namespace MediaPack.Models.Common
{
    public class Channel
    {
        public long Id { get; set; }
        public string Country { get; set; }
        public string ChannelType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string M3U8Address { get; set; }
    }
}