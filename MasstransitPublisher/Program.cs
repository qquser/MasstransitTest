using System;
using System.Configuration;
using System.Threading;
using MassTransit;
using MassTransit.Util;
using System.Threading.Tasks;
using MassTransitCommon;


namespace MasstransitPublisher
{
    public class Program
    {
        public static void Main()
        {
            Startup.CreateBus();

            Console.WriteLine("Hello Publisher!");
            //Console.ReadKey();
            ConsoleKeyInfo input;
            do
            {
                //Console.WriteLine("1");
                var text = Console.ReadLine();
                //ProjectAdd(Guid.NewGuid());
                var command = new AddCustomerCommand
                {
                    CustomerId = text,
                };
                Send(command).Wait();

                input = Console.ReadKey();
            } while (input.Key != ConsoleKey.Escape);
        }
        class AddCustomerCommand : IUpdateCustomerAddress
        {
            public string CustomerId { get; set; }
 
        }

        static async Task Send<TMessage>(TMessage message,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : class
        {
            var address = "rabbitmq://localhost/customer_update_queue";
            var endpoint = await Startup.Bus.GetSendEndpoint(new Uri(address));
            await endpoint.Send<TMessage>(message, cancellationToken);
        }

        public class Startup
        {
            static IBusControl _bus;
            public static IBus Bus => _bus;
            public static void CreateBus()
            {
                _bus = MassTransit.Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri("rabbitmq://localhost/"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                });
                _bus.StartAsync().Wait();

                //TaskUtil.Await(() => _bus.StartAsync());
            }
        }


    }
}
