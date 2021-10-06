using System;
using System.Threading;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker
{
    class Worker
    {
        static void Main(string[] args)
        {
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					// We declare the queue here aswell because we might start the consumer before the publisher, make sure the queue exists before trying to consume messages from it
					channel.QueueDeclare(queue: "task_queue",
							durable: true,
							exclusive: false,
							autoDelete: false,
							arguments: null);


					// only one message at a time
					channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

					// Server deliver us messages from queue, it will push messages asynchronously, so we provide a callback
					// That is what EventingBasicConsumer.Received event handler does
					var consumer = new EventingBasicConsumer(channel);
					consumer.Received += (model, ea) =>
					{
						var body = ea.Body.ToArray();
						var message = Encoding.UTF8.GetString(body);
						Console.WriteLine($"Message Received : {message}");

						int dots = message.Split('.').Length - 1;
						Thread.Sleep(dots * 1000);

						Console.WriteLine("Done !");

						channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
					};

					channel.BasicConsume(queue: "task_queue",
										 autoAck: false,
										 consumer: consumer);
					Console.WriteLine("Enter to exit...");
					Console.ReadLine();
				}
			}

        }
    }
}
