using Microsoft.AspNetCore.Mvc;
using GearShop.Models;
using GearShop.Models.Enums;
using GearShop.Dtos.Order;
using GearShop.Repositories;
using System.ComponentModel.DataAnnotations;

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPaymentRepository _paymentRepository;

        public OrderController(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IPaymentRepository paymentRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _paymentRepository = paymentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var orders = await _orderRepository.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(id);
            if (order is null)
                return NotFound(new { message = $"Order with ID {id} not found" });

            return Ok(order);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetByUserId(int userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<Order>> Create([FromBody] CreateOrderDto dto)
        {
            // Validar se o usuário existe (simulação - você pode implementar validação real)
            if (dto.UserId <= 0)
                return BadRequest(new { message = "User ID is required" });

            // Validar produtos e calcular total
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var itemDto in dto.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product is null)
                    return BadRequest(new { message = $"Product with ID {itemDto.ProductId} not found" });

                if (product.StockQuantity < itemDto.Quantity)
                    return BadRequest(new { message = $"Insufficient stock for product {product.Name}" });

                var unitPrice = product.Price;
                var totalPrice = unitPrice * itemDto.Quantity;
                totalAmount += totalPrice;

                orderItems.Add(new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice
                });
            }

            // Criar o pedido
            var order = new Order
            {
                UserId = dto.UserId,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending,
                Notes = dto.Notes,
                OrderItems = orderItems
            };

            var createdOrder = await _orderRepository.CreateAsync(order);

            // Criar pagamento baseado no tipo selecionado
            var payment = new Payment
            {
                OrderId = createdOrder.Id,
                PaymentType = dto.PaymentType,
                Status = PaymentStatus.Pending,
                Amount = totalAmount
            };

            await _paymentRepository.CreateAsync(payment);

            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        public async Task<ActionResult<Order>> Update(int id, [FromBody] UpdateOrderDto dto)
        {
            var orderToUpdate = new Order
            {
                Id = id,
                Status = dto.Status,
                Notes = dto.Notes
            };

            var updatedOrder = await _orderRepository.UpdateAsync(id, orderToUpdate);
            if (updatedOrder is null)
                return NotFound(new { message = $"Order with ID {id} not found" });

            return Ok(updatedOrder);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _orderRepository.DeleteAsync(id);
            return ok ? NoContent() : NotFound(new { message = $"Order with ID {id} not found" });
        }
    }
}
