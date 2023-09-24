using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Ordering.Application.Features.Orders.Queries.GetOrdersList
{
    public class GetOrderListQueryHandler : IRequestHandler<GetOrdersListQuery, List<OrdersVm>>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;

        public GetOrderListQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
        }

        public async Task<List<OrdersVm>> Handle(GetOrdersListQuery request, 
            CancellationToken cancellationToken)
        {
            var orderList = await orderRepository.GetOrdersByUserName(request.UserName);
            return mapper.Map<List<OrdersVm>>(orderList);
        }
    }
}
