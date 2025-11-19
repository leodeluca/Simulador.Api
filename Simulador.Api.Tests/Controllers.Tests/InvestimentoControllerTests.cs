using Microsoft.AspNetCore.Mvc;
using Moq;
using Simulador.Api.Controllers;
using Simulador.Api.Logic.Service;
using static Simulador.Api.Controllers.InvestimentoController;

namespace Simulador.Api.Tests.Controllers.Tests
{
    public class InvestimentoControllerTests
    {
        private readonly Mock<IInvestimentoService> _mockService;
        private readonly InvestimentoController _controller;

        public InvestimentoControllerTests()
        {
            _mockService = new Mock<IInvestimentoService>();
            _controller = new InvestimentoController(_mockService.Object);
        }

        [Fact]
        public async Task GetHistoricoInvestimentos_DeveRetornarBadRequest_QuandoClienteIdInvalido()
        {
            // Arrange
            int clienteIdInvalido = -1;

            // Act
            var result = await _controller.GetHistoricoInvestimentos(clienteIdInvalido);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            _mockService.Verify(s => s.ObterHistoricoAsync(It.IsAny<int>()), Times.Never); // Garante que o serviço nem foi chamado
        }

        [Fact]
        public async Task GetHistoricoInvestimentos_DeveRetornarNotFound_QuandoNaoHaDados()
        {
            // Arrange
            int clienteId = 999;
            _mockService.Setup(s => s.ObterHistoricoAsync(clienteId))
                        .ReturnsAsync(Enumerable.Empty<InvestimentoDto>()); // Retorna lista vazia

            // Act
            var result = await _controller.GetHistoricoInvestimentos(clienteId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
            _mockService.Verify(s => s.ObterHistoricoAsync(clienteId), Times.Once);
        }


    }
}
