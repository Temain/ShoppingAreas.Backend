using FluentValidation;

namespace ShoppingAreas.WebApi.ViewModels.Account
{
    public class VmAreaValidator : AbstractValidator<VmArea>
    {
        public VmAreaValidator()
        {
            RuleFor(vm => vm.Name).NotEmpty().WithMessage("Наименование не может быть пустым.");
			RuleFor(vm => vm.Address).NotEmpty().WithMessage("Адрес не может быть пустым.");
		}
    }
}
