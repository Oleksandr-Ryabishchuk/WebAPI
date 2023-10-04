namespace WebAPI.Entities
{
    public class Node
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public int? TreeId { get; set; }
        public Tree? Tree { get; set; }
        
    }
}
