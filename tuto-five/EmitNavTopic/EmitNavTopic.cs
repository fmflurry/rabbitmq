using System;
using System.Linq;
using RabbitMQ.Client;
using System.Text;

namespace EmitNavTopic
{
    class EmitNavTopic
    {
        public static void Main(string[] args)
        {
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using(var connection = factory.CreateConnection())
			{
				using(var channel = connection.CreateModel())
				{
					channel.ExchangeDeclare(
							exchange: "topic_nav",
							type: "topic");

					var routingKey = args.Any() ?
						args[0]
						: "nav.*";

					var message = args.Any()
						? string.Join(" ", args.Skip( 1 ).ToArray())
						: "DefaultNav";

					var body = Encoding.UTF8.GetBytes(message);
					channel.BasicPublish(exchange: "topic_nav",
							routingKey: routingKey,
							basicProperties: null,
							body: body);
					Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
				}
			}
        }
    }
}
