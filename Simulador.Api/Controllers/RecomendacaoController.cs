using Microsoft.AspNetCore.Mvc;
using Simulador.Api.Logic.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Controlador responsável por gerenciar recomendações de produtos de investimento.
/// </summary>
[ApiController]
[Route("[controller]")]
public class RecomendacoesController : ControllerBase
{
    private readonly IRecomendacaoService _recomendacaoService;

    public RecomendacoesController(IRecomendacaoService recomendacaoService)
    {
        _recomendacaoService = recomendacaoService;
    }

    /// <summary>
    /// Representa um DTO (Data Transfer Object) para um produto recomendado.
    /// </summary>
    public record RecomendacaoProdutoDto(int ProdutoId, string NomeProduto, string TipoProduto, string MotivoRecomendacao);

    /// <summary>
    /// Obtém uma lista de recomendações de produtos de investimento com base no perfil do cliente.
    /// </summary>
    /// <remarks>
    /// Retorna até 3 opções de produtos que se alinham ao perfil de risco e histórico do cliente.
    /// </remarks>
    /// <param name="clienteId">O ID exclusivo do cliente para quem a recomendação se destina.</param>
    /// <returns>Uma lista de objetos RecomendacaoProdutoDto.</returns>
    [HttpGet("/recomendacoes/{clienteId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RecomendacaoProdutoDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<RecomendacaoProdutoDto>>> GetRecomendacoesPorCliente(int clienteId)
    {
        if (clienteId <= 0)
        {
            return BadRequest("O ID do cliente deve ser um valor positivo.");
        }

        try
        {
            var recomendacoes = await _recomendacaoService.GetRecomendacoesPorClienteId(clienteId);

            if (recomendacoes == null || !recomendacoes.Any())
            {
                return NotFound("Nenhuma recomendação encontrada para o cliente.");
            }

            return Ok(recomendacoes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocorreu um erro interno ao buscar as recomendações para o cliente.");
        }
    }
}
