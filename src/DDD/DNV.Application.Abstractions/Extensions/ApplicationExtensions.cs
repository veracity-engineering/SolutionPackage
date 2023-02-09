using System;
using DNV.Application.Abstractions.UoW;

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
