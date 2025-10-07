using MediatR;
using MultiShop.Order.Application.Features.Mediator.Queries.OrderingQueries;
using MultiShop.Order.Application.Features.Mediator.Results.OrderingResults;
using MultiShop.Order.Application.Interfaces;
using MultiShop.Order.Domain.Entities;

namespace MultiShop.Order.Application.Features.Mediator.Handlers.OrderingHandlers;

public class GetOrderingByUserIdQueryHandler : IRequestHandler<GetOrderingByUserIdQuery, List<GetOrderingByUserIdQueryResult>>
{
    private readonly IOrderingRepository _orderingRepository;

    public GetOrderingByUserIdQueryHandler(IOrderingRepository orderingRepository)
    {
        _orderingRepository = orderingRepository;
    }

    public async Task<List<GetOrderingByUserIdQueryResult>> Handle(GetOrderingByUserIdQuery request, CancellationToken cancellationToken)
    {
        var values = _orderingRepository.GetOrderingsByUserId(request.Id);
        return values.Select(o => new GetOrderingByUserIdQueryResult
        {
            OrderDate = o.OrderDate,
            OrderingId = o.OrderingId,
            TotalPrice = o.TotalPrice,
            UserId = o.UserId
        }).ToList();
    }
}
