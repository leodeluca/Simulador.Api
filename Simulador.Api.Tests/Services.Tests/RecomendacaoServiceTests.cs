using Moq;
using Simulador.Api.Logic.Repositories;
using Simulador.Api.Logic.Service;
using Simulador.Api.Models;

namespace Simulador.Api.Tests.Services.Tests
{
    public class RecomendacaoServiceTests
    {
        private readonly Mock<IClienteRepository> _mockClienteRepo;
        private readonly Mock<IInvestimentoRepository> _mockInvestimentoRepo;
        private readonly Mock<IProdutoRepository> _mockProdutoRepo;
        private readonly RecomendacaoService _service;

        public RecomendacaoServiceTests()
        {
            _mockClienteRepo = new Mock<IClienteRepository>();
            _mockInvestimentoRepo = new Mock<IInvestimentoRepository>();
            _mockProdutoRepo = new Mock<IProdutoRepository>();
            _service = new RecomendacaoService(
                _mockClienteRepo.Object,
                _mockInvestimentoRepo.Object,
                _mockProdutoRepo.Object
            );
        }

        private List<Produto> GetProdutosMock()
        {
            return new List<Produto>
            {
                // Risco Baixo
                new Produto { Id = 1, Nome = "CDB DI", Tipo = "RF", Rentabilidade = 0.08m, Risco = "Baixo" },
                new Produto { Id = 2, Nome = "Tesouro Selic", Tipo = "RF", Rentabilidade = 0.07m, Risco = "Baixo" },
                // Risco Moderado/Medio
                new Produto { Id = 3, Nome = "LCI High Yield", Tipo = "RF", Rentabilidade = 0.10m, Risco = "Moderado" },
                new Produto { Id = 4, Nome = "Fundo Multimercado", Tipo = "Fundo", Rentabilidade = 0.12m, Risco = "Moderado" },
                // Risco Alto
                new Produto { Id = 5, Nome = "Ações Blue Chip", Tipo = "RV", Rentabilidade = 0.15m, Risco = "Alto" },
                new Produto { Id = 6, Nome = "BDR Tech", Tipo = "RV", Rentabilidade = 0.18m, Risco = "Alto" },
            };
        }

        // --- Testes para ObterProdutosRecomendadosAsync(string perfilRisco) ---

        [Fact]
        public async Task ObterProdutosRecomendadosAsync_PerfilConservador_DeveRetornarApenasBaixoRisco()
        {
            // Arrange
            var perfil = "Conservador";
            var produtosMock = GetProdutosMock();
            _mockProdutoRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(produtosMock);

            // Act
            var resultado = await _service.ObterProdutosRecomendadosAsync(perfil);

            // Assert
            Assert.NotNull(resultado);
            var produtosList = resultado.ToList();
            Assert.Equal(2, produtosList.Count); // CDB DI e Tesouro Selic
            Assert.True(produtosList.All(p => p.Risco == "Baixo"));
            _mockProdutoRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task ObterProdutosRecomendadosAsync_PerfilModerado_DeveRetornarBaixoEModeradoRisco()
        {
            // Arrange
            var perfil = "Moderado";
            var produtosMock = GetProdutosMock();
            _mockProdutoRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(produtosMock);

            // Act
            var resultado = await _service.ObterProdutosRecomendadosAsync(perfil);

            // Assert
            Assert.NotNull(resultado);
            var produtosList = resultado.ToList();
            Assert.Equal(4, produtosList.Count); // Baixo (2) + Medio (2)
            Assert.True(produtosList.All(p => p.Risco == "Baixo" || p.Risco == "Moderado"));
        }

        [Fact]
        public async Task ObterProdutosRecomendadosAsync_PerfilAgressivo_DeveRetornarTodosOsNiveisDeRisco()
        {
            // Arrange
            var perfil = "Agressivo";
            var produtosMock = GetProdutosMock();
            _mockProdutoRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(produtosMock);

            // Act
            var resultado = await _service.ObterProdutosRecomendadosAsync(perfil);

            // Assert
            Assert.NotNull(resultado);
            var produtosList = resultado.ToList();
            Assert.Equal(6, produtosList.Count); // Todos os 6 produtos
        }


        // --- Testes para GetRecomendacoesPorClienteId(int clienteId) ---

        [Fact]
        public async Task GetRecomendacoesPorClienteId_DeveRetornarListaVazia_QuandoClienteNaoExiste()
        {
            // Arrange
            int clienteId = 999;
            _mockClienteRepo.Setup(repo => repo.GetClienteById(clienteId))
                            .ReturnsAsync((Cliente)null);

            // Act
            var resultado = await _service.GetRecomendacoesPorClienteId(clienteId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
            // Garante que não tentou buscar histórico ou produtos se o cliente não existe
            _mockInvestimentoRepo.Verify(repo => repo.GetByClienteIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetRecomendacoesPorClienteId_PerfilModerado_ComVolumeMedio_DeveOrdenarPorRisco()
        {
            // Arrange
            int clienteId = 1;
            var cliente = new Cliente
            {
                Id = clienteId,
                PontuacaoRisco = 50, // Moderado
                PerfilId = 2,
                Perfil = new Perfil { Id = 2, Nome = "Moderado", Descricao = "" }
            };
            var historico = new List<Investimento>
            { 
                // Volume e Frequencia Baixos (teste o path default da ordenação por risco)
                new Investimento { Id = 1, ClienteId = clienteId, Valor = 100m, Data = DateTime.Now.AddMonths(-10) }
            };
            var produtosMock = GetProdutosMock();

            _mockClienteRepo.Setup(repo => repo.GetClienteById(clienteId)).ReturnsAsync(cliente);
            _mockInvestimentoRepo.Setup(repo => repo.GetByClienteIdAsync(clienteId)).ReturnsAsync(historico);
            _mockProdutoRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(produtosMock);

            // Act
            var resultado = await _service.GetRecomendacoesPorClienteId(clienteId);

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.Equal("LCI High Yield", resultado[0].NomeProduto); // Risco Medio
            Assert.Equal("Fundo Multimercado", resultado[1].NomeProduto); // Risco Medio
        }

        [Fact]
        public async Task GetRecomendacoesPorClienteId_PerfilAgressivo_AltaFrequencia_DeveOrdenarPorRentabilidade()
        {
            // Arrange
            int clienteId = 2;
            var cliente = new Cliente
            {
                Id = clienteId,
                PontuacaoRisco = 80, // Agressivo
                PerfilId = 3,
                Perfil = new Perfil { Id = 3, Nome = "Agressivo", Descricao = "" }
            };

            // Simula alta frequência e volume médio alto (ativa o path de ordenação por rentabilidade)
            decimal valorInvestimento = 50001m;
            var historico = new List<Investimento>();
            for (int i = 0; i < 6; i++) // Mais de 5 investimentos recentes
            {
                historico.Add(new Investimento { Id = i + 1, ClienteId = clienteId, Valor = valorInvestimento, Data = DateTime.Now.AddMonths(-1) });
            }
            var produtosMock = GetProdutosMock(); // Todos são elegíveis para Agressivo

            _mockClienteRepo.Setup(repo => repo.GetClienteById(clienteId)).ReturnsAsync(cliente);
            _mockInvestimentoRepo.Setup(repo => repo.GetByClienteIdAsync(clienteId)).ReturnsAsync(historico);
            _mockProdutoRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(produtosMock);

            // Act
            var resultado = await _service.GetRecomendacoesPorClienteId(clienteId);
            var listaResultados = resultado.ToList();

            // Assert
            Assert.Equal(2, listaResultados.Count);
            Assert.Equal("BDR Tech", listaResultados[0].NomeProduto); // 0.18m
            Assert.Equal("Ações Blue Chip", listaResultados[1].NomeProduto); // 0.15m
        }

        [Fact]
        public async Task ObterProdutosRecomendadosAsync_DeveLancarExcecao_QuandoPerfilInvalido()
        {
            // Arrange
            var perfilInvalido = "Inexistente";

            // Act & Assert
            // Espera que o método lance uma ArgumentException
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ObterProdutosRecomendadosAsync(perfilInvalido));

            //Verificar a mensagem da exceção
            Assert.Contains("O perfil de risco 'Inexistente' é inválido", exception.Message);

            // Garante que o repositório nunca foi chamado, pois falhou na validação inicial
            _mockProdutoRepo.Verify(repo => repo.GetAllAsync(), Times.Never);
        }


    }
}
