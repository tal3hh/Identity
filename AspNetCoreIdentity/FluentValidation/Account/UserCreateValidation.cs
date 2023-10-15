using AspNetCoreIdentity.Models.Account;
using FluentValidation;

namespace AspNetCoreIdentity.FluentValidation.Account
{
    public class UserCreateValidation : AbstractValidator<UserCreateModel>
    {
        public UserCreateValidation()
        {
            RuleFor(x => x.Username).NotNull().WithMessage("Istifadeci adinizi yazin...").Length(3, 50).WithMessage("3-50 intervalinda simvol yazin.");

            RuleFor(x => x.Email).NotNull().WithMessage("Email yazin...").Length(3, 100).WithMessage("3-100 intervalinda simvol yazin.")
                .EmailAddress().WithMessage("Email formatinda('@') yazi daxil edin.");

            RuleFor(x => x.Password).NotNull().WithMessage("Password daxil edin...").Length(3, 100).WithMessage("3-100 intervalinda simvol yazin.");

            RuleFor(x => x.ConfrimPassword).NotNull().WithMessage("ConfrimPassword daxil edin...")
                .Equal(x => x.Password).WithMessage("Tekrar sifre yalnisdir.");
        }
    }
}
