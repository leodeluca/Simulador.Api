using Moq;
using Simulador.Api.Logic.Repositories;
using Simulador.Api.Logic.Service;
using Simulador.Api.Models;
using static Simulador.Api.Controllers.InvestimentoController;

namespace Simulador.Api.Tests.Services.Tests
{
    public class InvestimentoServiceTests
    {
        private readonly Mock<IInvestimentoRepository> _mockRepository;
        private readonly InvestimentoService _service;

        // O construtor é executado antes de cada teste (Fixture Setup)
        public InvestimentoServiceTests()
        {
            _mockRepository = new Mock<IInvestimentoRepository>();
            _service = new InvestimentoService(_mockRepository.Object);
        }

        [Fact]
        public async Task ObterHistoricoAsync_DeveRetornarListaVazia_QuandoNaoHouverInvestimentosParaOCliente()
        {
            // Arrange (Preparação)
            int clienteId = 99; // Um ID que não existe

            // Configura o mock para retornar uma lista vazia quando GetByClienteIdAsync for chamado com qualquer Int
            _mockRepository.Setup(repo => repo.GetByClienteIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(new List<Investimento>());

            // Act (Ação)
            var resultado = await _service.ObterHistoricoAsync(clienteId);

            // Assert (Verificação)
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task ObterHistoricoAsync_DeveRetornarListaDeInvestimentosDto_QuandoExistiremInvestimentos()
        {
            // Arrange (Preparação)
            int clienteId = 1;
            var investimentosDoRepositorio = new List<Investimento>
            {
                new Investimento { Id = 1, ClienteId = clienteId, Tipo = "Renda Fixa", Valor = 1000m, Rentabilidade = 0.05m, Data = DateTime.Now.AddDays(-10) },
                new Investimento { Id = 2, ClienteId = clienteId, Tipo = "Renda Variável", Valor = 2000m, Rentabilidade = 0.02m, Data = DateTime.Now.AddDays(-5) }
            };

            // Configura o mock para retornar a lista preparada quando o método for chamado com o clienteId específico
            _mockRepository.Setup(repo => repo.GetByClienteIdAsync(clienteId))
                           .ReturnsAsync(investimentosDoRepositorio);

            // Act (Ação)
            var resultado = await _service.ObterHistoricoAsync(clienteId);

            // Assert (Verificação)
            Assert.NotNull(resultado);
            var listaResultado = resultado.ToList();
            Assert.Equal(2, listaResultado.Count);

            // Verifica se o mapeamento para DTO foi feito corretamente
            Assert.IsType<InvestimentoDto>(listaResultado[0]);
            Assert.Equal(1000m, listaResultado[0].Valor);
            Assert.Equal("Renda Variável", listaResultado[1].Tipo);
        }

        [Fact]
        public async Task ObterHistoricoAsync_DeveChamarOMetodoDoRepositorioApenasUmaVez()
        {
            // Arrange (Preparação)
            int clienteId = 1;
            _mockRepository.Setup(repo => repo.GetByClienteIdAsync(clienteId))
                          .ReturnsAsync(new List<Investimento>());

            // Act (Ação)
            await _service.ObterHistoricoAsync(clienteId);

            // Assert (Verificação)
            // Verifica se o método GetByClienteIdAsync no mock foi chamado exatamente uma vez com o clienteId correto.
            _mockRepository.Verify(repo => repo.GetByClienteIdAsync(clienteId), Times.Once);
        }
    }
}
