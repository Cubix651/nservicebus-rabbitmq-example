using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;

namespace Receiver
{
    public class Handler : IHandleMessages<SimpleMessage>
    {
        public Task Handle(SimpleMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received: {message.Content}");
            return Task.CompletedTask;
        }
    }
}