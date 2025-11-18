using Microsoft.AspNetCore.Mvc;
using Simulador.Api.Logic.Service;

namespace Simulador.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PerfilRiscoController : ControllerBase
    {
        private readonly IPerfilRiscoService _perfilRiscoService;
        private readonly IRecomendacaoService _recomendacaoService;
        public PerfilRiscoController(IPerfilRiscoService perfilRiscoService, IRecomendacaoService recomendacaoService)
        {
            _perfilRiscoService = perfilRiscoService;
            _recomendacaoService = recomendacaoService; // Atribua o serviço injetado
        }

        // DTO para o response (record)
        public record PerfilRiscoDto(int ClienteId, string Perfil, int Pontuacao, string Descricao);
        public record ProdutoRecomendadoDto(int Id, string Nome, string Tipo, decimal Rentabilidade, string Risco);

        // GET /perfil-risco/{clienteId}
        [HttpGet("/perfil-risco/{clienteId}")]
        public async Task<ActionResult<PerfilRiscoDto>> GetPerfilRisco(int clienteId)
        {
            var perfil = await _perfilRiscoService.ObterPerfilRiscoAsync(clienteId);

            if (perfil == null)
            {
                return NotFound($"Cliente com ID {clienteId} não encontrado ou sem perfil.");
            }

            return Ok(perfil);
        }

        [HttpGet("/produtos-recomendados/{perfil}")]
        public async Task<ActionResult<IEnumerable<ProdutoRecomendadoDto>>> GetProdutosRecomendados(string perfil)
        {
            var produtos = await _recomendacaoService.ObterProdutosRecomendadosAsync(perfil);
            return Ok(produtos);
        }
    }
}
