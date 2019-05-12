using FluentValidation;

namespace ShoppingAreas.WebApi.ViewModels.Account
{
    public class VmCredentialsValidator : AbstractValidator<VmCredentials>
    {
        public VmCredentialsValidator()
        {
            RuleFor(vm => vm.UserName).NotEmpty().WithMessage("Username cannot be empty");
            RuleFor(vm => vm.Password).NotEmpty().WithMessage("Password cannot be empty");
            RuleFor(vm => vm.Password).Length(6, 18).WithMessage("Password must be between 6 and 18 characters");
        }
    }
}
