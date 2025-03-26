using FluentValidation;
using IoT.Core.AuthService.Controllers.Dto;

namespace IoT.Core.AuthService.Controllers.Validators
{
    public class SetPasswordRequestValidator : AbstractValidator<SetPasswordRequestDto>
    {
        public SetPasswordRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        }
    }
}
