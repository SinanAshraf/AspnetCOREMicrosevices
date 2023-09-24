using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;
        private readonly ILogger<DeleteOrderHandler> logger;

        public DeleteOrderHandler(
            IOrderRepository orderRepository,
            IMapper mapper,
            ILogger<DeleteOrderHandler> logger
        )
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToDelete = await orderRepository.GetByIdAsync(request.Id);
            if(orderToDelete is null)
            {
                logger.LogError("Order not exists on database.");
                throw new NotFoundException(nameof(Order), orderToDelete.Id);
            }
            await orderRepository.DeleteAsync(orderToDelete);

            logger.LogInformation($"Order {orderToDelete.Id} is successfully deleted.");

            return Unit.Value;
        }
    }
}
