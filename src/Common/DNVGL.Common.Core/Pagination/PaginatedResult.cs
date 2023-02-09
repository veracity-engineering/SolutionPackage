using System;
using System.Collections;
using System.Collections.Generic;

namespace DNVGL.Common.Core.Pagination
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedResult<T>: IEnumerable<T>
    {
	    private readonly IEnumerable<T> _result;

	    public PaginatedResult(IEnumerable<T> result, int pageIndex, int pageSize, int? totalCount)
	    {
		    if (pageIndex <= 0)
			    throw new ArgumentOutOfRangeException(nameof(pageIndex), "Must be greater than zero.");
		    if (pageSize <= 0)
			    throw new ArgumentOutOfRangeException(nameof(pageSize), "Must be greater than zero.");
		    if (totalCount < 0)
			    throw new ArgumentOutOfRangeException(nameof(totalCount), "Must be greater than or equal to zero.");
		    _result = result;
            PageIndex = pageIndex;
		    PageSize = pageSize;
		    TotalCount = totalCount;
	    }

        public int PageIndex { get; }

        public int PageSize { get; }

        public int? TotalCount { get; }

        public int? TotalPages => TotalCount.HasValue? 
	        TotalCount / PageSize + (((TotalCount % PageSize) > 0)? 1: 0): 
	        null;

        public IEnumerator<T> GetEnumerator()
        {
	        return _result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
	        return GetEnumerator();
        }
    }
}