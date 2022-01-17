﻿using MediatR;

namespace DNV.Application.CQRS.Abstractions
{
	public interface ICommand<out TResponse> : IRequest<TResponse>
	{
	}

	public interface ICommand : IRequest<Unit>
	{
	}

}
