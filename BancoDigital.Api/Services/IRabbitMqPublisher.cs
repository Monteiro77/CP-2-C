namespace BancoDigital.Api.Services;

public interface IRabbitMqPublisher
{
    void Publish(string queueName, string message);
}
