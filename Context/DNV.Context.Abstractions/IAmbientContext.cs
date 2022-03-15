using System.Collections.Generic;

namespace DNV.Context.Abstractions
{
	public interface IAmbientContext<out T> where T : class
	{
		string? CorrelationId { get; }

		T? Payload { get; }

		IDictionary<object, object>? Items { get; }
	}
}
