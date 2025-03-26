using FluentValidation;
using IoT.Core.AuthService.Controllers.Dto;

namespace IoT.Core.AuthService.Controllers.Validators
{
    public class UpdatePasswordRequestValidator : AbstractValidator<UpdatePasswordRequestDto>
    {
        public UpdatePasswordRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Old password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .NotEqual(x => x.OldPassword).WithMessage("New password must be different from the old password.");
        }
    }
}
