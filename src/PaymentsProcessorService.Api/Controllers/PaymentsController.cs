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
        /// Processa um pagamento.
        /// </summary>
        /// <param name="dto">Dados do pagamento a ser processado</param>
        /// <returns>Resultado do processamento do pagamento</returns>
        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] PaymentProcessInputDto dto)
        {
            return Ok(await _paymentService.ProcessAsync(dto));
        }

        /// <summary>
        /// Obt�m os detalhes de um pagamento pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador do pagamento</param>
        /// <returns>Dados do pagamento</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            return Ok(await _paymentService.GetByIdAsync(id));
        }

        /// <summary>
        /// Obt�m todos os pagamentos realizados por um usu�rio.
        /// </summary>
        /// <param name="userId">Identificador do usu�rio</param>
        /// <returns>Lista de pagamentos do usu�rio</returns>
        [HttpGet("get-by-user/{userId}")]
        public async Task<IActionResult> GetAllByUser([FromRoute] int userId)
        {
            return Ok(await _paymentService.GetAllByUserAsync(userId));
        }
    }
}
