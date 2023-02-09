using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Common.Core.Sorting
{
	public enum SortDirection
	{
		Ascending,
		Descending
	}

	public class SortParam
	{
		public SortParam(string sortColumn, SortDirection sortDirection)
		{
			SortColumn = sortColumn;
			SortDirection = sortDirection;
		}

		public string SortColumn { get; }

		public SortDirection SortDirection { get; }
	}
}
