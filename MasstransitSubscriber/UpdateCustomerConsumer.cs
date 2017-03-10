using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MassTransitCommon;

namespace MasstransitSubscriber
{
    public class UpdateCustomerConsumer :
    IConsumer<IUpdateCustomerAddress>
    {
        public async Task Consume(ConsumeContext<IUpdateCustomerAddress> context)
        {
            await Console.Out.WriteLineAsync($"Updating customer: {context.Message.CustomerId}");

            // update the customer address
        }
    }
}
