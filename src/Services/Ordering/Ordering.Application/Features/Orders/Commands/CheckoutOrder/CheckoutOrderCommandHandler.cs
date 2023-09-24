using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;
        private readonly IEmailService emailService;
        private readonly ILogger<CheckoutOrderCommandHandler> logger;

        public CheckoutOrderCommandHandler(
            IOrderRepository orderRepository,
            IMapper mapper,
            IEmailService emailService,
            ILogger<CheckoutOrderCommandHandler> logger
        )
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
            this.emailService = emailService;
            this.logger = logger;
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = mapper.Map<Order>(request);
            var newOrder = await orderRepository.AddAsync(orderEntity);

            logger.LogInformation($"Order {newOrder.Id} is successfully created.");

            await SendMail(newOrder);

            return newOrder.Id;
        }

        private async Task SendMail(Order newOrder)
        {
            var email = new Email
            {
                To = "test@test.com",
                Body = "Order was created",
                Subject = "Order creation"
            };

            try
            {
                await emailService.SendEmail(email);
            }
            catch(Exception ex)
            {
                logger.LogError($"Order {newOrder.Id} failed due to an error with the mail service: {ex.Message}");
            }
        }
    }
}
