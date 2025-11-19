using Simulador.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Simulador.Api.Tests.Models.Tests
{
    public class InvestimentoTests
    {
        // Método auxiliar para facilitar a validação em todos os testes
        private bool TryValidate(object model, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            return Validator.TryValidateObject(model, context, results, true);
        }

        [Fact]
        public void Investimento_DeveSerValido_QuandoTodosOsCamposCorretos()
        {
            // Arrange
            var investimento = new Investimento
            {
                Id = 1,
                ClienteId = 1,
                Tipo = "RF",
                Valor = 1000m,
                Rentabilidade = 0.05m,
                Data = DateTime.Now
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(investimento, out results);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Investimento_DeveSerInvalido_QuandoValorZeroOuNegativo()
        {
            // Arrange
            var investimento = new Investimento
            {
                ClienteId = 1,
                Tipo = "RF",
                Valor = 0m, // Inválido (deve ser > 0.01)
                Rentabilidade = 0.05m,
                Data = DateTime.Now
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(investimento, out results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, v => v.ErrorMessage.Contains("maior que zero"));
        }

        [Fact]
        public void Investimento_DeveSerInvalido_QuandoTipoVazio()
        {
            // Arrange
            var investimento = new Investimento
            {
                ClienteId = 1,
                Tipo = "", // Inválido (Required e StringLength)
                Valor = 100m,
                Rentabilidade = 0.05m,
                Data = DateTime.Now
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(investimento, out results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, v => v.ErrorMessage.Contains("obrigatório"));
        }

        [Fact]
        public void Investimento_DeveSerInvalido_QuandoRentabilidadeForaDoRange()
        {
            // Arrange
            var investimento = new Investimento
            {
                ClienteId = 1,
                Tipo = "RF",
                Valor = 100m,
                Rentabilidade = 1.5m, // Inválido (max 1.0 ou 100%)
                Data = DateTime.Now
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(investimento, out results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, v => v.ErrorMessage.Contains("entre 0 e 1"));
        }
    }
}
