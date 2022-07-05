using DNV.Context.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNV.Context.ServiceBus
{
	public class LocalContextAccessor<T> : IContextAccessor<T> where T : class
	{
		public static readonly string HeaderKey = $"X-Ambient-Context-{typeof(T).Name}";

		public bool Initialized => throw new NotImplementedException();

		public IAmbientContext<T>? Context => throw new NotImplementedException();
	}
}
