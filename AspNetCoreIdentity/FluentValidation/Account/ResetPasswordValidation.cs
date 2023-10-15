using AspNetCoreIdentity.Models.Account;
using FluentValidation;

namespace AspNetCoreIdentity.FluentValidation.Account
{
    public class ResetPasswordValidation : AbstractValidator<ResetPasswordModel>
    {
        public ResetPasswordValidation()
        {
            RuleFor(x => x.Password).NotNull().WithMessage("Password daxil edin...").Length(3, 100).WithMessage("3-100 intervalinda simvol yazin.");

            RuleFor(x => x.ConfrimPassword).NotNull().WithMessage("ConfrimPassword daxil edin...")
                .Equal(x => x.Password).WithMessage("Tekrar sifre yalnisdir.");
        }
    }
}
