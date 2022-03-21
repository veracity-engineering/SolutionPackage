using System;

namespace DNVGL.Common.Core.Continuation
{
    /// <summary>
    /// 
    /// </summary>
    public struct ContinuationToken
    {
        public static ContinuationToken None { get; } = default;

        public string Key { get; }

        public bool EndOfResult { get; }

        private ContinuationToken(string key, bool eor)
        {
            Key = key;
            EndOfResult = eor;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ContinuationToken o))
                return false;

            return Equals(o);
        }

        public bool Equals(ContinuationToken other)
        {
            return Key == other.Key;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public static bool operator ==(ContinuationToken left, ContinuationToken right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ContinuationToken left, ContinuationToken right)
        {
            return !left.Equals(right);
        }

        public static ContinuationToken Create(string key, bool eor = false)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return new ContinuationToken(key, eor);
        }
    }
}
