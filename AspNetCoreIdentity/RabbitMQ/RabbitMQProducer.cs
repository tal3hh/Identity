using AspNetCoreIdentity.RabbitMQ.Interface;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace AspNetCoreIdentity.RabbitMQ
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private readonly IConfiguration _configuration;

        public RabbitMQProducer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfrimEmail<T>(T message)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_configuration["RabbitMQ:Uri"])
            };

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "register", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var json = JsonConvert.SerializeObject(message);
            var byteBody = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "", routingKey: "register", body: byteBody);
        }
    }
}
