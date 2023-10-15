namespace AspNetCoreIdentity.Models.Account
{
    public class UserLoginModel
    {
        public string? UsernameorEmail { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
