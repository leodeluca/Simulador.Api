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
            // A lógica complexa e específica do Entity Framework fica aqui
            var resultadosAgregadosNoBanco = await dbContext.Simulacoes
                .GroupBy(s => new {
                    NomeDoProduto = s.ProdutoNome, // Usando ProdutoNome conforme o modelo original da pergunta
                    Data = s.Data.Date // Usando .Value porque DataSimulacao é anulável
                })
                .Select(g => new {
                    Produto = g.Key.NomeDoProduto,
                    Data = g.Key.Data, // Mapeia para o formato string do DTO
                    QuantidadeSimulacoes = g.Count(),
                    MediaValorFinal = g.Average(s => s.ValorFinal) // Usando .Value porque ValorFinal é anulável
                })
                .ToListAsync();

            var resultadosFinais = resultadosAgregadosNoBanco.Select(r => 
                new ValoresPorProdutoDiaDto(
                    Produto: r.Produto,
                    // Formatamos para string AGORA, em memória, sem restrições de tradução SQL
                    Data: r.Data.ToString("yyyy-MM-dd"),
                    QuantidadeSimulacoes: r.QuantidadeSimulacoes,
                    MediaValorFinal: r.MediaValorFinal
                    )
            ).ToList();

            return resultadosFinais;
        }
    }
}
