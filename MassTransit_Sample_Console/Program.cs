
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GreenPipes;
using System.Threading;
using System.Collections.Concurrent;
using System.Transactions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MassTransit.Util;

namespace MassTransit_Sample_Console
{
    public class Message
    {
        public string Text { get; set; }
    }

    public class Program
    {
        public static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host("rabbitmq://localhost");

                sbc.ReceiveEndpoint("test_queue", ep =>
                {
                    ep.Handler<Message>(context =>
                    {
                        return Console.Out.WriteLineAsync($"Received: {context.Message.Text}");
                    });
                });
            });

            await bus.StartAsync(); // This is important!



            while (true)
            {
                Console.WriteLine("Envie mensaje o pulse (q) para salir:");
                var publicacion = Console.ReadLine();
                if (publicacion.Equals("q"))
                {
                    break;
                }
                else
                {
                    await bus.Publish(new Message { Text = publicacion });
                }
            }
            await bus.StopAsync();
        }
    }
}
