using System.Collections.Generic;

namespace DNVGL.Common.Core.Pagination
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPaginatedResult<out T>
    {
        IEnumerable<T> Result { get; }

        ContinuationToken ContinuationToken { get; }
    }
}