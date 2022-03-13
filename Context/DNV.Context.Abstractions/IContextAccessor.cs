namespace DNV.Context.Abstractions
{
	public interface IContextAccessor<out T> where T : class
	{
		public IAmbientContext<T>? Current { get; }
	}
}
