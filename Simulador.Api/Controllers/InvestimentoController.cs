using Microsoft.AspNetCore.Mvc;
using Simulador.Api.Logic.Service;

namespace Simulador.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvestimentoController : ControllerBase
    {
        private readonly IInvestimentoService service;
        public InvestimentoController(IInvestimentoService service) => this.service = service;

        public record InvestimentoDto(int Id, string Tipo, decimal Valor, decimal Rentabilidade, DateTime Data);

        [HttpGet("/investimentos/{clienteId}")]
        public async Task<ActionResult<IEnumerable<InvestimentoDto>>> GetHistoricoInvestimentos(int clienteId)
        {
            var investimentos = await service.ObterHistoricoAsync(clienteId);
            return Ok(investimentos);
        }
    }
}
