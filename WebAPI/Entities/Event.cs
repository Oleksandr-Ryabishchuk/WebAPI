namespace WebAPI.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
