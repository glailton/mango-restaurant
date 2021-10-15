using System;
using System.Text;
using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Mango.Services.OrderAPI.RabbitMQSender
{
    public class RabbitMqOrderMessageSender : IRabbitMQOrderMessageSender
    {
        private readonly string _url = "amqps://haclcmpk:CRP7ljQV-OBWeAeV5-RReFOVrThDhqf5@cattle.rmq2.cloudamqp.com/haclcmpk";
        private IConnection _connection;
        
        private const string ExchangeName = "PublishSubscrivePaymentUpdate_Exchange";
        
        public void SendMessage(BaseMessage baseMessage, string queueName)
        {
            if (ConnectionExists())
            {
                using var channel = _connection.CreateModel();
                channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, false);
                var json = JsonConvert.SerializeObject(baseMessage);
                var body = Encoding.UTF8.GetBytes(json);
                channel.BasicPublish(exchange: ExchangeName, "", basicProperties: null, body: body);
            }
        }
        
        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(_url)
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception)
            {
                //log exception
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }
            CreateConnection();
            return _connection != null;
        }
    }
}