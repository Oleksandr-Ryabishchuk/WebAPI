using AutoMapper;
using WebAPI.DTOs;
using WebAPI.Entities;

namespace WebAPI.Automapper
{
    public class JournalProfile: Profile
    {
        public JournalProfile()
        {
            CreateMap<Journal, JournalDto>();
        }
    }
}
