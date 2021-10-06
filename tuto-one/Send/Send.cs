using System;
using RabbitMQ.Client;
using System.Text;

namespace Send
{
    class Send
    {
        public static void Main(string[] args)
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
					channel.QueueDeclare(queue: "hello",
									     durable: false,
										 exclusive: false,
										 autoDelete: false,
										 arguments: null);
					string message = "Hello Vitis Receiver, this is Vitis Sender.";
					var body = Encoding.UTF8.GetBytes(message);

					channel.BasicPublish(exchange: "",
										 routingKey: "hello",
										 basicProperties: null,
										 body: body);

					Console.WriteLine("Message sent.");
				}
			}
			Console.WriteLine("Enter to exit...");
			Console.ReadLine();
        }
    }
}
