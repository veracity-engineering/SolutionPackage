using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DNVGL.Common.Core.Hash
{
    public static class HashExtension
    {
        // SHA1 is light weight and good at creating distribution across the range.
        // Do not use for encryption!
        private static readonly SHA1CryptoServiceProvider Hasher = new SHA1CryptoServiceProvider();

        public static long GetLongHashCode(this Guid key)
        {
            // Hash algorithms need byte arrays, so we're converting the Guid here
            var guidBytes = key.ToByteArray();

            // Hash the Guid's bytes.
            var hashedBytes = Hasher.ComputeHash(guidBytes);

            // Now that our data is repeatibly but distributed evenly, we make it a long
            var guidAsLong = BitConverter.ToInt64(hashedBytes, 0);

            // return the partition key
            return guidAsLong;
        }
    }
}
