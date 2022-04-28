using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DNV.SecretsManager.ConsoleApp.Commands
{
	public static class ConsoleCommand
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

		public static CommandType GetCommandTypeOrInvalid(CommandType value, ConsoleOption downloadOption, ConsoleOption uploadOption)
		{
			var _commandTypes = new Dictionary<char, CommandType>
			{
				{ downloadOption.Abbreviation, CommandType.Download },
				{ uploadOption.Abbreviation, CommandType.Upload }
			};
			if (value == CommandType.None)
				Console.WriteLine($"What would you like to do? (Download: [{downloadOption.Abbreviation}], Upload: [{uploadOption.Abbreviation}])");
			while (value == CommandType.None)
			{
				var commandTypeChoice = $"{Console.ReadKey().Key}".ToLowerInvariant()[0];
				Console.WriteLine();
				if (_commandTypes.ContainsKey(commandTypeChoice))
				{
					return _commandTypes[commandTypeChoice];
				}
				else
				{
					Console.WriteLine($"Invalid option '{commandTypeChoice}'. Please enter a valid option (Download: [{downloadOption.Abbreviation}], Upload: [{uploadOption.Abbreviation}])");
				}
			}
			return value;
		}

		public static string GetStringOrInvalid(string value, string prompt, Func<string, bool> isValid, Func<string, string> invalidMessage)
		{
			if (string.IsNullOrEmpty(value))
				Console.WriteLine(prompt);
			while (string.IsNullOrEmpty(value))
			{
				var input = Console.ReadLine();
				if (isValid(input))
				{
					return input;
				}
				else
				{
					Console.WriteLine(invalidMessage(input));
				}
			}
			return value;
		}

		public static string GetFilenameOrInvalid(CommandType type, string value)
		{
			if (type == CommandType.Download)
				return GetDownloadFilenameOrInvalid(value);
			if (type == CommandType.Upload)
				return GetUploadFilenameOrInvalid(value);
			return value;
		}

		private static string GetDownloadFilenameOrInvalid(string value)
		{
			if (string.IsNullOrEmpty(value))
				Console.WriteLine("Specify the target filename you would like to download to:");
			while (string.IsNullOrEmpty(value))
			{
				var input = Console.ReadLine();
				if (ValidationUtility.IsFilenameValid(input))
				{
					return input;
				}
				else
				{
					Console.WriteLine($"Invaild filename '{input}'. Please specify a valid filename to download to:");
				}
			}
			return value;
		}

		private static string GetUploadFilenameOrInvalid(string value)
		{
			if (string.IsNullOrEmpty(value))
				Console.WriteLine("Specify the source file you would like to upload:");
			while (string.IsNullOrEmpty(value))
			{
				var input = Console.ReadLine();
				if (ValidationUtility.IsFilenameValid(input))
				{
					if (File.Exists(input))
					{
						return input;
					}
					else
					{
						Console.WriteLine($"Could not find source file '{input}'.  Please specify an existing file to upload:");
					}
				}
				else
				{
					Console.WriteLine($"Invalid filename '{input}'. Please specify a valid filename to upload:");
				}
			}
			return value;
		}

		public static string BuildCommandUseage(IConsoleCommand command, string applicationName)
		{
			var stringBuilder = new StringBuilder($"useage: {applicationName} {command.Name}");
			if (command.Options != null && command.Options.Any())
			{
				stringBuilder.Append("\t");
				foreach (var option in command.Options)
				{
					stringBuilder.Append($"{BuildOptionUseage(option)} ");
				}
			}
			return stringBuilder.ToString().TrimEnd(' ');
		}

		private static string BuildOptionUseage(ConsoleOption option)
		{
			var value = option.IsFlag
				? string.Empty
				: $" <{option.Name}>";
			if (option.IsOptional)
				return $"[-{option.Abbreviation} | --{option.Name}{value}]";
			return $"-{option.Abbreviation} | --{option.Name}{value}";
		}
	}
}
