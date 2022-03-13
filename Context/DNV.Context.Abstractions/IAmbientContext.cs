namespace DNV.Context.Abstractions
{
	public interface IAmbientContext<out T> where T: class
	{
		string Key { get; }

		T Context { get; }
	}
}
