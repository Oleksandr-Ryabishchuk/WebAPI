namespace WebAPI.DTOs
{
    public class NodeDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentId;
    }
}
