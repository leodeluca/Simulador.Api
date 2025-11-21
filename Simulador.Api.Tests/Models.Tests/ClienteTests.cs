using Simulador.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Simulador.Api.Tests.Models.Tests
{
    public class ClienteTests
    {
        // Método auxiliar para facilitar a validação em todos os testes
        private bool TryValidate(object model, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            return Validator.TryValidateObject(model, context, results, true);
        }

        [Fact]
        public void Cliente_DeveSerValido_QuandoDadosCorretos()
        {
            // Arrange
            var cliente = new Cliente
            {
                Id = 1,
                PontuacaoRisco = 50,
                PerfilId = 1
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(cliente, out results);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Cliente_DeveSerInvalido_QuandoPontuacaoRiscoAbaixoDoMinimo()
        {
            // Arrange
            var cliente = new Cliente
            {
                Id = 1,
                PontuacaoRisco = -10, // Inválido (min 0)
                PerfilId = 1
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(cliente, out results);

            // Assert
            Assert.False(isValid);
            Assert.Single(results);
            Assert.Contains(results, v => v.ErrorMessage.Contains("entre 0 e 100"));
        }

        [Fact]
        public void Cliente_DeveSerInvalido_QuandoPontuacaoRiscoAcimaDoMaximo()
        {
            // Arrange
            var cliente = new Cliente
            {
                Id = 1,
                PontuacaoRisco = 200, // Inválido (max 100)
                PerfilId = 1
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(cliente, out results);

            // Assert
            Assert.False(isValid);
            Assert.Single(results);
            Assert.Contains(results, v => v.ErrorMessage.Contains("entre 0 e 100"));
        }

    }
}
