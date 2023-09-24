using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient client;
        public OrderService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<IEnumerable<OrderResponseModel>> GetOrderByUserName(string userName)
        {
            var response = await client.GetAsync($"/api/v1/Order/{userName}");
            return await response.ReadContentAs<List<OrderResponseModel>>();
        }
    }
}
