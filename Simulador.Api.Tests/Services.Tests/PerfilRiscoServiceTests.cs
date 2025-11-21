using Moq;
using Simulador.Api.Logic.Repositories;
using Simulador.Api.Logic.Service;
using Simulador.Api.Models;
using static Simulador.Api.Controllers.PerfilRiscoController;

namespace Simulador.Api.Tests.Services.Tests
{
    public class PerfilRiscoServiceTests
    {
        private readonly Mock<IClienteRepository> _mockRepository;
        private readonly PerfilRiscoService _service;

        public PerfilRiscoServiceTests()
        {
            _mockRepository = new Mock<IClienteRepository>();
            _service = new PerfilRiscoService(_mockRepository.Object);
        }

        [Fact]
        public async Task ObterPerfilRiscoAsync_DeveRetornarNull_QuandoClienteNaoEncontrado()
        {
            // Arrange
            int clienteIdInexistente = 999;

            // Configura o mock para retornar null quando o repositório for chamado com este ID
            _mockRepository.Setup(repo => repo.GetClienteComPerfilAsync(clienteIdInexistente))
                           .ReturnsAsync((Cliente)null);

            // Act
            var resultado = await _service.ObterPerfilRiscoAsync(clienteIdInexistente);

            // Assert
            Assert.Null(resultado);

            // Verifica se o método do repositório foi chamado exatamente uma vez com o ID correto
            _mockRepository.Verify(repo => repo.GetClienteComPerfilAsync(clienteIdInexistente), Times.Once);
        }

        [Fact]
        public async Task ObterPerfilRiscoAsync_DeveRetornarNull_QuandoClienteEncontradoSemPerfilAssociado()
        {
            // Arrange
            int clienteIdSemPerfil = 50;
            //Define Perfil como null para simular a falha na inclusão.
            var clienteSemPerfil = new Cliente { Id = clienteIdSemPerfil, PerfilId = 0, Perfil = null, PontuacaoRisco = 10 };

            _mockRepository.Setup(repo => repo.GetClienteComPerfilAsync(clienteIdSemPerfil))
                           .ReturnsAsync(clienteSemPerfil);

            // Act
            var resultado = await _service.ObterPerfilRiscoAsync(clienteIdSemPerfil);

            // Assert (Verificação)
            // O serviço deve retornar null se o objeto Perfil for nulo, mesmo que o Cliente exista.
            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObterPerfilRiscoAsync_DeveRetornarPerfilRiscoDtoCorreto_QuandoClienteEPerfilExistem()
        {
            // Arrange
            int clienteId = 1;
            string nomePerfilEsperado = "Agressivo";
            int pontuacaoEsperada = 85;
            string descricaoEsperada = "Alto risco, alto retorno.";
            int perfilId = 3;

            // Simula os objetos completos que viriam do banco de dados com a nova estrutura
            var perfilExistente = new Perfil
            {
                Id = perfilId,
                Nome = nomePerfilEsperado,
                Descricao = descricaoEsperada
            };

            var clienteExistente = new Cliente
            {
                Id = clienteId,
                PerfilId = perfilId, // Chave estrangeira
                PontuacaoRisco = pontuacaoEsperada,
                Perfil = perfilExistente // Propriedade de navegação populada
            };

            _mockRepository.Setup(repo => repo.GetClienteComPerfilAsync(clienteId))
                           .ReturnsAsync(clienteExistente);

            // Act
            var resultado = await _service.ObterPerfilRiscoAsync(clienteId);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<PerfilRiscoDto>(resultado);

            // Verifica se os dados do DTO estão corretos
            Assert.Equal(clienteId, resultado.ClienteId);
            Assert.Equal(nomePerfilEsperado, resultado.Perfil);
            Assert.Equal(pontuacaoEsperada, resultado.Pontuacao);
            Assert.Equal(descricaoEsperada, resultado.Descricao);
        }
    }
}
