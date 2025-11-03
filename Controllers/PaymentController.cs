using Microsoft.AspNetCore.Mvc;
using GearShop.Models;
using GearShop.Models.Enums;
using GearShop.Dtos.Payment;
using GearShop.Repositories;

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository paymentRepository;
        private readonly IOrderRepository orderRepository;

        public PaymentController(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository)
        {
            this.paymentRepository = paymentRepository;
            this.orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetAll()
        {
            var payments = await paymentRepository.GetAllAsync();
            return Ok(payments);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Payment>> GetById(int id)
        {
            var payment = await paymentRepository.GetByIdAsync(id);
            if (payment is null)
                return NotFound(new { message = $"Payment with ID {id} not found" });

            return Ok(payment);
        }

        [HttpGet("order/{orderId:int}")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetByOrderId(int orderId)
        {
            var payments = await paymentRepository.GetByOrderIdAsync(orderId);
            return Ok(payments);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetByUserId(int userId)
        {
            var payments = await paymentRepository.GetByUserIdAsync(userId);
            return Ok(payments);
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPendingPayments()
        {
            var payments = await paymentRepository.GetPendingPaymentsAsync();
            return Ok(payments);
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<Payment>> Create([FromBody] CreatePaymentDto dto)
        {
            // Verificar se o pedido existe
            var order = await orderRepository.GetByIdAsync(dto.OrderId);
            if (order is null)
                return BadRequest(new { message = $"Order with ID {dto.OrderId} not found" });

            var payment = new Payment
            {
                OrderId = dto.OrderId,
                PaymentType = dto.PaymentType,
                Status = PaymentStatus.Pending,
                Amount = dto.Amount
            };

            var createdPayment = await paymentRepository.CreateAsync(payment);

            // Simular processamento do pagamento baseado no tipo
            await ProcessPaymentAsync(createdPayment);

            return CreatedAtAction(nameof(GetById), new { id = createdPayment.Id }, createdPayment);
        }

        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        public async Task<ActionResult<Payment>> Update(int id, [FromBody] UpdatePaymentDto dto)
        {
            var paymentToUpdate = new Payment
            {
                Id = id,
                Status = dto.Status,
                ExternalPaymentId = dto.ExternalPaymentId,
                PaymentUrl = dto.PaymentUrl,
                QrCode = dto.QrCode,
                Barcode = dto.Barcode,
                FailureReason = dto.FailureReason,
                LastFourDigits = dto.LastFourDigits,
                CardBrand = dto.CardBrand
            };

            var updatedPayment = await paymentRepository.UpdateAsync(id, paymentToUpdate);
            if (updatedPayment is null)
                return NotFound(new { message = $"Payment with ID {id} not found" });

            return Ok(updatedPayment);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await paymentRepository.DeleteAsync(id);
            return ok ? NoContent() : NotFound(new { message = $"Payment with ID {id} not found" });
        }

        [HttpPost("{id:int}/approve")]
        public async Task<ActionResult<Payment>> ApprovePayment(int id)
        {
            var payment = await paymentRepository.GetByIdAsync(id);
            if (payment is null)
                return NotFound(new { message = $"Payment with ID {id} not found" });

            payment.Status = PaymentStatus.Approved;
            payment.ProcessedAt = DateTime.UtcNow;

            var updatedPayment = await paymentRepository.UpdateAsync(id, payment);

            // Atualizar status do pedido se for um pagamento de pedido
            if (payment.OrderId.HasValue)
            {
                var order = await orderRepository.GetByIdAsync(payment.OrderId.Value);
                if (order is not null)
                {
                    order.Status = OrderStatus.Confirmed;
                    await orderRepository.UpdateAsync(order.Id, order);
                }
            }

            return Ok(updatedPayment);
        }

        [HttpPost("{id:int}/reject")]
        public async Task<ActionResult<Payment>> RejectPayment(int id, [FromBody] string reason)
        {
            var payment = await paymentRepository.GetByIdAsync(id);
            if (payment is null)
                return NotFound(new { message = $"Payment with ID {id} not found" });

            payment.Status = PaymentStatus.Rejected;
            payment.ProcessedAt = DateTime.UtcNow;
            payment.FailureReason = reason;

            var updatedPayment = await paymentRepository.UpdateAsync(id, payment);

            return Ok(updatedPayment);
        }

        private async Task ProcessPaymentAsync(Payment payment)
        {
            // Simulação de processamento de pagamento
            // Em um cenário real, aqui você integraria com APIs de pagamento como:
            // - Stripe, PagSeguro, Mercado Pago, etc.

            await Task.Delay(1000); // Simular processamento

            switch (payment.PaymentType)
            {
                case PaymentType.CreditCard:
                    // Simular processamento de cartão
                    payment.Status = PaymentStatus.Approved;
                    payment.ExternalPaymentId = $"CC_{Guid.NewGuid():N}";
                    payment.LastFourDigits = "1234";
                    payment.CardBrand = "Visa";
                    break;

                case PaymentType.Pix:
                    // Simular geração de PIX
                    payment.Status = PaymentStatus.Pending;
                    payment.PaymentUrl = $"https://pix.example.com/pay/{payment.Id}";
                    payment.QrCode = $"pix-qr-{Guid.NewGuid():N}";
                    break;

                case PaymentType.Boleto:
                    // Simular geração de boleto
                    payment.Status = PaymentStatus.Pending;
                    payment.PaymentUrl = $"https://boleto.example.com/{payment.Id}";
                    payment.Barcode = $"23791{DateTime.Now:yyyyMMdd}{payment.Id:D10}";
                    break;
            }

            await paymentRepository.UpdateAsync(payment.Id, payment);
        }
    }
}
