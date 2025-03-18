using AutoMapper;
using IoT.Core.DeviceService.Controllers.Dto;

namespace IoT.Core.DeviceService.Controllers.Mapping;

public class DeviceProfile : Profile
{
    public DeviceProfile()
    {
        CreateMap<AddDeviceRequestDto, Model.Device>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<UpdateDeviceRequestDto, Model.Device>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}