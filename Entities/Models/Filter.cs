namespace Entities
{
    public class Filter
    {
        public string? CarNumber { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Material { get; set; }
        public string? Shipper { get; set; }
        public string? Contragent { get; set; }
    }
}