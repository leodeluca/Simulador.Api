using Simulador.Api.Models;
using static Simulador.Api.Controllers.SimuladorController;

namespace Simulador.Api.Logic.Repositories
{
    public interface ISimuladorRepository
    {
        Task AddSimulacaoAsync(Simulacao simulacao);
        Task<IEnumerable<Simulacao>> GetAllSimulacoesAsync();

        Task<IEnumerable<ValoresPorProdutoDiaDto>> GetValoresPorProdutoDiaAsync();
    }
}
