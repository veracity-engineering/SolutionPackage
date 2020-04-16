using System.Collections.Generic;

namespace DNVGL.Common.Core.Pagination
{
    /// <summary>
    /// 
    /// </summary>
    public static class PaginatedResult
    {
        private class PResult<T> : IPaginatedResult<T>
        {
            public PResult(IEnumerable<T> result, ContinuationToken continuationToken)
            {
                Result = result;

                ContinuationToken = continuationToken;
            }

            public IEnumerable<T> Result { get; }

            public ContinuationToken ContinuationToken { get; }
        }

        public static IPaginatedResult<T> Create<T>(IEnumerable<T> result, ContinuationToken continuationToken)
        {
            return new PResult<T>(result, continuationToken);
        }

        public static IPaginatedResult<T> PaginateAt<T>(this IEnumerable<T> result, ContinuationToken continuationToken)
        {
            return Create(result, continuationToken);
        }
    }
}