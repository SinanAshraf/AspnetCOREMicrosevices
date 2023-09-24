using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;
        private readonly ILogger<UpdateOrderCommandHandler> logger;

        public UpdateOrderCommandHandler(
            IOrderRepository orderRepository,
            IMapper mapper, 
            ILogger<UpdateOrderCommandHandler> logger
        )
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await orderRepository.GetByIdAsync(request.Id);
            if(orderToUpdate is null)
            {
                logger.LogError("Order not exists on database.");
                throw new NotFoundException(nameof(Order), orderToUpdate.Id);
            }

            mapper.Map(request, orderToUpdate, typeof(UpdateOrderCommand), typeof(Order));

            await orderRepository.UpdateAsync(orderToUpdate);

            logger.LogInformation($"Order {orderToUpdate.Id} is successfully updated.");

            return Unit.Value;
        }
    }
}
