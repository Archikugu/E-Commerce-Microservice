using MultiShop.Order.Application.Features.CQRS.Commands.OrderDetailCommands;
using MultiShop.Order.Application.Interfaces;
using MultiShop.Order.Domain.Entities;

namespace MultiShop.Order.Application.Features.CQRS.Handlers.OrderDetailHandlers;

public class RemoveOrderDetailCommandHandler
{
    private readonly IRepository<OrderDetail> _orderDetailRepository;

    public RemoveOrderDetailCommandHandler(IRepository<OrderDetail> orderDetailRepository)
    {
        _orderDetailRepository = orderDetailRepository;
    }
    public async Task Handle(RemoveOrderDetailCommand command)
    {
        var values = await _orderDetailRepository.GetByIdAsync(command.Id);
        await _orderDetailRepository.DeleteAsync(values);
    }
}
