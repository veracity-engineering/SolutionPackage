namespace DNV.OAuth.Abstractions
{
	public class VeracityClientOptions
	{
		private VeracityOptions _veracityOptions;

		public VeracityEnvironment Environment { get; set; } = VeracityEnvironment.Production;
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string SubscriptionKey { get; set; }

		public VeracityOptions VeracityOptions => _veracityOptions ??= VeracityOptions.Get(this.Environment);
	}
}
