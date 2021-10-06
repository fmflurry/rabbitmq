using System;
using RabbitMQ.Client;
using System.Text;
using System.Linq;

namespace NewTask
{
    class NewTask
    {
        static void Main(string[] args)
        {
			// connect to a node on local machine, hence the "localhost"
			var factory = new ConnectionFactory() { HostName = "localhost" };

			// abstract the socket connection and takes care of protocol version negotiation and authentication and so on
			using (var connection = factory.CreateConnection())
			{
				// channel is where most of the API for getting things done resides
				using (var channel = connection.CreateModel())
				{
					// We declare a queue, a queue is idempotent, it will only be created if it doesn't exist already
					channel.QueueDeclare(queue: "task_queue",
							durable: true, // dont delete queue when RabbitMQ stops
							exclusive: false,
							autoDelete: false,
							arguments: null);
					string message = GetMessage(args);
					var body = Encoding.UTF8.GetBytes(message);

					var properties = channel.CreateBasicProperties();
					// Don't delete the queue when RabbitMQ stops
					properties.Persistent = true;

					channel.BasicPublish(exchange: "",
										 routingKey: "task_queue",
										 basicProperties: properties,
										 body: body);

					Console.WriteLine("Message sent.");
				}
			}
			Console.WriteLine("Enter to exit...");
			Console.ReadLine();
        }

		private static string GetMessage(string[] args)
		{
			return args.Any()
				? string.Join(" ", args)
				: "Hello World!";
		}
    }
}
