using FluentValidation;
using IoT.Core.DeviceService.Controllers.Dto;

namespace IoT.Core.DeviceService.Controllers.Validators;

public class AddDeviceRequestValidator : AbstractValidator<AddDeviceRequestDto>
{
    public AddDeviceRequestValidator()
    {
        RuleFor(x => x.DevEUI)
            .NotEmpty().WithMessage("DevEUI is required.")
            .Matches("^[0-9A-Fa-f]{16}$").WithMessage("DevEUI must be a 16-character hexadecimal string.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Device name is required.")
            .MinimumLength(3).WithMessage("Device name must be at least 3 characters long.");

        RuleFor(x => x.CustomerId)
            .GreaterThan(-1).WithMessage("CustomerId cannot be negative.");

        RuleFor(x => x.Location)
            .NotNull().WithMessage("Location is required.");
    }
}
