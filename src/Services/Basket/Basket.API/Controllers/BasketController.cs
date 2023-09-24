using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBusMessages.Events;
using Grpc.Core;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository repository;
        private readonly DiscountGrpcService discountGrpcService;
        private readonly IMapper mapper;
        private readonly IPublishEndpoint publishEndpoint;
        public BasketController(
            IBasketRepository repository,
            DiscountGrpcService discountGrpcService,
            IMapper mapper, 
            IPublishEndpoint publishEndpoint)
        {
            this.repository = repository;
            this.discountGrpcService = discountGrpcService;
            this.mapper = mapper;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            return Ok((await repository.GetBasket(userName)) ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart shoppingCart)
        {
            foreach (var item in shoppingCart.Items)
            {
                try
                {
                    var coupon = await discountGrpcService.GetDiscount(item.ProductName);
                    item.Price -= coupon.Amount;
                }
                catch (Exception ex)
                {
                    if (ex is RpcException rEx && rEx.StatusCode != Grpc.Core.StatusCode.NotFound)
                    {
                        throw;
                    }
                }
            }

            return Ok(await repository.UpdateBusket(shoppingCart));
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await repository.DeleteBasket(userName);
            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            var basket = await repository.GetBasket(basketCheckout.UserName);
            if (basket == null)
            {
                return BadRequest();
            }

            var eventMessage = mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            var publishTask = publishEndpoint.Publish(eventMessage);
            var removeTask = repository.DeleteBasket(basketCheckout.UserName);

            await Task.WhenAll(publishTask, removeTask);

            return Accepted();
        }
    }
}
