using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Util;
using MassTransitCommon;
using Ninject;
using Ninject.Activation.Providers;
using Ninject.Extensions.Conventions;

namespace MasstransitSubscriber
{
    public class Program
    {
        private static readonly StandardKernel _kernel = new StandardKernel();
        private static IBusControl _busControl;
        private static BusHandle _busHandle;
        static void Main(string[] args)
        {
            ConfigureContainer();
            //var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            //{
            //    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h =>
            //    {
            //        h.Username("guest");
            //        h.Password("guest");
            //    });

            //    //cfg.ReceiveEndpoint(host, "customer_update_queue", e =>
            //    //{
            //    //    e.Consumer<UpdateCustomerConsumer>();
            //    //});

            //});
            _busControl = _kernel.Get<IBusControl>();
            StartBus().Wait();
            TaskUtil.Await(() => _busHandle.Ready);

        }
        private static async Task StartBus()
        {
            _busHandle = await _busControl.StartAsync();
        }

        private static void ConfigureContainer()
        {
            _kernel.Bind(x => x
                .FromThisAssembly()
                .IncludingNonePublicTypes() // 
                
                .SelectAllClasses()

                .InheritedFrom(typeof(IConsumer))
                .BindToSelf());

            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                x.ReceiveEndpoint(host, "customer_update_queue", e =>
                {
                    e.LoadFrom(_kernel);
                });
            });



            _kernel.Bind<IBusControl>()
                .ToConstant(busControl)
                .InSingletonScope();

            _kernel.Bind<IBus>()
                .ToProvider(new CallbackProvider<IBus>(x => x.Kernel.Get<IBusControl>()));

 

   
        }
    }


    
}
