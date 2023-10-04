namespace WebAPI.DTOs
{
    public class JournalInfo
    {
        public int Skip { get; set; }
        public int Count { get; set; }
        public IEnumerable<JournalDto>? Items { get; set; }
    }
}
