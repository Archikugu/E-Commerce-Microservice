﻿using MultiShop.Order.Application.Features.CQRS.Commands.OrderDetailCommands;
using MultiShop.Order.Application.Interfaces;
using MultiShop.Order.Domain.Entities;

namespace MultiShop.Order.Application.Features.CQRS.Handlers.OrderDetailHandlers;

public class UpdateOrderDetailCommandHandler
{
    private readonly IRepository<OrderDetail> _orderDetailRepository;

    public UpdateOrderDetailCommandHandler(IRepository<OrderDetail> orderDetailRepository)
    {
        _orderDetailRepository = orderDetailRepository;
    }
    public async Task Handle(UpdateOrderDetailCommand command)
    {
        var values = await _orderDetailRepository.GetByIdAsync(command.OrderDetailId);

        values.OrderingId = command.OrderingId;
        values.ProductId = command.ProductId;
        values.ProductPrice = command.ProductPrice;
        values.ProductName = command.ProductName;
        values.ProductTotalPrice = command.ProductTotalPrice;
        values.ProductAmount = command.ProductAmount;

        await _orderDetailRepository.UpdateAsync(values);
    }
}