using Simulador.Api.Models;

namespace Simulador.Api.Logic.Repositories
{
    public interface IInvestimentoRepository
    {
        Task<IEnumerable<Investimento>> GetByClienteIdAsync(int clienteId);
    }
}
