using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DNVGL.Common.Core.JsonOptions
{
	public static class JsonOptionsExtensions
	{
		internal static readonly JsonSerializerOptions WebDefaultOptions = new(JsonSerializerDefaults.Web);
		internal static readonly JsonSerializerOptions GeneralDefaultOptions = new(JsonSerializerDefaults.General);

		public static IServiceCollection AddWebDefaultJsonOptions(this IServiceCollection services)
		{
			services.TryAddSingleton(_ => Options.Create(WebDefaultOptions));

			return services;
		}

		public static IServiceCollection AddGeneralDefaultJsonOptions(this IServiceCollection services)
		{
			services.TryAddSingleton(_ => Options.Create(GeneralDefaultOptions));

			return services;
		}

		internal static void CopyTo(this JsonSerializerOptions source, JsonSerializerOptions target)
		{
			target.AllowTrailingCommas = source.AllowTrailingCommas;
			target.IgnoreNullValues = source.IgnoreNullValues;
			target.IgnoreReadOnlyProperties = source.IgnoreReadOnlyProperties;
			target.DefaultBufferSize = source.DefaultBufferSize;
			target.MaxDepth = source.MaxDepth;
			target.DefaultIgnoreCondition = source.DefaultIgnoreCondition;
			target.DictionaryKeyPolicy = source.DictionaryKeyPolicy;
			target.Encoder = source.Encoder;
			target.IgnoreReadOnlyFields = source.IgnoreReadOnlyFields;
			target.IncludeFields = source.IncludeFields;
			target.NumberHandling = source.NumberHandling;
			target.PropertyNameCaseInsensitive = source.PropertyNameCaseInsensitive;
			target.ReadCommentHandling = source.ReadCommentHandling;
			target.ReferenceHandler = source.ReferenceHandler;
			target.WriteIndented = source.WriteIndented;
			target.PropertyNamingPolicy = source.PropertyNamingPolicy;
			target.Converters.Clear();
			source.Converters.ToList().ForEach(c => target.Converters.Add(c));
		}
	}
}
