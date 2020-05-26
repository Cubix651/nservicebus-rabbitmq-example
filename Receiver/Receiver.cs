using System;
using System.Threading.Tasks;
using NServiceBus;
using RabbitMQ.Client.Exceptions;

namespace Receiver
{
    class Receiver
    {
        static async Task Main(string[] args)
        {
            var program = new Receiver();

            await program.RunProgram(args);
        }

        private async Task RunProgram(string[] args)
        {
            for(int i = 0; i < 10; ++i)
            {
                if(await RunInstance())
                    break;
                Console.WriteLine("Retrying in 5 seconds");
                await Task.Delay(5000);
            }
        }

        private async Task<bool> RunInstance()
        {
            try {
                var endpointConfiguration = GetEndpointConfiguration();
                var endpointInstance = await Endpoint.Start(endpointConfiguration)
                    .ConfigureAwait(false);

                await Task.Delay(40 * 1000);

                Console.WriteLine("Stopping instance");

                await endpointInstance.Stop()
                    .ConfigureAwait(false);
            }
            catch (BrokerUnreachableException)
            {
                Console.Error.WriteLine("Broker Unreachable");
                return false;
            }
            return true;
        }

        private EndpointConfiguration GetEndpointConfiguration()
        {
            var endpointConfiguration = new EndpointConfiguration($"Receiver.{Environment.MachineName}");
            endpointConfiguration.EnableInstallers();
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>()
                .ConnectionString("host=rabbitmq")
                .UseCustomRoutingTopology(durable => new RoutingTopology(durable));

            return endpointConfiguration;
        }
    }
}
