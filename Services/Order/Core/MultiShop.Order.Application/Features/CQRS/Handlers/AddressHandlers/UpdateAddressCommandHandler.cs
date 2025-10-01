using MultiShop.Order.Application.Features.CQRS.Commands.AddressCommands;
using MultiShop.Order.Application.Interfaces;
using MultiShop.Order.Domain.Entities;

namespace MultiShop.Order.Application.Features.CQRS.Handlers.AddressHandlers;

public class UpdateAddressCommandHandler
{
    private readonly IRepository<Address> _addressRepository;

    public UpdateAddressCommandHandler(IRepository<Address> addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task Handle(UpdateAddressCommand command)
    {
        var values = await _addressRepository.GetByIdAsync(command.AddressId);
        values.UserId = command.UserId;
        values.FirstName = command.FirstName;
        values.LastName = command.LastName;
        values.Email = command.Email;
        values.PhoneNumber = command.PhoneNumber;
        values.Country = command.Country;
        values.Description = command.Description;
        values.ZipCode = command.ZipCode;
        values.City = command.City;
        values.District = command.District;
        values.AddressLine1 = command.AddressLine1;
        values.AddressLine2 = command.AddressLine2;
        await _addressRepository.UpdateAsync(values);
    }
}