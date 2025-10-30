using MediatR;
using MultiShop.Order.Application.Features.Mediator.Commands.OrderingCommands;
using MultiShop.Order.Application.Interfaces;
using MultiShop.Order.Domain.Entities;

namespace MultiShop.Order.Application.Features.Mediator.Handlers.OrderingHandlers;

public class UpdateOrderingStatusCommandHandler : IRequestHandler<UpdateOrderingStatusCommand>
{
    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    { "New", "Processing", "Shipped", "Delivered", "Cancelled" };

    private readonly IRepository<Ordering> _orderingRepository;
    public UpdateOrderingStatusCommandHandler(IRepository<Ordering> orderingRepository)
    {
        _orderingRepository = orderingRepository;
    }

    public async Task Handle(UpdateOrderingStatusCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Status) || !Allowed.Contains(request.Status))
            throw new ArgumentException("Invalid status value");

        var entity = await _orderingRepository.GetByIdAsync(request.OrderingId);
        if (entity == null) throw new InvalidOperationException("Order not found");
        entity.Status = Allowed.First(s => s.Equals(request.Status, StringComparison.OrdinalIgnoreCase));
        await _orderingRepository.UpdateAsync(entity);
    }
}


