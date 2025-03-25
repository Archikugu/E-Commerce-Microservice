using MultiShop.Order.Application.Features.CQRS.Commands.AddressCommands;
using MultiShop.Order.Application.Interfaces;
using MultiShop.Order.Domain.Entities;

namespace MultiShop.Order.Application.Features.CQRS.Handlers.AddressHandlers;

public class RemoveAddressCommandHandler
{
    private readonly IRepository<Address> _addressRepository;

    public RemoveAddressCommandHandler(IRepository<Address> addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task Handle(RemoveAddressCommand command)
    {
        var values = await _addressRepository.GetByIdAsync(command.Id);
        await _addressRepository.DeleteAsync(values);
    }
}