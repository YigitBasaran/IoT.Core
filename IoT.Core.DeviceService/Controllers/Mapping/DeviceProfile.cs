using AutoMapper;
using IoT.Core.DeviceService.Controllers.Dto;
using IoT.Core.DeviceService.Model;

namespace IoT.Core.DeviceService.Controllers.Mapping
{
    public class DeviceProfile : Profile
    {
        public DeviceProfile()
        {
            CreateMap<Device, DeviceResponseDto>()
                .ConvertUsing(src => new DeviceResponseDto(
                    src.Id,
                    src.Name,
                    src.ClientId,
                    new Location(src.Location.Country, src.Location.Province, src.Location.District)
                ));
        }
    }
}
