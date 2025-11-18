using static Simulador.Api.Controllers.SimuladorController;

namespace Simulador.Api.Logic.Service
{
    public interface ISimuladorService
    {
        Task<SimularInvestimentoResponse> ProcessarESalvarSimulacaoAsync(SimularInvestimentoRequest request);
        Task<IEnumerable<HistoricoSimulacaoDto>> ObterHistoricoSimulacoesAsync();
        Task<IEnumerable<ValoresPorProdutoDiaDto>> ObterValoresPorProdutoDiaAsync();
    }
}
