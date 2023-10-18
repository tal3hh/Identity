using AspNetCoreIdentity.Entities;

namespace AspNetCoreIdentity.Models.RabbitMQModel
{
    public class RegisterRabbitMQModel
    {
        //public string? Email { get; set; }
        //public string? Username { get; set; }
        public AppUser? AppUser { get; set; }
        public string? Token { get; set; }
        public string? Url { get; set; }
    }
}
