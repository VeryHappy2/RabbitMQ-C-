using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Runtime.CompilerServices;
using System.Text;

namespace RabbitMQ
{
    public class RabbitBgWorker : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;

        private static bool _isConnected = false;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Проверяем, если уже подключено, пропускаем попытку подключения
            if (!_isConnected)
            {
                _isConnected = await TryConnectOnceAsync(stoppingToken);
                if (_isConnected)
                {
                    Console.WriteLine("Connected to RabbitMQ");
                }
                else
                {
                    Console.WriteLine("Failed to connect to RabbitMQ. Running without connection.");
                    return;
                }
            }

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consume;
            _channel.BasicConsume("TestQueue", false, consumer);
        }

        private async Task<bool> TryConnectOnceAsync(CancellationToken stoppingToken, int delayMilliseconds = 5000)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = "host.docker.internal",
                    Password = "rabbitmq",
                    UserName = "rabbitmq",
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(
                    queue: "TestQueue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initial connection attempt failed: {ex.Message}");
                Console.WriteLine("Waiting 5 seconds before continuing without connection...");
                await Task.Delay(delayMilliseconds, stoppingToken);
            }
            return false;
        }

        private void Consume(object? ch, BasicDeliverEventArgs eventArgs)
        {   
            try
            {
                String content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                Console.WriteLine($"The message: {content}");
                _channel.BasicAck(eventArgs.DeliveryTag, false);
            }
            catch
            {
                Console.WriteLine("I got a problem");
                _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
            }
            
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
