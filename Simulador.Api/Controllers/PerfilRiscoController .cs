using Microsoft.AspNetCore.Mvc;
using Simulador.Api.Logic.Service;

namespace Simulador.Api.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar perfis de risco de clientes e recomendações de produtos associadas.
    /// </summary>
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

        /// <summary>
        /// Representa o DTO (Data Transfer Object) para o perfil de risco de um cliente.
        /// </summary>
        public record PerfilRiscoDto(int ClienteId, string Perfil, int Pontuacao, string Descricao);

        /// <summary>
        /// Representa o DTO para um produto recomendado com base no perfil.
        /// </summary>
        public record ProdutoRecomendadoDto(int Id, string Nome, string Tipo, decimal Rentabilidade, string Risco);


        // GET /perfil-risco/{clienteId}
        /// <summary>
        /// Obtém o perfil de risco e a pontuação de um cliente específico.
        /// </summary>
        /// <remarks>
        /// Fornece o nome do perfil (ex: Moderado, Agressivo), a pontuação numérica e uma descrição detalhada.
        /// </remarks>
        /// <param name="clienteId">O ID exclusivo do cliente.</param>
        /// <returns>Os detalhes do PerfilRiscoDto.</returns>
        [HttpGet("/perfil-risco/{clienteId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PerfilRiscoDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                    return NotFound($"Cliente com ID {clienteId} não encontrado ou sem perfil.");
                }

                return Ok(perfil);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocorreu um erro interno ao buscar o perfil de risco.");
            }
        }

        /// <summary>
        /// Lista produtos recomendados com base em um perfil de risco específico (Conservador, Moderado, Agressivo).
        /// </summary>
        /// <remarks>
        /// Filtra o catálogo de produtos com base no nível de risco aceitável para o perfil fornecido.
        /// </remarks>
        /// <param name="perfil">O nome do perfil de risco (e.g., 'Moderado', 'Agressivo').</param>
        /// <returns>Uma lista de objetos ProdutoRecomendadoDto.</returns>
        [HttpGet("/produtos-recomendados/{perfil}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProdutoRecomendadoDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocorreu um erro interno ao buscar produtos recomendados.");
            }
        }
    }
}
