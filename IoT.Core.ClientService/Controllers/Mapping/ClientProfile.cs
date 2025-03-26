using AutoMapper;
using IoT.Core.ClientService.Controllers.Dto;
using IoT.Core.ClientService.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IoT.Core.ClientService.Controllers.Mapping
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<Client, ClientResponseDto>();
        }
    }
}
