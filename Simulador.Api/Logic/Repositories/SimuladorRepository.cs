using Microsoft.EntityFrameworkCore;
using Simulador.Api.Data;
using Simulador.Api.Models;
using static Simulador.Api.Controllers.SimuladorController;

namespace Simulador.Api.Logic.Repositories
{
    public class SimuladorRepository : ISimuladorRepository
    {
        private readonly AppDbContext dbContext;
        public SimuladorRepository(AppDbContext dbContext) => this.dbContext = dbContext;

        public async Task AddSimulacaoAsync(Simulacao simulacao)
        {
            await dbContext.Simulacoes.AddAsync(simulacao);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Simulacao>> GetAllSimulacoesAsync()
        {
            return await dbContext.Simulacoes.ToListAsync();
        }

        public async Task<IEnumerable<ValoresPorProdutoDiaDto>> GetValoresPorProdutoDiaAsync()
        {
            var resultadosAgregadosNoBanco = await dbContext.Simulacoes
                .GroupBy(s => new {
                    NomeDoProduto = s.ProdutoNome,
                    Data = s.Data.Date
                })
                .Select(g => new {
                    Produto = g.Key.NomeDoProduto,
                    Data = g.Key.Data,
                    QuantidadeSimulacoes = g.Count(),
                    MediaValorFinal = g.Average(s => s.ValorFinal)
                })
                .ToListAsync();

            var resultadosFinais = resultadosAgregadosNoBanco.Select(r => 
                new ValoresPorProdutoDiaDto(
                    Produto: r.Produto,
                    Data: r.Data.ToString("yyyy-MM-dd"),
                    QuantidadeSimulacoes: r.QuantidadeSimulacoes,
                    MediaValorFinal: r.MediaValorFinal
                    )
            ).ToList();

            return resultadosFinais;
        }
    }
}
