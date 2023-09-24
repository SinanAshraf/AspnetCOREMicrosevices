using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache redisCache;

        public BasketRepository(IDistributedCache distributedCache)
        {
            this.redisCache = distributedCache;
        }

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var basket = await redisCache.GetStringAsync(userName);
            
            if (string.IsNullOrEmpty(basket))
                return null;

            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBusket(ShoppingCart basket)
        {
            await redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));

            return await GetBasket(basket.UserName);
        }
        public async Task DeleteBasket(string userName)
        {
            await redisCache.RemoveAsync(userName);
        }
    }
}
