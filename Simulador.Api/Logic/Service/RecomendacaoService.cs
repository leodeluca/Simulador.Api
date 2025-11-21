using Simulador.Api.Logic.Repositories;
using Simulador.Api.Models;
using static RecomendacoesController;
using static Simulador.Api.Controllers.PerfilRiscoController;

namespace Simulador.Api.Logic.Service
{
    public class RecomendacaoService : IRecomendacaoService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IInvestimentoRepository _investimentoRepository;
        private readonly IProdutoRepository _produtoRepository;
        public RecomendacaoService(IClienteRepository clienteRepository,
                               IInvestimentoRepository investimentoRepository,
                               IProdutoRepository produtoRepository)
        {
            _clienteRepository = clienteRepository;
            _investimentoRepository = investimentoRepository;
            _produtoRepository = produtoRepository;
        }
        public async Task<IEnumerable<ProdutoRecomendadoDto>> ObterProdutosRecomendadosAsync(string perfilRisco)
        {
            var perfisValidos = new List<string> { "Conservador", "Moderado", "Agressivo" };
            if (!perfisValidos.Contains(perfilRisco, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"O perfil de risco '{perfilRisco}' é inválido. Use Conservador, Moderado ou Agressivo.");
            }

            string perfilPadronizado = char.ToUpper(perfilRisco[0]) + perfilRisco.Substring(1).ToLower();
            // Lógica de negócios: traduzir perfil para níveis de risco aceitáveis
            List<string> riscosAceitaveis = new List<string>();
            if (perfilPadronizado == "Moderado")
            {
                riscosAceitaveis.Add("Baixo");
                riscosAceitaveis.Add("Moderado");
            }
            else if (perfilPadronizado == "Agressivo")
            {
                riscosAceitaveis.Add("Baixo");
                riscosAceitaveis.Add("Moderado");
                riscosAceitaveis.Add("Alto");
            } else
            {
                riscosAceitaveis.Add("Baixo");
            }

                // A busca no repositório por enquanto é simples (pelo risco exato), 
                // mas a lógica de filtragem pode ser aprimorada aqui ou no repositório.
                var produtos = await _produtoRepository.GetAllAsync(); // Busca todos para filtrar em memória por simplicidade

            var produtosFiltrados = produtos
                .Where(p => riscosAceitaveis.Contains(p.Risco))
                .Select(p => new ProdutoRecomendadoDto(
                    p.Id,
                    p.Nome,
                    p.Tipo,
                    p.Rentabilidade,
                    p.Risco
                ));

            return produtosFiltrados;
        }

        public async Task<List<RecomendacaoProdutoDto>> GetRecomendacoesPorClienteId(int clienteId)
        {
            var cliente = await _clienteRepository.GetClienteById(clienteId);
            if (cliente == null)
            {
                return new List<RecomendacaoProdutoDto>();
            }

            var historico = await _investimentoRepository.GetByClienteIdAsync(clienteId);
            var todosProdutos = await _produtoRepository.GetAllAsync();

            // 1. Analisar Volume de Investimentos
            var volumeTotal = historico.Sum(i => i.Valor);
            var volumeMedio = historico.Any() ? historico.Average(i => i.Valor) : 0;

            // 2. Analisar Frequência de Movimentações
            var investimentosRecentes = historico.Where(i => i.Data >= DateTime.Now.AddMonths(-6)).ToList();
            var frequenciaAlta = investimentosRecentes.Count > 3;

            // 3. Analisar Preferência por Liquidez ou Rentabilidade
            List<Produto> produtosRecomendados = new List<Produto>();
            List<Produto> produtosBase = new List<Produto>();

            // Mapeamento simplificado de risco do produto (ex: Baixo, Medio, Alto) para uma pontuação numérica
            // Assumindo: Baixo=20, Medio=50, Alto=80
            Func<Produto, int> getRiscoNumerico = p => p.Risco switch
            {
                "Baixo" => 20,
                "Medio" => 50,
                "Alto" => 80,
                _ => 50 // Padrão
            };

            switch (cliente.Perfil.Nome)
            {
                case "Conservador":
                    // Base: Produtos de Baixo Risco
                    produtosBase = todosProdutos.Where(p => getRiscoNumerico(p) <= 40).ToList();
                    break;
                case "Moderado":
                    // Base: Produtos de Risco Baixo a Médio
                    produtosBase = todosProdutos.Where(p => getRiscoNumerico(p) <= 60).ToList();
                    break;
                case "Agressivo":
                    // Base: Produtos de Risco Médio a Alto
                    produtosBase = todosProdutos.Where(p => getRiscoNumerico(p) >= 40).ToList();
                    break;
                default:
                    produtosBase = todosProdutos.Where(p => p.Risco == "Baixo").ToList();
                    break;
            }

            // Refinamento usando a Pontuação de Risco do cliente (0-100)
            // Filtra os produtos base para aqueles cujo risco é "compatível" com a pontuação do cliente.
            // Define uma margem de tolerância (ex: +/- 20 pontos da pontuação do cliente)
            int pontuacaoCliente = cliente.PontuacaoRisco;

            produtosRecomendados = produtosBase
            .Where(p => getRiscoNumerico(p) <= pontuacaoCliente + 15 && getRiscoNumerico(p) >= pontuacaoCliente - 15)
            .ToList();

            // Se a lista de produtos recomendados estiver vazia, voltamos para a lista base para garantir alguma recomendação
            if (!produtosRecomendados.Any())
            {
                produtosRecomendados = produtosBase;
            }

            // Refina as recomendações com base no volume e frequência
            if (frequenciaAlta && volumeMedio > 50000)
            {
                produtosRecomendados = produtosRecomendados.OrderByDescending(p => p.Rentabilidade).ThenBy(p => p.Risco).ToList();
            }
            else if (!frequenciaAlta && volumeTotal > 20000)
            {
                produtosRecomendados = produtosRecomendados.Where(p => p.Tipo == "RF").ToList();
            }
            else
            {
                produtosRecomendados = produtosRecomendados.OrderBy(p => p.Risco).ToList();
            }

            return produtosRecomendados.Distinct()
                                   .Take(3)
                                   .Select(p => new RecomendacaoProdutoDto
                                   (
                                       p.Id,
                                       p.Nome,
                                       p.Tipo,
                                       $"Recomendação baseada no perfil {cliente.Perfil.Nome} e pontuação de risco {cliente.PontuacaoRisco}."
                                   )).ToList();
        }
    }
}
