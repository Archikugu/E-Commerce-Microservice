using MultiShop.Order.Application.Features.CQRS.Commands.AddressCommands;
using MultiShop.Order.Application.Interfaces;
using MultiShop.Order.Domain.Entities;

namespace MultiShop.Order.Application.Features.CQRS.Handlers.AddressHandlers;
public class CreateAddressCommandHandler
{
    private readonly IRepository<Address> _addressRepository;

    public CreateAddressCommandHandler(IRepository<Address> addressRepository)
    {
        _addressRepository = addressRepository;
    }
    public async Task Handle(CreateAddressCommand createAddressCommand)
    {
        await _addressRepository.CreateAsync(new Address
        {
            UserId = createAddressCommand.UserId,
            FirstName = createAddressCommand.FirstName,
            LastName = createAddressCommand.LastName,
            Email = createAddressCommand.Email,
            PhoneNumber = createAddressCommand.PhoneNumber,
            Country = createAddressCommand.Country,
            Description = createAddressCommand.Description,
            ZipCode = createAddressCommand.ZipCode,
            City = createAddressCommand.City,
            District = createAddressCommand.District,
            AddressLine1 = createAddressCommand.AddressLine1,
            AddressLine2 = createAddressCommand.AddressLine2,
        });
    }
}
