using AutoMapper;
using WebAPI.DTOs;
using WebAPI.Entities;

namespace WebAPI.Automapper
{
    public class TreeProfile: Profile
    {
        public TreeProfile() 
        {
            CreateMap<Tree, TreeDto>()
                .ForMember(x => x.Children, x => x.MapFrom(a => a.Children.OrderBy(q => q.Id)));
        }
    }
}
