using AutoMapper;
using IoT.Core.DataService.Controllers.Dto;
using IoT.Core.DataService.Model;

namespace IoT.Core.DataService.Controllers.Mapping
{
    public class DataProfile : Profile
    {
        public DataProfile()
        {
            CreateMap<Data, DataResponseDto>();
        }
    }
}
