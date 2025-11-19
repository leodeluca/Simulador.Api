using Microsoft.AspNetCore.Mvc;
using Simulador.Api.Logic.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class RecomendacoesController : ControllerBase
{
    private readonly IRecomendacaoService _recomendacaoService;

    public RecomendacoesController(IRecomendacaoService recomendacaoService)
    {
        _recomendacaoService = recomendacaoService;
    }

    public record RecomendacaoProdutoDto(int ProdutoId, string NomeProduto, string TipoProduto, string MotivoRecomendacao);

    // Endpoint: GET /api/recomendacoes/{clienteId}
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
                // Este cenário ocorre se o cliente não existe ou se a lógica do serviço não encontrou produtos
                return NotFound("Nenhuma recomendação encontrada para o cliente.");
            }

            return Ok(recomendacoes);
        }
        catch (Exception ex)
        {
            // Logar exceção 'ex' aqui
            return StatusCode(500, "Ocorreu um erro interno ao buscar as recomendações para o cliente.");
        }
    }
}
