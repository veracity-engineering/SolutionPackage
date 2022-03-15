namespace DNV.Context.Abstractions
{
	public interface IContextAccessor<out T> where T : class
	{
		bool Initialized { get; }

		IAmbientContext<T>? Context { get; }
	}
}
