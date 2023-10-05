using WebAPI.DTOs;
using WebAPI.Entities;
using WebAPI.Exceptions;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services
{
    public class NodeService : INodeService
    {
        public ValidityResponse CheckIfValid(Tree tree, NodeCreateDto dto)
        {
            if(tree.Children.Count == 0) 
            {
                return new ValidityResponse(true, "");
            }
            var parent = tree.Children.FirstOrDefault(a => a.ParentId == null);
            if (parent == null)
            {                
                return new ValidityResponse(false, $"Tree with Name={tree.Name} has been built in wrong way");
            }
            var supposedParent = tree.Children.FirstOrDefault(a => a.Id == dto.ParentId);
            if (supposedParent == null)
            {
                return new ValidityResponse(false, $"Node with Id={dto.ParentId} does not exist");
            }
            var child = tree.Children.FirstOrDefault(b => b.ParentId == parent.Id);
            if (child != null)
            {
                return new ValidityResponse(false, $"Node with Id={child.ParentId} already has a child");
            }
            if(tree.Children.FirstOrDefault(x => x.Name == dto.Name) != null)
            {
                return new ValidityResponse(false, $"Node with Name={dto.Name} already exists in current tree");
            }

            return new ValidityResponse(true, "");
        }
    }
}
