using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ReceiveLogDirect
{
    class ReceiveLogDirect
    {
        public static void Main(string[] args)
        {
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using(var connection = factory.CreateConnection())
			{
				using(var channel = connection.CreateModel())
				{

					// Declare exchange with name and type
					channel.ExchangeDeclare(
							exchange: "direct_logs",
							type: "direct");

					// RabbitMQ assigns a random queue name, we dont need to manage it
					var queueName = channel.QueueDeclare().QueueName;


					if(args.Length < 1)
					{
						Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
								Environment.GetCommandLineArgs()[0]);
						Console.WriteLine(" Press [enter] to exit.");
						Console.ReadLine();
						Environment.ExitCode = 1;
						return;
					}

					foreach(var severity in args)
					{
						// Declare routing key
						channel.QueueBind(queue: queueName,
								exchange: "direct_logs",
								routingKey: severity);
					}

					Console.WriteLine(" [*] Waiting for messages.");

					var consumer = new EventingBasicConsumer(channel);
					consumer.Received += (model, ea) =>
					{
						var body = ea.Body.ToArray();
						var message = Encoding.UTF8.GetString(body);
						var routingKey = ea.RoutingKey;
						Console.WriteLine(" [x] Received '{0}':'{1}'",
								routingKey, message);
					};
					channel.BasicConsume(queue: queueName,
							autoAck: true,
							consumer: consumer);

					Console.WriteLine(" Press [enter] to exit.");
					Console.ReadLine();
				}
			}
        }
    }
}
