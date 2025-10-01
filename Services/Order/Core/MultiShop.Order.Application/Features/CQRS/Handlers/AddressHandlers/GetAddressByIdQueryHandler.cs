using MultiShop.Order.Application.Features.CQRS.Queries.AddressQueries;
using MultiShop.Order.Application.Features.CQRS.Results.AddressResults;
using MultiShop.Order.Application.Interfaces;
using MultiShop.Order.Domain.Entities;

namespace MultiShop.Order.Application.Features.CQRS.Handlers.AddressHandlers;

public class GetAddressByIdQueryHandler
{
    private readonly IRepository<Address> _addressRepository;

    public GetAddressByIdQueryHandler(IRepository<Address> addressRepository)
    {
        _addressRepository = addressRepository;
    }
    public async Task<GetAddressByIdQueryResult> Handle(GetAddressByIdQuery query)
    {
        var values = await _addressRepository.GetByIdAsync(query.Id);

        return new GetAddressByIdQueryResult
        {
            AddressId = values.AddressId,
            UserId = values.UserId,
            FirstName = values.FirstName,
            LastName = values.LastName,
            Email = values.Email,
            PhoneNumber = values.PhoneNumber,
            Country = values.Country,
            Description = values.Description,
            ZipCode = values.ZipCode,
            City = values.City,
            District = values.District,
            AddressLine1 = values.AddressLine1,
            AddressLine2 = values.AddressLine2,
        };
    }
}
