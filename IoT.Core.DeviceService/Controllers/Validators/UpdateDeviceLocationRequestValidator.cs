using FluentValidation;
using IoT.Core.DeviceService.Controllers.Dto;

namespace IoT.Core.DeviceService.Controllers.Validators
{
    public class UpdateDeviceLocationRequestValidator : AbstractValidator<UpdateDeviceLocationRequestDto>
    {
        public UpdateDeviceLocationRequestValidator()
        {
            RuleFor(x => x.DevEui)
                .NotEmpty().WithMessage("DevEui is required.")
                .WithMessage("DevEui cannot be null.");
            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.");
            RuleFor(x => x.Location.Country)
                .NotEmpty().WithMessage("Country is required.");

            RuleFor(x => x.Location.Province)
                .NotEmpty().When(x => !string.IsNullOrEmpty(x.Location.Country))
                .WithMessage("Province cannot be empty if Country is provided.");
            RuleFor(x => x.Location.District)
                .NotEmpty().When(x => !string.IsNullOrEmpty(x.Location.Country) &&
                                      !string.IsNullOrEmpty(x.Location.Province))
                .WithMessage("District cannot be empty if Country and Province are provided.");

        }

    }
}
