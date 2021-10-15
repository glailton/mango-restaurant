using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Repository;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mango.Services.Email.Messaging
{
    public class RabbitMQEmailConsumer : BackgroundService
    {
        private readonly string _url = "amqps://haclcmpk:CRP7ljQV-OBWeAeV5-RReFOVrThDhqf5@cattle.rmq2.cloudamqp.com/haclcmpk";
        private IConnection _connection;
        private IModel _channel;
        private const string ExchangeName= "DirectPaymentUpdate_Exchange";
        private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
        private readonly EmailRepository _emailRepository;
        string queueName = "";

        public RabbitMQEmailConsumer( EmailRepository emailRepository)
        {
            _emailRepository = emailRepository;

            var factory = new ConnectionFactory
            {
                Uri = new Uri(_url)
            };
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
            _channel.QueueBind(PaymentEmailUpdateQueueName, ExchangeName, "PaymentEmail");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                UpdatePaymentResultMessage updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(content);
                HandleMessage(updatePaymentResultMessage).GetAwaiter().GetResult();
                
                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume(PaymentEmailUpdateQueueName, false, consumer);

            return Task.CompletedTask;
        }
        
        private async Task HandleMessage(UpdatePaymentResultMessage updatePaymentResultMessage)
        {
            try
            {
                await _emailRepository.SendAndLogEmail(updatePaymentResultMessage);
            }
            catch (Exception e)
            {
                throw;
            }
            
        }
    }
}