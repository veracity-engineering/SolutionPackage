using System;
using System.Collections.Generic;
using System.Text;

namespace DNV.Context.Abstractions
{
	public interface IContextCreator<T>
	{
		void InitializeContext(T? payload, string? correlationId, IDictionary<string, object>? items = null);
	}
}
