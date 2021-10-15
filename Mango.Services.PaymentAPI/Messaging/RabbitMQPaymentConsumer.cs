using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mango.Services.PaymentAPI.Messages;
using Mango.Services.PaymentAPI.RabbitMQSender;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mango.Services.PaymentAPI.Messaging
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly string _url = "amqps://haclcmpk:CRP7ljQV-OBWeAeV5-RReFOVrThDhqf5@cattle.rmq2.cloudamqp.com/haclcmpk";
        private IConnection _connection;
        private IModel _channel;
        private readonly IRabbitMQPaymentMessageSender _messageSender;
        private readonly IProcessPayment _processPayment;

        public RabbitMQPaymentConsumer( IRabbitMQPaymentMessageSender messageSender, IProcessPayment processPayment)
        {
            _messageSender = messageSender;
            _processPayment = processPayment;

            var factory = new ConnectionFactory
            {
                Uri = new Uri(_url)
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "orderpaymentprocesstopic", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(content);
                HandleMessage(paymentRequestMessage).GetAwaiter().GetResult();
                
                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume("checkoutqueue", false, consumer);

            return Task.CompletedTask;
        }
        
        private async Task HandleMessage(PaymentRequestMessage paymentRequestMessage)
        {
            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage updatePaymentResultMessage = new()
            {
                Status = result,
                OrderId = paymentRequestMessage.OrderId,
                Email = paymentRequestMessage.Email
            };
            
            try
            {
                _messageSender.SendMessage(updatePaymentResultMessage);
            }
            catch (Exception e)
            {
                throw;
            }
            
        }
    }
}