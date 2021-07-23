using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DNVGL.Web.Security.PermissionsPolicies
{
    public class PermissionsPolicy
    {
        public static string Key {get { return "Permissions-Policy"; } }
        private readonly List<Feature> features = new List<Feature>();
        public PermissionsPolicy()
        {

        }

        public Feature Feature(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException($"Feature {nameof(name)} can't be null or empty.");
            }

            if (features.Any(f => f.Name == name))
            {
                throw new DuplicateNameException($"Duplicate feature name ({name}).");
            }

            var feature = new Feature(name);
            features.Add(feature);
            return feature;
        }

        public override string ToString()
        {
            var values = string.Join(",", features.Select(f => f.ToString()).ToArray());
            return values;
        }
    }
}

