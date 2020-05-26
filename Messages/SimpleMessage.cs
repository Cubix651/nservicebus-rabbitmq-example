using System;
using NServiceBus;

namespace Messages
{
    public class SimpleMessage : IEvent
    {
        public string Content {get; set;}
    }
}
