using WebAPI.DTOs;
using WebAPI.Entities;

namespace WebAPI.Services.Interfaces
{
    public interface INodeService
    {
        ValidityResponse CheckIfValid(Tree tree, NodeCreateDto dto);
    }
}
