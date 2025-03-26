using FluentValidation;
using IoT.Core.ClientService.Controllers.Dto;

namespace IoT.Core.ClientService.Controllers.Validators
{
    public class UpdateClientEmailRequestValidator : AbstractValidator<UpdateClientEmailRequestDto>
    {
        public UpdateClientEmailRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
