using MediatR;
using MultiShop.Order.Application.Features.Mediator.Commands.OrderingCommands;
using MultiShop.Order.Application.Interfaces;
using MultiShop.Order.Domain.Entities;

namespace MultiShop.Order.Application.Features.Mediator.Handlers.OrderingHandlers
{
    public class CreateOrderingCommandHandler : IRequestHandler<CreateOrderingCommand, int>
    {
        private readonly IRepository<Ordering> _orderingRepository;

        public CreateOrderingCommandHandler(IRepository<Ordering> orderingRepository)
        {
            _orderingRepository = orderingRepository;
        }

        public async Task<int> Handle(CreateOrderingCommand request, CancellationToken cancellationToken)
        {
            var entity = new Ordering
            {
                OrderDate = request.OrderDate,
                TotalPrice = request.TotalPrice,
                UserId = request.UserId,
                Status = "New",
            };
            await _orderingRepository.CreateAsync(entity);
            return entity.OrderingId;
        }
    }
}
