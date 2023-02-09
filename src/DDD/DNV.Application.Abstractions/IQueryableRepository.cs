using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DNVGL.Common.Core.Pagination;
using DNVGL.Common.Core.Sorting;
using DNVGL.Domain.Seedwork;

namespace DNV.Application.Abstractions
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IQueryableRepository<T> where T : Entity
	{
		Task<PaginatedResult<T>> QueryAsync(Expression<Func<T, bool>> predict, PageParam pageParam,
			SortParam? sortParam = null, CancellationToken cancellationToken = default);
	}
}
