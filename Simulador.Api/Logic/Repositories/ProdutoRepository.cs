using Microsoft.EntityFrameworkCore;
using Simulador.Api.Data;
using Simulador.Api.Models;

namespace Simulador.Api.Logic.Repositories
{
        public class ProdutoRepository : IProdutoRepository
        {
            private readonly AppDbContext dbContext;
            public ProdutoRepository(AppDbContext dbContext) => this.dbContext = dbContext;

            public async Task<IEnumerable<Produto>> GetByRiscoAsync(string risco)
            {
                return await dbContext.Produtos.Where(p => p.Risco == risco).ToListAsync();
            }

            public async Task<IEnumerable<Produto>> GetAllAsync()
            {
                return await dbContext.Produtos.ToListAsync();
            }

            public async Task<Produto?> GetByTipoAsync(string tipoProduto)
            {
                // Implemente a lógica de busca (ex: pelo tipo ou nome)
                return await dbContext.Produtos
                    .FirstOrDefaultAsync(p => p.Tipo == tipoProduto);
            }
        }

}
