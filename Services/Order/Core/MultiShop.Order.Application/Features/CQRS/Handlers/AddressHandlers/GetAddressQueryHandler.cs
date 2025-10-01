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
            UserId = x.UserId,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            PhoneNumber = x.PhoneNumber,
            Country = x.Country,
            Description = x.Description,
            ZipCode = x.ZipCode,
            City = x.City,
            District = x.District,
            AddressLine1 = x.AddressLine1,
            AddressLine2 = x.AddressLine2,
        }).ToList();
    }
}