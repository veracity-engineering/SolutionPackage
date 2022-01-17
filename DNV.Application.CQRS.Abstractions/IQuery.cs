using MediatR;

namespace DNV.Application.CQRS.Abstractions
{
	public interface IQuery<out TResponse> : IRequest<TResponse>
	{
	}
}