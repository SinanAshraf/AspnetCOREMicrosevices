using AutoMapper;
using EventBusMessages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.EventBusConsumer
{
    public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        private readonly ILogger<BasketCheckoutConsumer> logger;
        public BasketCheckoutConsumer(IMapper mapper, IMediator mediator, ILogger<BasketCheckoutConsumer> logger)
        {
            this.mapper = mapper;
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            var command = mapper.Map<CheckoutOrderCommand>(context.Message);
            var result = await mediator.Send(command);

            logger.LogInformation("Success");
        }
    }
}
