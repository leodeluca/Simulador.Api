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
            if (clienteId <= 0)
            {
                return BadRequest("O ID do cliente deve ser um valor positivo.");
            }
            try
            {
                var perfil = await _perfilRiscoService.ObterPerfilRiscoAsync(clienteId);

                if (perfil == null)
                {
                    // Tratamento para cliente não encontrado ou sem perfil
                    return NotFound($"Cliente com ID {clienteId} não encontrado ou sem perfil.");
                }

                return Ok(perfil);
            }
            catch (Exception ex)
            {
                // Logar exceção 'ex' aqui
                return StatusCode(500, "Ocorreu um erro interno ao buscar o perfil de risco.");
            }
        }

        [HttpGet("/produtos-recomendados/{perfil}")]
        public async Task<ActionResult<IEnumerable<ProdutoRecomendadoDto>>> GetProdutosRecomendados(string perfil)
        {
            if (string.IsNullOrWhiteSpace(perfil))
            {
                return BadRequest("O perfil de risco deve ser especificado.");
            }

            try
            {
                var produtos = await _recomendacaoService.ObterProdutosRecomendadosAsync(perfil);
                return Ok(produtos);
            }
            catch (ArgumentException ex)
            {
                // Retorna 400 Bad Request com a mensagem de erro detalhada do serviço
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Logar exceção 'ex' aqui
                return StatusCode(500, "Ocorreu um erro interno ao buscar produtos recomendados.");
            }
        }
    }
}
