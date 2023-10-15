using AspNetCoreIdentity.Models.Account;
using FluentValidation;

namespace AspNetCoreIdentity.FluentValidation.Account
{
    public class UserLoginValidation : AbstractValidator<UserLoginModel>
    {
        public UserLoginValidation()
        {
            RuleFor(x => x.UsernameorEmail).NotNull().WithMessage("UsernameorEmail yazin...").Length(3, 100).WithMessage("3-100 intervalinda simvol yazin.");

            RuleFor(x => x.Password).NotNull().WithMessage("Password daxil edin...").Length(3, 100).WithMessage("3-100 intervalinda simvol yazin.");
        }
    }
}
