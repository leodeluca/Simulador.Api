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
    public async Task<ActionResult<List<RecomendacaoProdutoDto>>> GetRecomendacoesPorCliente(int clienteId)
    {
        var recomendacoes = await _recomendacaoService.GetRecomendacoesPorClienteId(clienteId);

        if (recomendacoes == null || !recomendacoes.Any())
        {
            return NotFound("Nenhuma recomendação encontrada para o cliente.");
        }

        return Ok(recomendacoes);
    }
}
