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
			FileInfo fileInfo;
			try
			{
				fileInfo = new FileInfo(value);
			}
			catch (ArgumentException) { return false; }
			catch (PathTooLongException) { return false; }
			catch (NotSupportedException) { return false; }
			return !ReferenceEquals(fileInfo, null);
		}
	}
}
