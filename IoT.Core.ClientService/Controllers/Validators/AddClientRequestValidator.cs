using FluentValidation;
using IoT.Core.ClientService.Controllers.Dto;

namespace IoT.Core.ClientService.Controllers.Validators
{
    public class AddClientRequestValidator : AbstractValidator<AddClientRequestDto>
    {
        public AddClientRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
