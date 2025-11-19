using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Simulador.Api.Logic.Service;
using static RecomendacoesController;

namespace Simulador.Api.Tests.Controllers.Tests
{
    public class RecomendacoesControllerTests
    {
        private readonly Mock<IRecomendacaoService> _mockService;
        private readonly RecomendacoesController _controller;

        public RecomendacoesControllerTests()
        {
            _mockService = new Mock<IRecomendacaoService>();
            _controller = new RecomendacoesController(_mockService.Object);
        }

        [Fact]
        public async Task GetRecomendacoesPorCliente_DeveRetornarOkComListaDeRecomendacoes()
        {
            // Arrange
            int clienteId = 1;
            var mockRecomendacoes = new List<RecomendacaoProdutoDto>
            {
                new RecomendacaoProdutoDto(1, "Produto A", "RF", "Motivo 1"),
                new RecomendacaoProdutoDto(2, "Produto B", "RV", "Motivo 2")
            };

            _mockService.Setup(s => s.GetRecomendacoesPorClienteId(clienteId))
                        .ReturnsAsync(mockRecomendacoes);

            // Act
            var result = await _controller.GetRecomendacoesPorCliente(clienteId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var dtos = Assert.IsAssignableFrom<IEnumerable<RecomendacaoProdutoDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
            _mockService.Verify(s => s.GetRecomendacoesPorClienteId(clienteId), Times.Once);
        }

        [Fact]
        public async Task GetRecomendacoesPorCliente_DeveRetornarBadRequest_QuandoClienteIdInvalido()
        {
            // Arrange
            int clienteIdInvalido = -10;

            // Act
            var result = await _controller.GetRecomendacoesPorCliente(clienteIdInvalido);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            // Garante que o serviço não foi chamado
            _mockService.Verify(s => s.GetRecomendacoesPorClienteId(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetRecomendacoesPorCliente_DeveRetornarNotFound_QuandoServicoRetornaListaVazia()
        {
            // Arrange
            int clienteId = 999; // ID inexistente ou sem recomendações
            _mockService.Setup(s => s.GetRecomendacoesPorClienteId(clienteId))
                        .ReturnsAsync(new List<RecomendacaoProdutoDto>()); // Retorna lista vazia

            // Act
            var result = await _controller.GetRecomendacoesPorCliente(clienteId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            _mockService.Verify(s => s.GetRecomendacoesPorClienteId(clienteId), Times.Once);
        }

        [Fact]
        public async Task GetRecomendacoesPorCliente_DeveRetornarStatusCode500_QuandoServicoLancaExcecao()
        {
            // Arrange
            int clienteId = 1;
            _mockService.Setup(s => s.GetRecomendacoesPorClienteId(clienteId))
                        .ThrowsAsync(new System.Exception("Falha de infraestrutura"));

            // Act
            var result = await _controller.GetRecomendacoesPorCliente(clienteId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }
    }
}
