using System.Collections.ObjectModel;

namespace WebAPI.Entities
{
    public class Tree
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Node> Children { get; set; } = new Collection<Node>();
    }
}
