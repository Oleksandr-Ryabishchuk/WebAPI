using System.ComponentModel;

namespace WebAPI.DTOs
{
    public class NodeCreateDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [DefaultValue(null)]
        public int? ParentId { get; set; }
        public string TreeName { get; set; } = string.Empty;
    }
}
