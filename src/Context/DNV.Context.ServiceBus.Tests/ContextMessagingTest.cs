using Azure.Messaging.ServiceBus;
using DNV.Context.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit.Abstractions;

namespace DNV.Context.ServiceBus.Tests
{
    public class ContextMessagingTest
    {
#if DEBUG
        private const string QUEUE = "unittest";

        public IConfigurationRoot _configuration;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IContextAccessor<UnitTestContext> _contextAccessor;
        private readonly IContextCreator<UnitTestContext> _contextCreator;
        private readonly string _connectionString;

        private readonly UnitTestContext _expectedUnitTestContext;
        private readonly string _expectedCorrelationId;
        private readonly IDictionary<string, object> _expectedContextItems;


        public ContextMessagingTest(ITestOutputHelper testOutputHelper)
        {
            var config = new ConfigurationBuilder().AddUserSecrets(Assembly.GetExecutingAssembly(), true);
            _configuration = config.Build();

            _connectionString = _configuration["ConnectionString"];

            _testOutputHelper = testOutputHelper;

            var localContextAccessor = new LocalContextAccessor<UnitTestContext>((args) =>
            {
                return (false, null);
            });

            _contextAccessor = localContextAccessor;
            _contextCreator = localContextAccessor;

            _expectedUnitTestContext = new UnitTestContext { Purpose = "ut" };
            _expectedCorrelationId = Guid.NewGuid().ToString();
            _expectedContextItems = new Dictionary<string, object>();
            _expectedContextItems.Add("who", "DNV");
            _expectedContextItems.Add("where", "CN");

            localContextAccessor.InitializeContext(_expectedUnitTestContext, _expectedCorrelationId, _expectedContextItems);
        }


        [Fact]
        public async Task QueueBroadcastTest()
        {

            var client = new ServiceBusClient(_connectionString);
            var sender = client.CreateSender(QUEUE);

            var message = new ServiceBusMessage("Message unittest");

            await sender.SendMessageAsync(message, _contextAccessor);


            var processor = client.CreateProcessor(QUEUE, new ServiceBusProcessorOptions());



            var localContextAccessor = new LocalContextAccessor<UnitTestContext>((args) =>
            {
                return (false, null);
            });

            //processor.ProcessMessageAsync += ServiceBusProcessorExtensions.InitializeContext(Processor_ProcessMessageAsync, localContextAccessor, (args) =>
            //{
            //    return (false, null);
            //}, null);

            Func<ProcessMessageEventArgs, Task> messageHandler = args =>
            {
                Assert.Equal(_expectedCorrelationId, localContextAccessor.Context.CorrelationId);
                Assert.Equal(_expectedUnitTestContext.Purpose, localContextAccessor.Context.Payload.Purpose);
                Assert.Equal(_expectedContextItems["who"], localContextAccessor.Context.Items["who"]);

                _testOutputHelper.WriteLine(args.Message.Body.ToString());
                _testOutputHelper.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
                return Task.CompletedTask;
            };

            processor.ProcessMessageAsync += messageHandler.InitializeContext(localContextAccessor, (args) =>
			{
				return (false, null);
			}, null);

            processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

            await processor.StartProcessingAsync();
            Thread.Sleep(2000);


            _testOutputHelper.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
            await processor.StopProcessingAsync();
        }

		[Fact]
		public void Deserialization()
		{
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new DictionaryStringObjectJsonConverter());
            var jsonStr = "{\"CorrelationId\":\"94ae27d3-3189-43ac-b276-86df4b41c195\",\"Payload\":{\"Purpose\":\"ut\"},\"Items\":{\"who\":\"DNV\",\"where\":\"CN\"}}";


            var ctx = JsonSerializer.Deserialize<AsyncLocalContext<UnitTestContext>.ContextHolder>(jsonStr, jsonSerializerOptions);
            Assert.NotNull(ctx);
        }

		private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
		{
            _testOutputHelper.WriteLine(arg.Exception.Message);
            return Task.CompletedTask;
        }

		private Task Processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
		{
            _testOutputHelper.WriteLine("1");
            return Task.CompletedTask;

        }
#endif
    }

    public class UnitTestContext
	{
        public string Purpose { get; set; }
	}
}