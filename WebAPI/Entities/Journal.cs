namespace WebAPI.Entities
{
    public class Journal
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; } = string.Empty;
        public int? EventId { get; set; }
        public Event? Event { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
