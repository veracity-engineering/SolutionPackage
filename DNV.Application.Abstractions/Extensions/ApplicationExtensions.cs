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
		public static IUnitOfWork CreateUoW<T>(this IUoWProvider provider, bool autoCommit = true) where T : Entity, IAggregateRoot
		{
			return new UnitOfWork(provider, autoCommit);
		}
	}
}
