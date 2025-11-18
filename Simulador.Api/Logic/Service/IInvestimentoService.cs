using static Simulador.Api.Controllers.InvestimentoController;

namespace Simulador.Api.Logic.Service
{
    public interface IInvestimentoService
    {
        Task<IEnumerable<InvestimentoDto>> ObterHistoricoAsync(int clienteId);
    }
}
