using Microsoft.AspNetCore.Mvc;
using Moq;
using Simulador.Api.Controllers;
using Simulador.Api.Logic.Service;
using static Simulador.Api.Controllers.SimuladorController;

namespace Simulador.Api.Tests.Controllers.Tests
{
    public class SimuladorControllerTests
    {
        private readonly Mock<ISimuladorService> _mockService;
        private readonly SimuladorController _controller;

        public SimuladorControllerTests()
        {
            _mockService = new Mock<ISimuladorService>();
            _controller = new SimuladorController(_mockService.Object);
        }

        // --- Testes para POST /simular-investimento ---

        [Fact]
        public async Task SimularInvestimento_DeveRetornarOkComResponse_CenarioFeliz()
        {
            // Arrange
            var request = new SimularInvestimentoRequest(1, 1000m, 12, "RF");
            var mockResponse = new SimularInvestimentoResponse(
                new ProdutoValidadoDto(1, "CDB", "RF", 0.1m, "Baixo"),
                new ResultadoSimulacaoDto(1100m, 0.1m, 12),
                DateTime.UtcNow
            );

            _mockService.Setup(s => s.ProcessarESalvarSimulacaoAsync(request))
                        .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.SimularInvestimento(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var responseDto = Assert.IsType<SimularInvestimentoResponse>(okResult.Value);
            Assert.Equal(1100m, responseDto.ResultadoSimulacao.ValorFinal);
            _mockService.Verify(s => s.ProcessarESalvarSimulacaoAsync(request), Times.Once);
        }

        [Fact]
        public async Task SimularInvestimento_DeveRetornarBadRequest_QuandoDadosDeInputInvalidos()
        {
            // Arrange
            // Valor 0m é inválido de acordo com a validação do controller
            var requestInvalido = new SimularInvestimentoRequest(1, 0m, 12, "RF");

            // Act
            var result = await _controller.SimularInvestimento(requestInvalido);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            // Garante que o serviço não foi chamado
            _mockService.Verify(s => s.ProcessarESalvarSimulacaoAsync(It.IsAny<SimularInvestimentoRequest>()), Times.Never);
        }

        [Fact]
        public async Task SimularInvestimento_DeveRetornarNotFound_QuandoServicoLancaInvalidOperationException()
        {
            // Arrange
            var request = new SimularInvestimentoRequest(1, 1000m, 12, "ProdutoInexistente");

            // Simula a exceção que o serviço lançaria (conforme a lógica que testamos anteriormente)
            _mockService.Setup(s => s.ProcessarESalvarSimulacaoAsync(request))
                        .ThrowsAsync(new InvalidOperationException("Produto não encontrado."));

            // Act
            var result = await _controller.SimularInvestimento(request);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result); // Esperamos um 404
            _mockService.Verify(s => s.ProcessarESalvarSimulacaoAsync(request), Times.Once);
        }

        // --- Testes para GET /simulacoes ---

        [Fact]
        public async Task GetHistoricoSimulacoes_DeveRetornarOkComLista()
        {
            // Arrange
            var historicoMock = new List<HistoricoSimulacaoDto> { /* ... */ };
            _mockService.Setup(s => s.ObterHistoricoSimulacoesAsync()).ReturnsAsync(historicoMock);

            // Act
            var result = await _controller.GetHistoricoSimulacoes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        // --- Testes para GET /simulacoes/por-produto-dia ---

        [Fact]
        public async Task GetValoresPorProdutoDia_DeveDelegarAoServicoERetornarOk()
        {
            // Arrange
            var valoresMock = new List<ValoresPorProdutoDiaDto> { /* ... */ };
            _mockService.Setup(s => s.ObterValoresPorProdutoDiaAsync()).ReturnsAsync(valoresMock);

            // Act
            var result = await _controller.GetValoresPorProdutoDia();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            _mockService.Verify(s => s.ObterValoresPorProdutoDiaAsync(), Times.Once);
        }
    }
}
