using Moq;
using Simulador.Api.Logic.Repositories;
using Simulador.Api.Logic.Service;
using Simulador.Api.Models;
using static Simulador.Api.Controllers.SimuladorController;

namespace Simulador.Api.Tests.Services.Tests
{
    public class SimuladorServiceTests
    {
        private readonly Mock<ISimuladorRepository> _mockSimuladorRepo;
        private readonly Mock<IProdutoRepository> _mockProdutoRepo;
        private readonly SimuladorService _service;

        public SimuladorServiceTests()
        {
            _mockSimuladorRepo = new Mock<ISimuladorRepository>();
            _mockProdutoRepo = new Mock<IProdutoRepository>();
            _service = new SimuladorService(_mockSimuladorRepo.Object, _mockProdutoRepo.Object);
        }

        // --- Testes para ProcessarESalvarSimulacaoAsync ---

        [Fact]
        public async Task ProcessarESalvarSimulacaoAsync_DeveRetornarErro_QuandoProdutoNaoEncontrado()
        {
            // Arrange
            var requestInvalido = new SimularInvestimentoRequest(
                ClienteId: 1,
                Valor: 1000m,
                PrazoMeses: 12,
                TipoProduto: "ProdutoInexistente"
            );

            // Configura o mock do ProdutoRepository para retornar null
            _mockProdutoRepo.Setup(repo => repo.GetByTipoAsync(requestInvalido.TipoProduto))
                            .ReturnsAsync((Produto)null);

            // Act & Assert
            // Esperamos que o método lance uma InvalidOperationException
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ProcessarESalvarSimulacaoAsync(requestInvalido));

            // Garante que o repositório de simulação NÃO foi chamado
            _mockSimuladorRepo.Verify(repo => repo.AddSimulacaoAsync(It.IsAny<Simulacao>()), Times.Never);
        }

        [Fact]
        public async Task ProcessarESalvarSimulacaoAsync_DeveCalcularE_SalvarCorretamente_CenarioFeliz()
        {
            // Arrange
            var request = new SimularInvestimentoRequest(
                ClienteId: 1,
                Valor: 1000m,
                PrazoMeses: 12,
                TipoProduto: "RF"
            );
            var produtoFalso = new Produto
            {
                Id = 10,
                Nome = "CDB XYZ",
                Tipo = "RF",
                Rentabilidade = 0.10m, // 10% ao ano
                Risco = "Baixo"
            };

            // Setup: Quando buscar por "RF", retorne o produto falso
            _mockProdutoRepo.Setup(repo => repo.GetByTipoAsync("RF"))
                            .ReturnsAsync(produtoFalso);

            // Setup: Configura o mock do SimuladorRepository para aceitar qualquer chamada (void method)
            _mockSimuladorRepo.Setup(repo => repo.AddSimulacaoAsync(It.IsAny<Simulacao>()))
                              .Returns(Task.CompletedTask)
                              .Callback<Simulacao>(s =>
                              {
                                  // Captura a entidade que foi passada para o repositório mockado
                                  Assert.Equal(request.ClienteId, s.ClienteId);
                                  Assert.Equal(request.Valor, s.ValorInvestido);
                                  Assert.Equal(1100m, s.ValorFinal); // 1000 * (1 + 0.10 * (12/12))
                                  Assert.Equal("CDB XYZ", s.ProdutoNome);
                              });

            // Act
            var resultado = await _service.ProcessarESalvarSimulacaoAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1100m, resultado.ResultadoSimulacao.ValorFinal);
            Assert.Equal("CDB XYZ", resultado.ProdutoValidado.Nome);

            // Verifica se o método de adicionar simulação foi chamado exatamente uma vez
            _mockSimuladorRepo.Verify(repo => repo.AddSimulacaoAsync(It.IsAny<Simulacao>()), Times.Once);
        }

        // --- Testes para ObterHistoricoSimulacoesAsync ---

        [Fact]
        public async Task ObterHistoricoSimulacoesAsync_DeveRetornarListaVazia_QuandoNaoHaDadosNoRepositorio()
        {
            // Arrange
            _mockSimuladorRepo.Setup(repo => repo.GetAllSimulacoesAsync())
                           .ReturnsAsync(new List<Simulacao>()); // Retorna lista vazia

            // Act
            var resultado = await _service.ObterHistoricoSimulacoesAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
            _mockSimuladorRepo.Verify(repo => repo.GetAllSimulacoesAsync(), Times.Once);
        }

        [Fact]
        public async Task ObterHistoricoSimulacoesAsync_DeveMapearCorretamenteParaDto()
        {
            // Arrange
            var simulacoesDoRepo = new List<Simulacao>
            {
                new Simulacao { Id = 1, ClienteId = 1, ProdutoNome = "A", ValorInvestido = 100, ValorFinal = 110, PrazoMeses = 12, Data = DateTime.UtcNow },
                new Simulacao { Id = 2, ClienteId = 1, ProdutoNome = "B", ValorInvestido = 200, ValorFinal = 230, PrazoMeses = 24, Data = DateTime.UtcNow }
            };

            _mockSimuladorRepo.Setup(repo => repo.GetAllSimulacoesAsync())
                           .ReturnsAsync(simulacoesDoRepo);

            // Act
            var resultado = (await _service.ObterHistoricoSimulacoesAsync()).ToList();

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.IsType<HistoricoSimulacaoDto>(resultado[0]);
            Assert.Equal(110m, resultado[0].ValorFinal);
            Assert.Equal(230m, resultado[1].ValorFinal);
        }

        // --- Testes para ObterValoresPorProdutoDiaAsync ---

        [Fact]
        public async Task ObterValoresPorProdutoDiaAsync_DeveDelegarChamadaAoRepositorio()
        {
            // Arrange
            var dtosDoRepo = new List<ValoresPorProdutoDiaDto>
            {
                new ValoresPorProdutoDiaDto("A", "2023-01-01", 5, 150m)
            };

            _mockSimuladorRepo.Setup(repo => repo.GetValoresPorProdutoDiaAsync())
                              .ReturnsAsync(dtosDoRepo);

            // Act
            var resultado = await _service.ObterValoresPorProdutoDiaAsync();

            // Assert
            Assert.Single(resultado);
            Assert.Equal("A", resultado.First().Produto);
            // Verifica se o serviço apenas repassou a chamada sem adicionar lógica extra
            _mockSimuladorRepo.Verify(repo => repo.GetValoresPorProdutoDiaAsync(), Times.Once);
        }
    }
}
