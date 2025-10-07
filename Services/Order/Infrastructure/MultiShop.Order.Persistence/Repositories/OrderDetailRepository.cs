using MultiShop.Order.Application.Interfaces;
using MultiShop.Order.Domain.Entities;
using MultiShop.Order.Persistence.Context;

namespace MultiShop.Order.Persistence.Repositories;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository(MultiShopOrderContext context) : base(context)
    {
    }
}


