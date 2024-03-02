using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace amazon_backend.Controllers
{
    [Route("orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderDao orderDao;
        public OrderController(OrderDao orderDao)
        {
            this.orderDao = orderDao;
        }
        [HttpGet]
        public Order[] GetOrders()
        {
            return orderDao.GetAll();
        }
        [HttpPost]
        public Order CreateOrder()
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                orderKey = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                Quantity = 2,
                TotalPrice = 100,
                Status = "Processing",
                CreatedAt = DateTime.Now
            };
            orderDao.Add(order);
            return order;
        }
        [HttpGet]
        [Route("{id}")]
        public Results<NotFound, Ok<Order>> GetOrderById(string id)
        {
            Guid orderId;
            try
            {
                orderId = Guid.Parse(id);
            }
            catch
            {
                return TypedResults.NotFound();
            }
            Order? order = orderDao.GetById(orderId);
            if (order is not null) return TypedResults.Ok(order);
            return TypedResults.NotFound();
        }
        [HttpPut]
        [Route("/restore-order/{id}")]
        public IActionResult RestoreOrder(string id)
        {
            Guid orderId;
            try
            {
                orderId = Guid.Parse(id);
            }
            catch
            {
                return StatusCode(500);
            }
            orderDao.Restore(orderId);
            return Ok();
        }
        [HttpDelete]
        [Route("/delete-order/{id}")]
        public IActionResult DeleteOrder(string id)
        {
            Guid orderId;
            try
            {
                orderId = Guid.Parse(id);
            }
            catch
            {
                return StatusCode(500);
            }
            orderDao.Delete(orderId);
            return Ok();
        }
    }
}