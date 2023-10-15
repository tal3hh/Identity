using AspNetCoreIdentity.Entities;

namespace AspNetCoreIdentity.Services.Interface
{
    public interface IMessageSend
    {
        void MimeKitConfrim(AppUser appUser, string url, string token);
        void MimeMessageResetPassword(AppUser user, string url, string code);
    }
}
