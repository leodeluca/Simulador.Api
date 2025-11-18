using Microsoft.AspNetCore.Mvc;
using Simulador.Api.Logic.Service;

namespace Simulador.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SimuladorController : ControllerBase
    {

        private readonly ISimuladorService service;
        public SimuladorController(ISimuladorService service) => this.service = service;

        public record SimularInvestimentoRequest(int ClienteId, decimal Valor, int PrazoMeses, string TipoProduto);
        public record ProdutoValidadoDto(int Id, string Nome, string Tipo, decimal Rentabilidade, string Risco);
        public record ValoresPorProdutoDiaDto(string Produto, string Data, int QuantidadeSimulacoes, decimal MediaValorFinal);
        public record ResultadoSimulacaoDto(decimal ValorFinal, decimal RentabilidadeEfetiva, int PrazoMeses);
        public record SimularInvestimentoResponse(ProdutoValidadoDto ProdutoValidado, ResultadoSimulacaoDto ResultadoSimulacao, DateTime DataSimulacao);
        public record HistoricoSimulacaoDto(int Id, int ClienteId, string Produto, decimal ValorInvestido, decimal ValorFinal, int PrazoMeses, DateTime DataSimulacao);

        [HttpGet("/simulacoes")]
        public async Task<ActionResult<IEnumerable<HistoricoSimulacaoDto>>> GetHistoricoSimulacoes()
        {
            var simulacoes = await service.ObterHistoricoSimulacoesAsync();
            return Ok(simulacoes);
        }

        [HttpPost("/simular-investimento")]
        public async Task<ActionResult<SimularInvestimentoResponse>> SimularInvestimento([FromBody] SimularInvestimentoRequest request)
        {
            var response = await service.ProcessarESalvarSimulacaoAsync(request);
            return Ok(response);
        }

        [HttpGet("/simulacoes/por-produto-dia")]
        public async Task<ActionResult<IEnumerable<ValoresPorProdutoDiaDto>>> GetValoresPorProdutoDia()
        {
            // O controlador apenas delega a tarefa ao serviço
            var resultados = await service.ObterValoresPorProdutoDiaAsync();
            return Ok(resultados);
        }
    }
}
