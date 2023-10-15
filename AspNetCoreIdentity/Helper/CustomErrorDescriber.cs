﻿using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Helper
{
    public class CustomErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError()
            {
                Code = "PasswordRequiresLower",
                Description = "*Şifre'de kicik herf olmalidir."
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError()
            {
                Code = "PasswordRequiresUpper",
                Description = "*Şifre'de boyuk herf olmalidir."
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError()
            {
                Code = "DuplicateUserName",
                Description = $"*'{userName}' adli istifadeci artiq movcuddur.(Yeniden istifade edile bilmez.!)"
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError()
            {
                Code = "PasswordTooShort",
                Description = $"*Şifre min. {length} simvol ola biler."
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError()
            {
                Code = "DuplicateEmail",
                Description = $"*'{email}' bu e-mail artiq movcuddur.(Yeniden istifade edile bilmez.!)"
            };
        }
    }
}
