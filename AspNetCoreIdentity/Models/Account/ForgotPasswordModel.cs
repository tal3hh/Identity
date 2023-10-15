using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Models.Account
{
    public class ForgotPasswordModel
    {
        [EmailAddress(ErrorMessage = "*E-mail formatinda('@') yazi daxil edin.")]
        public string? Email { get; set; }
    }
}
