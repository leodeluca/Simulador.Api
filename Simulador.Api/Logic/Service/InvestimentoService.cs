using Simulador.Api.Logic.Repositories;
using static Simulador.Api.Controllers.InvestimentoController;

namespace Simulador.Api.Logic.Service
{
    public class InvestimentoService : IInvestimentoService
    {
        private readonly IInvestimentoRepository repository;
        public InvestimentoService(IInvestimentoRepository repository) => this.repository = repository;
        public async Task<IEnumerable<InvestimentoDto>> ObterHistoricoAsync(int clienteId)
        {
            var investimentos = await repository.GetByClienteIdAsync(clienteId);

            return investimentos.Select(i => new InvestimentoDto(
                i.Id,
                i.Tipo,
                i.Valor,
                i.Rentabilidade,
                i.Data
            )).ToList();
        }
    }
}
