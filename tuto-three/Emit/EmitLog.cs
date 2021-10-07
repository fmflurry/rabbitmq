using System;
using RabbitMQ.Client;
using System.Text;
using System.Linq;

namespace Emit
{
    class EmitLog
    {
		public static void Main(string[] args)
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using(var connection = factory.CreateConnection())
			{
				using(var channel = connection.CreateModel())
				{
					// Publishing to a non-existing exchange is forbidden, make sure to declare it
					channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

					var message = GetMessage(args);
					var body = Encoding.UTF8.GetBytes(message);
					channel.BasicPublish(exchange: "logs",
							routingKey: "",
							basicProperties: null,
							body: body);
					Console.WriteLine(" [x] Sent {0}", message);
				}
				Console.WriteLine(" Press [enter] to exit.");
				Console.ReadLine();
			}
		}

		private static string GetMessage(string[] args)
		{
			return args.Any()
					? string.Join(" ", args)
					: "info: Hello World!";
		}
    }
}
