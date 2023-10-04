using AutoMapper;
using WebAPI.DTOs;
using WebAPI.Entities;

namespace WebAPI.Automapper
{
    public class NodeProfile: Profile
    {
        public NodeProfile()
        {
            CreateMap<Node, NodeDto>()
                .ReverseMap()
                .ForMember(x => x.TreeId, a => a.MapFrom(
                    (src, dest, destMember, context) => context.Items[nameof(Node.TreeId)]));
        }
    }
}
