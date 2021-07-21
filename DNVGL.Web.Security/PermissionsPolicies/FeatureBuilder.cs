using System;
using System.Collections.Generic;
using System.Linq;

namespace DNVGL.Web.Security.PermissionsPolicies
{
    public class FeatureBuilder
    {
        private List<string> allows = new List<string>();
        
        internal FeatureBuilder()
        {

        }

        internal bool Enabled { get { return allows.Any(); } }

        internal void Enable()
        {
            allows.RemoveAll(a => true );
            allows.Add("*");
        }

        internal void Disable()
        {
            allows.RemoveAll(a => true);
        }

        public FeatureBuilder Self()
        {
            allows.RemoveAll(a => a == "*" || a == "self");
            allows.Add("self");
            return this;
        }

        public FeatureBuilder Custom(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            string wrapper = $"\"{url}\"";

            allows.RemoveAll(a => a == "*" || a.ToLowerInvariant() == wrapper.ToLowerInvariant());
            allows.Add(wrapper);
            return this;
        }

        public override string ToString()
        {
            if (allows.Count() == 1 && allows.First() == "*")
            {
                return "*";
            }

            var result = string.Join(",", allows.ToArray());
            return $"({result})";
        }
    }
}

