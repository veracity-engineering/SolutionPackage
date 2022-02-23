using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DNV.Application.Abstractions.UoW;
using DNVGL.Domain.Seedwork;

namespace DNV.Application.Abstractions.Extensions
{
	public static class ApplicationExtensions
	{
		public static IUnitOfWork CreateUoW(this IServiceProvider serviceProvider, bool autoCommit = true)
		{
			return new UnitOfWork(serviceProvider, autoCommit);
		}
	}
}
