using System;
using System.Linq;
using RabbitMQ.Client;
using System.Text;
using System.Linq;

namespace EmitLogDirect
{
    class EmitLogDirect
    {
        public static void Main(string[] args)
        {
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using(var connection = factory.CreateConnection())
			{
				using(var channel = connection.CreateModel())
				{
					channel.ExchangeDeclare(exchange: "direct_logs",
							type: "direct");

					var severity = args.Any()
						? args[0]
						: "info";

						? string.Join(" ", args.Skip( 1 ).ToArray())
						: "Hello World!";

					var body = Encoding.UTF8.GetBytes(message);

					channel.BasicPublish(exchange: "direct_logs",
							routingKey: severity,
							basicProperties: null,
							body: body);
					Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
				}

				Console.WriteLine(" Press [enter] to exit.");
				Console.ReadLine();
			}
		}
	}
}
