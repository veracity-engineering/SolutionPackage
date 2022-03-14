using DNV.Context.Abstractions;

namespace DNV.Context.AspNet
{
	internal sealed class AspNetContext<T> : IAmbientContext<T> where T : class
	{
		internal static readonly string HeaderKey = $"X-Ambient-Context-{typeof(T).Name}";

		internal AspNetContext(T context)
		{
			Context = context;
		}

		public string Key => HeaderKey;

		public T Context { get; }
	}
}
