using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace DNVGL.Common.Core.Hash
{
    public static class HashExtension
    {
        private static readonly ObjectPool<SHA1CryptoServiceProvider> HasherPool = ObjectPool.Create(new DefaultPooledObjectPolicy<SHA1CryptoServiceProvider>());
        
        public static long GetLongHashCode(this Guid key)
        {
            // Hash algorithms need byte arrays, so we're converting the Guid here
            var guidBytes = key.ToByteArray();

            // Apply a hasher obj from pool
            var hasher = HasherPool.Get();

            // Hash the Guid's bytes.
            var hashedBytes = hasher.ComputeHash(guidBytes);

            // Return the hasher obj to pool
            HasherPool.Return(hasher);
            
            // Now that our data is repeatibly but distributed evenly, we make it a long
            var guidAsLong = BitConverter.ToInt64(hashedBytes, 0);

            // return the partition key
            return guidAsLong;
        }
    }
}
