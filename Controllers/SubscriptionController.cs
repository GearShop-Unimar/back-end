using Microsoft.AspNetCore.Mvc;
using GearShop.Models;
using GearShop.Models.Enums;
using GearShop.Dtos.Subscription;
using GearShop.Repositories;

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;

        public SubscriptionController(
            ISubscriptionRepository subscriptionRepository,
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetAll()
        {
            var subscriptions = await _subscriptionRepository.GetAllAsync();
            return Ok(subscriptions);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Subscription>> GetById(int id)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id);
            if (subscription is null)
                return NotFound(new { message = $"Subscription with ID {id} not found" });

            return Ok(subscription);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetByUserId(int userId)
        {
            var subscriptions = await _subscriptionRepository.GetByUserIdAsync(userId);
            return Ok(subscriptions);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetActiveSubscriptions()
        {
            var subscriptions = await _subscriptionRepository.GetActiveSubscriptionsAsync();
            return Ok(subscriptions);
        }

        [HttpGet("due")]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetDueSubscriptions()
        {
            var subscriptions = await _subscriptionRepository.GetDueSubscriptionsAsync();
            return Ok(subscriptions);
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<Subscription>> Create([FromBody] CreateSubscriptionDto dto)
        {
            // Verificar se o usuário já tem uma assinatura ativa para este produto
            if (await _subscriptionRepository.UserHasActiveSubscriptionAsync(dto.UserId, dto.ProductId))
                return Conflict(new { message = "User already has an active subscription for this product" });

            var subscription = new Subscription
            {
                UserId = dto.UserId,
                ProductId = dto.ProductId,
                Status = SubscriptionStatus.Active,
                MonthlyAmount = dto.MonthlyAmount,
                StartDate = DateTime.UtcNow,
                Notes = dto.Notes
            };

            var createdSubscription = await _subscriptionRepository.CreateAsync(subscription);

            // Processar primeiro pagamento
            await ProcessSubscriptionPaymentAsync(createdSubscription, dto.PaymentType);

            return CreatedAtAction(nameof(GetById), new { id = createdSubscription.Id }, createdSubscription);
        }

        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        public async Task<ActionResult<Subscription>> Update(int id, [FromBody] UpdateSubscriptionDto dto)
        {
            var subscriptionToUpdate = new Subscription
            {
                Id = id,
                Status = dto.Status,
                MonthlyAmount = dto.MonthlyAmount ?? 0,
                EndDate = dto.EndDate,
                Notes = dto.Notes
            };

            var updatedSubscription = await _subscriptionRepository.UpdateAsync(id, subscriptionToUpdate);
            if (updatedSubscription is null)
                return NotFound(new { message = $"Subscription with ID {id} not found" });

            return Ok(updatedSubscription);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _subscriptionRepository.DeleteAsync(id);
            return ok ? NoContent() : NotFound(new { message = $"Subscription with ID {id} not found" });
        }

        [HttpPost("{id:int}/pause")]
        public async Task<ActionResult<Subscription>> PauseSubscription(int id)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id);
            if (subscription is null)
                return NotFound(new { message = $"Subscription with ID {id} not found" });

            subscription.Status = SubscriptionStatus.Paused;
            subscription.UpdatedAt = DateTime.UtcNow;

            var updatedSubscription = await _subscriptionRepository.UpdateAsync(id, subscription);
            return Ok(updatedSubscription);
        }

        [HttpPost("{id:int}/resume")]
        public async Task<ActionResult<Subscription>> ResumeSubscription(int id)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id);
            if (subscription is null)
                return NotFound(new { message = $"Subscription with ID {id} not found" });

            subscription.Status = SubscriptionStatus.Active;
            subscription.UpdatedAt = DateTime.UtcNow;

            var updatedSubscription = await _subscriptionRepository.UpdateAsync(id, subscription);
            return Ok(updatedSubscription);
        }

        [HttpPost("{id:int}/cancel")]
        public async Task<ActionResult<Subscription>> CancelSubscription(int id)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id);
            if (subscription is null)
                return NotFound(new { message = $"Subscription with ID {id} not found" });

            subscription.Status = SubscriptionStatus.Cancelled;
            subscription.EndDate = DateTime.UtcNow;
            subscription.UpdatedAt = DateTime.UtcNow;

            var updatedSubscription = await _subscriptionRepository.UpdateAsync(id, subscription);
            return Ok(updatedSubscription);
        }

        [HttpPost("process-due")]
        public async Task<IActionResult> ProcessDueSubscriptions()
        {
            var dueSubscriptions = await _subscriptionRepository.GetDueSubscriptionsAsync();
            var processedCount = 0;

            foreach (var subscription in dueSubscriptions)
            {
                // Criar pagamento diretamente para a assinatura
                var payment = new Payment
                {
                    SubscriptionId = subscription.Id,
                    PaymentType = PaymentType.Pix, // Assumindo PIX como padrão para recorrência
                    Status = PaymentStatus.Pending,
                    Amount = subscription.MonthlyAmount
                };

                await _paymentRepository.CreateAsync(payment);

                // Atualizar próxima data de pagamento
                subscription.NextPaymentDate = subscription.NextPaymentDate?.AddMonths(1);
                await _subscriptionRepository.UpdateAsync(subscription.Id, subscription);

                processedCount++;
            }

            return Ok(new { message = $"Processed {processedCount} due subscriptions" });
        }

        private async Task ProcessSubscriptionPaymentAsync(Subscription subscription, PaymentType paymentType)
        {
            // Criar pagamento diretamente para a assinatura
            var payment = new Payment
            {
                SubscriptionId = subscription.Id,
                PaymentType = paymentType,
                Status = PaymentStatus.Pending,
                Amount = subscription.MonthlyAmount
            };

            await _paymentRepository.CreateAsync(payment);
        }
    }
}
