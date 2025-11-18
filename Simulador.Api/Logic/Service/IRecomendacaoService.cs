using static RecomendacoesController;
using static Simulador.Api.Controllers.PerfilRiscoController;

namespace Simulador.Api.Logic.Service
{
    public interface IRecomendacaoService
    {
        Task<IEnumerable<ProdutoRecomendadoDto>> ObterProdutosRecomendadosAsync(string perfilRisco);

        Task<List<RecomendacaoProdutoDto>> GetRecomendacoesPorClienteId(int clienteId);
    }
}
