using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private readonly ICatalogService catalogService;
        private readonly IBasketService basketService;
        private readonly IOrderService orderService;

        public ShoppingController(
            ICatalogService catalogService, 
            IBasketService basketService, 
            IOrderService orderService
        )
        {
            this.catalogService = catalogService;
            this.basketService = basketService;
            this.orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<ShoppingModel>> GetShopping(string userName)
        {
            var basket = await basketService.GetBasket(userName);

            foreach (var item in basket.Items)
            {
                var product = await catalogService.GetCatalog(item.ProductId);

                item.ProductName = product.Name;
                item.Category = product.Category;
                item.Summary = product.Summary;
                item.Description = product.Description;
                item.ImageFile = product.ImageFile;
            }

            var orders = await orderService.GetOrderByUserName(userName);

            var shoppingModel = new ShoppingModel
            {
                UserName = userName,
                BasketWithProducts = basket,
                Orders = orders
            };

            return Ok(shoppingModel);
        }
    }
}
