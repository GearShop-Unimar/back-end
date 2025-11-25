using GearShop.Dtos.Premium;
using GearShop.Services.Premium;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using GearShop.Dtos.Payment;

namespace GearShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PremiumAccountController : ControllerBase
    {
        private readonly IPremiumAccountService _premiumAccountService;

        public PremiumAccountController(IPremiumAccountService premiumAccountService)
        {
            _premiumAccountService = premiumAccountService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) throw new UnauthorizedAccessException("User not authenticated.");
            return int.Parse(userIdClaim.Value);
        }

        [Authorize]
        [HttpPost("activate")]
        public async Task<IActionResult> ActivatePremium([FromBody] ActivatePremiumDto activatePremiumDto)
        {
            try
            {
                var userId = GetUserId();
                var premiumAccount = await _premiumAccountService.ActivatePremiumAsync(userId, activatePremiumDto.DurationDays, 15.99m);
                return Ok(premiumAccount);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("status")]
        public async Task<IActionResult> GetPremiumStatus()
        {
            try
            {
                var userId = GetUserId();
                var isPremium = await _premiumAccountService.IsUserPremiumAsync(userId);
                return Ok(new { IsPremium = isPremium });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("payment-history")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentHistory()
        {
            try
            {
                var userId = GetUserId();
                var paymentHistory = await _premiumAccountService.GetPaymentHistoryAsync(userId);
                return Ok(paymentHistory);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Rota de detalhes removida: usar o endpoint /status para verificar se o usuário é premium

        [Authorize]
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelPremium()
        {
            try
            {
                var userId = GetUserId();
                await _premiumAccountService.CancelPremiumAsync(userId);
                return Ok("Premium account cancelled successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
