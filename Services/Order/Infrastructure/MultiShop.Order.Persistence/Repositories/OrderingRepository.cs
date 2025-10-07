using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiShop.Order.Application.Interfaces;
using MultiShop.Order.Domain.Entities;
using MultiShop.Order.Persistence.Context;

namespace MultiShop.Order.Persistence.Repositories;

public class OrderingRepository : IOrderingRepository
{
    private readonly MultiShopOrderContext _context;

    public OrderingRepository(MultiShopOrderContext context)
    {
        _context = context;
    }

    public List<Ordering> GetOrderingsByUserId(string id)
    {
        var values = _context.Orderings.Where(o => o.UserId == id).ToList();
        return values;
    }
}
