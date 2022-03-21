using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Common.Core.Pagination
{
	public class PageParam
	{
		public PageParam(int pageIndex, int pageSize)
		{
			if (pageIndex <= 0)
				throw new ArgumentOutOfRangeException(nameof(pageIndex), "Must greater than zero.");

			if (pageSize <= 0)
				throw new ArgumentOutOfRangeException(nameof(pageSize), "Must greater than zero.");

			PageIndex = pageIndex;
			PageSize = pageSize;
		}

		public int PageIndex { get; }

		public int PageSize { get; }
	}
}
