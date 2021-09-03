using System;
using System.IO;

namespace DNV.SecretsManager.ConsoleApp
{
	internal static class ValidationUtility
	{
		public static bool IsUriValid(string value)
		{
			return Uri.IsWellFormedUriString(value, UriKind.Absolute);
		}

		public static bool IsFilenameValid(string value)
		{
			FileInfo fileInfo = null;
			try
			{
				fileInfo = new FileInfo(value);
			}
			catch (ArgumentException) { }
			catch (PathTooLongException) { }
			catch (NotSupportedException) { }
			return !ReferenceEquals(fileInfo, null);
		}
	}
}
