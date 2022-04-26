using Microsoft.AspNetCore.Http;

namespace DNVGL.Web.Security.PermissionsPolicies
{
    public static class PermissionsPolicyExtensions
    { 
        public static void DisableAllPermissionsPolicy(this IHeaderDictionary dict)
        {
            var policy = new PermissionsPolicy();
            FeatureNames.All.ForEach(name => {
                policy.Feature(name).Disable();
            });
            dict.Add(PermissionsPolicy.Key, policy.ToString());
        }

        public static void EnableAllPermissionsPolicyForSelf(this IHeaderDictionary dict)
        {
            var policy = new PermissionsPolicy();
            FeatureNames.All.ForEach(name =>
            {
                policy.Feature(name).Enable().Self();
            });
            dict.Add(PermissionsPolicy.Key, policy.ToString());
        }
    }
}

