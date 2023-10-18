using AspNetCoreIdentity.Models.RabbitMQModel;
using AspNetCoreIdentity.RabbitMQ.Interface;
using AspNetCoreIdentity.Services.Interface;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace AspNetCoreIdentity.RabbitMQ
{
    public class RabbitMQConsume : IRabbitMQConsume
    {
        private readonly IMessageSend _message;
        private readonly IConfiguration _configuration;

        public RabbitMQConsume(IMessageSend message, IConfiguration configuration)
        {
            _message = message;
            _configuration = configuration;
        }

        public void ConsumeConfrimEmail()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_configuration["RabbitMQ:Uri"])
            };

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "register", durable: false, exclusive: false, autoDelete: false, arguments: null); //Yoxdusa yaradacaq

            var consume = new EventingBasicConsumer(channel);
            consume.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var registermodel = JsonConvert.DeserializeObject<RegisterRabbitMQModel>(message);

                if(registermodel !=null)
                    _message.MimeKitConfrim(registermodel.AppUser, registermodel.Url, registermodel.Token);
            };

            channel.BasicConsume(queue: "register", consumer: consume);
        }
    }
}
