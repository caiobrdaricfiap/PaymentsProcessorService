using Microsoft.AspNetCore.Mvc;
using PaymentsProcessorService.Application.DTOs;
using PaymentsProcessorService.Application.Services;

namespace PaymentsProcessorService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Cadastra um novo usuário.
        /// </summary>
        /// <param name="dto">Dados do usuário a ser cadastrado</param>
        /// <returns>Usuário cadastrado</returns>
        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] PaymentProcessInputDto dto)
        {
            return Ok(await _paymentService.ProcessAsync(dto));
        }

        /// <summary>
        /// Cadastra um novo usuário.
        /// </summary>
        /// <param name="dto">Dados do usuário a ser cadastrado</param>
        /// <returns>Usuário cadastrado</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            return Ok(await _paymentService.GetByIdAsync(id));
        }

        /// <summary>
        /// Cadastra um novo usuário.
        /// </summary>
        /// <param name="userId">Dados do usuário a ser cadastrado</param>
        /// <returns>Usuário cadastrado</returns>
        [HttpGet("get-by-user/{userId}")]
        public async Task<IActionResult> GetAllByUser([FromRoute] int userId)
        {
                return Ok(await _paymentService.GetAllByUserAsync(userId));
        }
    }
}
