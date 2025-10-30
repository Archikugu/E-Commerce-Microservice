using MediatR;

namespace MultiShop.Order.Application.Features.Mediator.Commands.OrderingCommands;

public class UpdateOrderingStatusCommand : IRequest
{
    public int OrderingId { get; }
    public string Status { get; }

    public UpdateOrderingStatusCommand(int orderingId, string status)
    {
        OrderingId = orderingId;
        Status = status;
    }
}


