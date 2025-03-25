using MultiShop.Order.Application.Features.CQRS.Results.AddressResults;
using MultiShop.Order.Application.Interfaces;
using MultiShop.Order.Domain.Entities;

namespace MultiShop.Order.Application.Features.CQRS.Handlers.AddressHandlers;

public class GetAddressQueryHandler
{
    private readonly IRepository<Address> _addressRepository;
    public GetAddressQueryHandler(IRepository<Address> addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task<List<GetAddressQueryResult>> Handle()
    {
        var values = await _addressRepository.GetAllAsync();
        return values.Select(x => new GetAddressQueryResult
        {
            AddressId = x.AddressId,
            City = x.City,
            Detail = x.Detail,
            District = x.District,
            UserId = x.UserId,
        }).ToList();
    }
}