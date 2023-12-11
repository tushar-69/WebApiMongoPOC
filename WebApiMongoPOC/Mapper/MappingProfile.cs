using AutoMapper;
using WebApiMongoPOC.Models;
using WebApiMongoPOC.Models.DTOs;

namespace WebApiMongoPOC.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PlayList, PlayListDTO>();
        }
    }
}
