using Simulador.Api.Models;

namespace Simulador.Api.Logic.Repositories
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> GetByRiscoAsync(string risco);
        Task<IEnumerable<Produto>> GetAllAsync();
        Task<Produto?> GetByTipoAsync(string tipoProduto);
    }
}
