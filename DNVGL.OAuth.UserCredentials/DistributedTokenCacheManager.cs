using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Text;

namespace DNVGL.OAuth.Api.HttpClient
{
    public interface IDistributedTokenCacheManager
    {
        void SetCacheInstance(ITokenCache cache);

        ITokenCache GetCacheInstance();
    }

    public class DistributedTokenCacheManager : IDistributedTokenCacheManager
    {
        private readonly IDistributedCache _distributedCache;
        private string _cacheKey;
        private ITokenCache _tokenCache;

        public DistributedTokenCacheManager(IDistributedCache distributedCache)
        {
            _cacheKey = "IDontGiveAFuck";
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        public void SetCacheInstance(ITokenCache cache)
        {
            _tokenCache = cache;
            GetCacheInstance();
        }

        public ITokenCache GetCacheInstance()
        {
            _tokenCache.SetBeforeAccess(BeforeAccessNotification);
            _tokenCache.SetAfterAccess(AfterAccessNotification);
            return _tokenCache;
        }

        public string ReadUserStateValue()
        {
            string state = string.Empty;

            var binaryState = _distributedCache.Get($"{_cacheKey}_state");
            if (binaryState != null && binaryState.Length > 0)
                state = Encoding.UTF8.GetString(binaryState);
            return state;
        }

        public void Load(TokenCacheNotificationArgs args)
        {
            var binaryData = _distributedCache.Get(_cacheKey);
            if (binaryData != null)
            {
                var str = Encoding.UTF8.GetString(binaryData);
            }

            try
            {
                args.TokenCache.DeserializeMsalV3(binaryData);
            }
            catch (Exception ex)
            {
                args.TokenCache.DeserializeMsalV2(binaryData);
            }
        }

        public void Persist(TokenCacheNotificationArgs args)
        {
            var binaryData = args.TokenCache.SerializeMsalV3();
            if (binaryData != null)
            {
                var str = Encoding.UTF8.GetString(binaryData);
            }

            _distributedCache.Set(_cacheKey, binaryData);
        }

        // Triggered right before MSAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            Load(args);
        }

        // Triggered right after MSAL accessed the cache.
        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (args.HasStateChanged)
            {
                Persist(args);
            }
        }
    }
}