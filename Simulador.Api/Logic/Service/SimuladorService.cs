using Simulador.Api.Logic.Exceptions;
using Simulador.Api.Logic.Repositories;
using Simulador.Api.Models;
using System.Linq;
using static Simulador.Api.Controllers.SimuladorController;

namespace Simulador.Api.Logic.Service
{
    public class SimuladorService : ISimuladorService
    {
        private readonly ISimuladorRepository _simuladorRepository;
        private readonly IProdutoRepository _produtoRepository;
        public SimuladorService(ISimuladorRepository simuladorRepository, IProdutoRepository produtoRepository) 
        {
            _simuladorRepository = simuladorRepository;
            _produtoRepository = produtoRepository;
        }

        public async Task<SimularInvestimentoResponse> ProcessarESalvarSimulacaoAsync(SimularInvestimentoRequest request)
        {
            var produtoReal = await _produtoRepository.GetByTipoAsync(request.TipoProduto); // Exemplo de um método que você criaria

            if (produtoReal == null)
            {
                // Trate o caso em que o produto não é encontrado
                throw new InvalidOperationException($"Produto do tipo '{request.TipoProduto}' não encontrado.");
            }

            var produtoValidado = new ProdutoValidadoDto(
                                produtoReal.Id,
                                produtoReal.Nome,
                                produtoReal.Tipo,
                                produtoReal.Rentabilidade,
                                produtoReal.Risco
                                );

            decimal valorFinalCalculado = request.Valor * (1 + produtoReal.Rentabilidade * (request.PrazoMeses / 12m));

            var resultadoSimulacao = new ResultadoSimulacaoDto(valorFinalCalculado, produtoReal.Rentabilidade, request.PrazoMeses);
            var dataSimulacao = DateTime.UtcNow;

            var simulacaoEntity = new Simulacao
            {
                ClienteId = request.ClienteId,
                ProdutoNome = produtoReal.Nome, // Usando o nome do produto real
                //TipoProduto = produtoReal.Tipo, // Usando o tipo do produto real
                ValorInvestido = request.Valor,
                ValorFinal = valorFinalCalculado,
                PrazoMeses = request.PrazoMeses,
                Data = dataSimulacao
            };

            await _simuladorRepository.AddSimulacaoAsync(simulacaoEntity);

            return new SimularInvestimentoResponse(produtoValidado, resultadoSimulacao, dataSimulacao);
        }

        public async Task<IEnumerable<HistoricoSimulacaoDto>> ObterHistoricoSimulacoesAsync()
        {
            // Uso do repositório para buscar dados
            var simulacoes = await _simuladorRepository.GetAllSimulacoesAsync();

            // Mapeamento de Entidade para DTO de Retorno
            return simulacoes.Select(s => new HistoricoSimulacaoDto(
                s.Id,
                s.ClienteId,
                s.ProdutoNome,
                s.ValorInvestido,
                s.ValorFinal,
                s.PrazoMeses,
                s.Data
            )).ToList();
        }

        public async Task<IEnumerable<ValoresPorProdutoDiaDto>> ObterValoresPorProdutoDiaAsync()
        {
            // O serviço apenas chama o repositório e retorna o resultado
            return await _simuladorRepository.GetValoresPorProdutoDiaAsync();
        }
    }
}

