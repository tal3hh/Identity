namespace AspNetCoreIdentity.RabbitMQ.Interface
{
    public interface IRabbitMQProducer
    {
        void ConfrimEmail<T>(T message);
    }
}
