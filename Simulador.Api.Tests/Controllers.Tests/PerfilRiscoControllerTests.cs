using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Simulador.Api.Controllers;
using Simulador.Api.Logic.Service;
using static Simulador.Api.Controllers.PerfilRiscoController;

namespace Simulador.Api.Tests.Controllers.Tests
{
    public class PerfilRiscoControllerTests
    {
        private readonly Mock<IPerfilRiscoService> _mockPerfilRiscoService;
        private readonly Mock<IRecomendacaoService> _mockRecomendacaoService;
        private readonly PerfilRiscoController _controller;

        public PerfilRiscoControllerTests()
        {
            _mockPerfilRiscoService = new Mock<IPerfilRiscoService>();
            _mockRecomendacaoService = new Mock<IRecomendacaoService>();
            _controller = new PerfilRiscoController(
                _mockPerfilRiscoService.Object,
                _mockRecomendacaoService.Object
            );
        }

        // --- Testes para GET /perfil-risco/{clienteId} ---

        [Fact]
        public async Task GetPerfilRisco_DeveRetornarOkComPerfilDto_QuandoClienteEncontrado()
        {
            // Arrange
            int clienteId = 1;
            var perfilDtoMock = new PerfilRiscoDto(clienteId, "Moderado", 50, "Descricao teste");

            _mockPerfilRiscoService.Setup(s => s.ObterPerfilRiscoAsync(clienteId))
                                   .ReturnsAsync(perfilDtoMock);

            // Act
            var result = await _controller.GetPerfilRisco(clienteId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var dto = Assert.IsType<PerfilRiscoDto>(okResult.Value);
            Assert.Equal(clienteId, dto.ClienteId);
            Assert.Equal("Moderado", dto.Perfil);
        }

        [Fact]
        public async Task GetPerfilRisco_DeveRetornarBadRequest_QuandoClienteIdInvalido()
        {
            // Arrange
            int clienteIdInvalido = -5;

            // Act
            var result = await _controller.GetPerfilRisco(clienteIdInvalido);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            // Verifica se o serviço não foi chamado, pois a validação ocorreu antes
            _mockPerfilRiscoService.Verify(s => s.ObterPerfilRiscoAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetPerfilRisco_DeveRetornarNotFound_QuandoServicoRetornaNull()
        {
            // Arrange
            int clienteId = 999; // ID inexistente
            _mockPerfilRiscoService.Setup(s => s.ObterPerfilRiscoAsync(clienteId))
                                   .ReturnsAsync((PerfilRiscoDto)null);

            // Act
            var result = await _controller.GetPerfilRisco(clienteId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
            _mockPerfilRiscoService.Verify(s => s.ObterPerfilRiscoAsync(clienteId), Times.Once);
        }

        [Fact]
        public async Task GetPerfilRisco_DeveRetornarStatusCode500_QuandoOcorreExcecaoNoServico()
        {
            // Arrange
            int clienteId = 1;
            _mockPerfilRiscoService.Setup(s => s.ObterPerfilRiscoAsync(clienteId))
                                   .ThrowsAsync(new System.Exception("Erro de BD"));

            // Act
            var result = await _controller.GetPerfilRisco(clienteId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // --- Testes para GET /produtos-recomendados/{perfil} ---

        [Fact]
        public async Task GetProdutosRecomendados_DeveRetornarOkComListaDeProdutos()
        {
            // Arrange
            var perfil = "Agressivo";
            var produtosMock = new List<ProdutoRecomendadoDto>
            {
                new ProdutoRecomendadoDto(1, "Ação X", "RV", 0.1m, "Alto")
            };
            _mockRecomendacaoService.Setup(s => s.ObterProdutosRecomendadosAsync(perfil))
                                    .ReturnsAsync(produtosMock);

            // Act
            var result = await _controller.GetProdutosRecomendados(perfil);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ProdutoRecomendadoDto>>(okResult.Value);
            Assert.Single(dtos);
        }

        [Fact]
        public async Task GetProdutosRecomendados_DeveRetornarBadRequest_QuandoPerfilVazio()
        {
            // Arrange
            string perfilInvalido = "";

            // Act
            var result = await _controller.GetProdutosRecomendados(perfilInvalido);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            _mockRecomendacaoService.Verify(s => s.ObterProdutosRecomendadosAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetProdutosRecomendados_DeveRetornarBadRequest_QuandoPerfilInvalido()
        {
            // Arrange
            string perfilInvalido = "Inexistente";

            // Configura o mock do serviço para lançar a ArgumentException
            _mockRecomendacaoService.Setup(s => s.ObterProdutosRecomendadosAsync(perfilInvalido))
                                    .ThrowsAsync(new ArgumentException("O perfil de risco 'Inexistente' é inválido..."));

            // Act
            var result = await _controller.GetProdutosRecomendados(perfilInvalido);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Contains("O perfil de risco 'Inexistente' é inválido", badRequestResult.Value.ToString());

            _mockRecomendacaoService.Verify(s => s.ObterProdutosRecomendadosAsync(perfilInvalido), Times.Once);
        }

    }
}
