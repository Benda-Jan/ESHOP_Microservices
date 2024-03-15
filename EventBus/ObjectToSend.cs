using System;
using RabbitMQ.Client;

namespace EventBus
{
	public class ObjectToSend
	{
		public string Id { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
	}
}

