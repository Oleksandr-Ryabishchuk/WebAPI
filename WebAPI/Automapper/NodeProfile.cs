using AutoMapper;
using WebAPI.DTOs;
using WebAPI.Entities;

namespace WebAPI.Automapper
{
    public class NodeProfile: Profile
    {
        public NodeProfile()
        {          
            CreateMap<Node, NodeCreateDto>()
                .ForMember(x => x.ParentId, y => y.MapFrom(a => a.ParentId))
                .ReverseMap()
                .ForMember(x => x.TreeId, a => a.MapFrom(
                    (src, dest, destMember, context) => context.Items[nameof(Node.TreeId)]))
                .ForMember(x => x.Tree, a => a.MapFrom(
                    (src, dest, destMember, context) => context.Items[nameof(Node.Tree)]));
        }
    }
}
