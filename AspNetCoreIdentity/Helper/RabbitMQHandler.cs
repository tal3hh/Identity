using AspNetCoreIdentity.Entities;
using AspNetCoreIdentity.Services.Interface;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Helper
{
    public class RabbitMQHandler 
    {
        private readonly RabbitMQHelper _rabbitMQHelper;
        private readonly IMessageSend _messageSend;

        public RabbitMQHandler(RabbitMQHelper rabbitMQHelper, IMessageSend messageSend)
        {
            _rabbitMQHelper = rabbitMQHelper;
            _messageSend = messageSend;
        }

        public void StartHandling()
        {
            _rabbitMQHelper.ConsumeSendEmail((username, email, url, token) =>
            {
                var user = new AppUser { UserName = username, Email = email };

                _messageSend.MimeKitConfrim(user, url, token);
            });
        }
    }
}
