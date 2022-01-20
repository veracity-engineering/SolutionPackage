using MediatR;

namespace DNV.Application.CQRS.Abstractions
{

	public interface IQueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
		where TRequest : IQuery<TResponse>
	{
	}
}