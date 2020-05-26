using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using RabbitMQ.Client.Exceptions;

namespace Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var program = new Program();

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

                for(int i = 0; i < 10; ++i)
                {
                    var message = new SimpleMessage { Content = $"Test message {i}" };
                    Console.WriteLine("Send message");
                    await endpointInstance.Publish(message);
                    await Task.Delay(2000);
                }

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
            var endpointConfiguration = new EndpointConfiguration("Sender");
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.SendOnly();
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>()
                .ConnectionString("host=rabbitmq")
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .UseConventionalRoutingTopology();

            return endpointConfiguration;
        }
    }
}
