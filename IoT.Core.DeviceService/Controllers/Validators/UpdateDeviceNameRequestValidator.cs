using FluentValidation;
using IoT.Core.DeviceService.Controllers.Dto;

namespace IoT.Core.DeviceService.Controllers.Validators;

public class UpdateDeviceNameRequestValidator : AbstractValidator<UpdateDeviceNameRequestDto>
{
    public UpdateDeviceNameRequestValidator()
    {
        RuleFor(x => x.DevEui)
            .NotEmpty().WithMessage("DevEui is required.")
            .WithMessage("DevEui cannot be null.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Device name is required.")
            .MinimumLength(3).WithMessage("Device name must be at least 3 characters long.");
    }
}