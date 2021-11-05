using System;
using System.Collections.Generic;
using System.Linq;

namespace DNV.SecretsManager.ConsoleApp.Commands
{
	public class ConsoleCommand
	{
		public static Dictionary<string, object> CollectOptions(IEnumerable<ConsoleOption> optionDefinitions, string[] args)
		{
			var options = new Dictionary<string, object>();
			var argumentIndex = 0;
			while (argumentIndex < args.Length)
			{
				var arg = args[argumentIndex];

				// Long option
				ConsoleOption optionDefinition = null;
				if (arg.StartsWith("--"))
				{
					optionDefinition = optionDefinitions.FirstOrDefault(o => o.Name.Equals(arg.Substring(2, arg.Length - 2)));
				}
				// Short option
				else if (arg.StartsWith('-'))
				{
					optionDefinition = optionDefinitions.FirstOrDefault(o => o.Abbreviation.Equals(arg[1]));
				}

				if (optionDefinition == null)
					throw new ArgumentException($"Unrecognized argument '{arg}'.");

				if (optionDefinition.IsFlag)
				{
					options.Add(optionDefinition.Name, true);
				}
				else
				{
					options.Add(optionDefinition.Name, args[++argumentIndex]);
				}
				argumentIndex++;
			}

			return options;
		}
	}
}
