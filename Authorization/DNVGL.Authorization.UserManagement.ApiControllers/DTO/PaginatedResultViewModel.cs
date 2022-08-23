using DNVGL.Common.Core.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Authorization.UserManagement.ApiControllers.DTO
{
	public class PaginatedResultViewModel<T>
	{
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int? TotalCount { get; set; }

        public IEnumerable<T> List { get; set; }

        public PaginatedResultViewModel(PaginatedResult<T> paginatedResult)
        {
            PageIndex = paginatedResult.PageIndex;
            PageSize = paginatedResult.PageSize;
            TotalCount = paginatedResult.TotalCount;
            List = paginatedResult;
        }
    }
}
