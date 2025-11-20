using Microsoft.AspNetCore.Mvc;
using Simulador.Api.Logic.Service;

namespace Simulador.Api.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar simulações de investimentos, histórico e dados agregados.
    /// </summary>
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

        /// <summary>
        /// Obtém o histórico completo de todas as simulações já realizadas na plataforma.
        /// </summary>
        /// <returns>Uma lista de objetos HistoricoSimulacaoDto.</returns>
        [HttpGet("/simulacoes")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<HistoricoSimulacaoDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<HistoricoSimulacaoDto>>> GetHistoricoSimulacoes()
        {
            try
            {
                var simulacoes = await service.ObterHistoricoSimulacoesAsync();
                return Ok(simulacoes);
            }
            catch (Exception ex)
            {
                // Logar exceção 'ex' aqui
                return StatusCode(500, "Ocorreu um erro interno ao buscar o histórico de simulações.");
            }
        }

        /// <summary>
        /// Processa uma nova simulação de investimento, calcula o resultado e salva o registro.
        /// </summary>
        /// <param name="request">Os parâmetros necessários para a simulação (ClienteId, Valor, Prazo, TipoProduto).</param>
        /// <returns>O resultado detalhado da simulação, incluindo o produto validado.</returns>
        [HttpPost("/simular-investimento")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SimularInvestimentoResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SimularInvestimentoResponse>> SimularInvestimento([FromBody] SimularInvestimentoRequest request)
        {
            // Validações básicas de entrada
            if (request.ClienteId <= 0 || request.Valor <= 0 || request.PrazoMeses <= 0 || string.IsNullOrEmpty(request.TipoProduto))
            {
                return BadRequest("Dados de requisição inválidos. ClienteId, Valor e PrazoMeses devem ser positivos e TipoProduto não pode ser vazio.");
            }

            try
            {
                var response = await service.ProcessarESalvarSimulacaoAsync(request);
                // Retorna 200 OK com o resultado
                return Ok(response);
                // Alternativamente, se preferir o 201 CreatedAtAction:
                // return CreatedAtAction(nameof(GetHistoricoSimulacoes), new { clienteId = request.ClienteId }, response);
            }
            catch (InvalidOperationException ex)
            {
                // Este catch lida especificamente com a exceção lançada pelo serviço quando o produto não existe
                return NotFound(ex.Message); // Retorna 404 Not Found
            }
            catch (Exception ex)
            {
                // Logar exceção 'ex' aqui
                return StatusCode(500, "Ocorreu um erro interno ao processar a simulação.");
            }
        }

        /// <summary>
        /// Obtém dados agregados das simulações por produto e por dia.
        /// </summary>
        /// <remarks>
        /// Retorna a quantidade de simulações e o valor médio final para cada combinação de produto e data.
        /// </remarks>
        /// <returns>Uma lista de objetos ValoresPorProdutoDiaDto.</returns>
        [HttpGet("/simulacoes/por-produto-dia")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ValoresPorProdutoDiaDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ValoresPorProdutoDiaDto>>> GetValoresPorProdutoDia()
        {
            try
            {
                // O controlador apenas delega a tarefa ao serviço
                var resultados = await service.ObterValoresPorProdutoDiaAsync();
                return Ok(resultados);
            }
            catch (Exception ex)
            {
                // Logar exceção 'ex' aqui
                return StatusCode(500, "Ocorreu um erro interno ao buscar valores agregados.");
            }
        }
    }
}
