using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DNVGL.AuthTest.Web
{
    public static class ConfigurationSectionExtensions
    {
        public static IEnumerable<T> ToCollection<T>(this IConfigurationSection section)
        {
            var collection = new List<T>();
            foreach (var child in section.GetChildren())
            {
                collection.Add(child.Get<T>());
            }
            return collection;
        }
    }
}
