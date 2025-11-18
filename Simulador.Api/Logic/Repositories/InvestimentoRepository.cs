using Microsoft.EntityFrameworkCore;
using Simulador.Api.Data;
using Simulador.Api.Models;

namespace Simulador.Api.Logic.Repositories
{
    public class InvestimentoRepository : IInvestimentoRepository
    {
        private readonly AppDbContext dbContext;
        public InvestimentoRepository(AppDbContext dbContext) => this.dbContext = dbContext;
        public async Task<IEnumerable<Investimento>> GetByClienteIdAsync(int clienteId)
        {
            return await dbContext.Investimentos
                .Where(i => i.ClienteId == clienteId)
                .ToListAsync();
        }
    }
}
