using Microsoft.AspNetCore.Mvc;
using Simulador.Api.Logic.Exceptions;
using Simulador.Api.Logic.Service;

namespace Simulador.Api.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar dados e histórico de investimentos de clientes.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class InvestimentoController : ControllerBase
    {
        private readonly IInvestimentoService service;
        public InvestimentoController(IInvestimentoService service) => this.service = service;

        /// <summary>
        /// Representa um DTO (Data Transfer Object) para os detalhes de um investimento.
        /// </summary>
        public record InvestimentoDto(int Id, string Tipo, decimal Valor, decimal Rentabilidade, DateTime Data);

        /// <summary>
        /// Obtém o histórico completo de investimentos de um cliente específico.
        /// </summary>
        /// <remarks>
        /// Retorna todos os investimentos já realizados pelo cliente, se existirem.
        /// </remarks>
        /// <param name="clienteId">O ID exclusivo do cliente.</param>
        /// <returns>Uma lista de objetos InvestimentoDto.</returns>
        [HttpGet("/investimentos/{clienteId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<InvestimentoDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<InvestimentoDto>>> GetHistoricoInvestimentos(int clienteId)
        {
            // 1. Validação Simples (ex: ID negativo)
            if (clienteId <= 0)
            {
                return BadRequest("O ID do cliente deve ser um valor positivo.");
            }

            try
            {
                var investimentos = await service.ObterHistoricoAsync(clienteId);

                // 2. Tratamento de Cenário Específico (se o serviço retornar null ou lista vazia)
                // Se preferir que o serviço lance uma exceção, trate-a aqui no catch
                if (investimentos == null || !investimentos.Any())
                {
                    // Retorna 404 Not Found se não houver dados para o cliente específico
                    return NotFound($"Nenhum histórico de investimento encontrado para o cliente ID {clienteId}.");
                }

                return Ok(investimentos);
            }
            // 3. Tratamento de Exceções de Domínio (erros de validação específicos)
            catch (SimuladorValidationException ex)
            {
                return BadRequest(ex.Message); // Retorna 400 Bad Request
            }
            // 4. Tratamento de Exceções Genéricas (erros de sistema/BD)
            catch (Exception ex)
            {
                // Em um ambiente de produção, você logaria o erro completo aqui
                return StatusCode(500, "Ocorreu um erro interno ao processar a solicitação.");

            }
        }
    }
}
