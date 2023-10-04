using WebAPI.Entities;

namespace WebAPI.DTOs
{
    public class TreeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IEnumerable<NodeDto>? Children { get; set; }
    }
}
