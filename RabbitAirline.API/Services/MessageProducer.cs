using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitAirline.API.Services;

public class MessageProducer : IMessageProducer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageProducer()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "user",
            Password = "mypass",
            VirtualHost = "/"
        };
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare( //Cria a fila se não existir
            queue: "bookings", 
            durable: true, //Fila persistente
            exclusive: false, //Pode ser acessada por outros canais
            autoDelete: false, //Não será deletada quando o servidor for reiniciado
            arguments: null //Não possui argumentos
        );
    }

    public void SendMessage<T>(T message)
    {
        var messageBody = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(messageBody);

        _channel.BasicPublish(
            exchange: "",
            routingKey: "bookings",
            basicProperties: null,
            body: body
        );
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}