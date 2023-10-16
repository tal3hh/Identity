using AspNetCoreIdentity.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Security.AccessControl;
using System.Text;
using IModel = RabbitMQ.Client.IModel;

namespace AspNetCoreIdentity.Helper
{
    public class RabbitMQHelper
    {
        private readonly ConnectionFactory _factory;
        private readonly IModel _channel;

        public RabbitMQHelper(ConnectionFactory factory, IModel channel)
        {
            _factory = factory;
            _channel = channel;
        }


        public RabbitMQHelper()
        {
            _factory = new ConnectionFactory();
            _factory.Uri = new Uri("amqps://gnvjdvrj:RxnyO1UOCNBzbbEqpo9YzNlsr_Md3tSI@octopus.rmq3.cloudamqp.com/gnvjdvrj");
            var connection = _factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "send_email", durable: false, exclusive: false, autoDelete: false, arguments: null);

            //durable: Eger true verilse, queue(quyruq) mesajlarini itirmez ve yeniden acildiginda mesajlari qoruyar. Eger false verilse quyruq baglandiqdan sonra mesajlar siliner.

            //exclusive: Eger true verilse quyruq sadece baglanti yaradilan terefden istifade edile biler. Eger false verilse, quyruq paylasila biler ve diger baglantilar terefinden istifade edile biler.

            //autoDelete: Quyrugun avtomatik silinib, silinmiyeceyini bildirir. Eger true verilse, quyruq icinde mesaj qalmadiqda quyruq avtomatik silinir.
        }

        public void SendEmailRequest(string username, string email, string url, string token)
        {
            string message = $"{username}|{email}|{url}|{token}";
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "send_email", basicProperties: null, body: body);
        }



        public void ConsumeSendEmail(Action<string, string, string, string> onRecived)
        {
            var consume = new EventingBasicConsumer(_channel);
            consume.Received += (model, ea) =>
            {
                //Request melumatlari
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                string[] parts = message.Split("|");
                string username = parts[0];
                string email = parts[1];
                string url = parts[2];
                string token = parts[3];

                onRecived(username, email, url, token);
            };

            _channel.BasicConsume(queue: "send_email", autoAck: true, consumer: consume);
        }
    }
}
