using System.Collections.Generic;

namespace DNVGL.Common.Core.Pagination
{
    /// <summary>
    /// 
    /// </summary>
    public static class PaginatedResultExtensions
    {
	    public static PaginatedResult<T> Paginate<T>(this IEnumerable<T> result, int pageIndex, int pageSize, int? totalCount)
        {
            return new PaginatedResult<T>(result, pageIndex, pageSize, totalCount);
        }

	    public static PaginatedResult<T> Paginate<T>(this IEnumerable<T> result, PageParam pageParam, int? totalCount)
        {
            return result.Paginate(pageParam.PageIndex, pageParam.PageSize, totalCount);
        }
    }
}