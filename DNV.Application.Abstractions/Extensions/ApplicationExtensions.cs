using System;
using System.Collections.Generic;
using System.Text;
using DNV.Application.Abstractions.UoW;
using DNVGL.Domain.Seedwork;

namespace DNV.Application.Abstractions.Extensions
{
	public static class ApplicationExtensions
	{
		public static IUnitOfWork CreateUoW<T>(this IUoWProvider provider, bool autoCommit = true) where T : IAggregateRoot
		{
			return new UnitOfWork(provider, autoCommit);
		}
	}
}
