using Simulador.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Simulador.Api.Tests.Models.Tests
{
    public class SimulacaoTests
    {
        // Método auxiliar para facilitar a validação em todos os testes
        private bool TryValidate(object model, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            return Validator.TryValidateObject(model, context, results, true);
        }

        [Fact]
        public void Simulacao_DeveSerValida_QuandoTodosOsCamposCorretos()
        {
            // Arrange
            var simulacao = new Simulacao
            {
                Id = 1,
                ClienteId = 1,
                ProdutoNome = "CDB",
                ValorInvestido = 1000m,
                ValorFinal = 1100m,
                PrazoMeses = 12,
                Data = DateTime.Now
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(simulacao, out results);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Simulacao_DeveSerInvalida_QuandoValorInvestidoEhZero()
        {
            // Arrange
            var simulacao = new Simulacao
            {
                ClienteId = 1,
                ProdutoNome = "CDB",
                ValorInvestido = 0m, // Inválido (deve ser > 0.01)
                ValorFinal = 0m,
                PrazoMeses = 12,
                Data = DateTime.Now
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(simulacao, out results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, v => v.ErrorMessage.Contains("O valor investido deve ser maior que zero"));
        }

        [Fact]
        public void Simulacao_DeveSerInvalida_QuandoPrazoMesesZero()
        {
            // Arrange
            var simulacao = new Simulacao
            {
                ClienteId = 1,
                ProdutoNome = "CDB",
                ValorInvestido = 100m,
                ValorFinal = 110m,
                PrazoMeses = 0, // Inválido (mínimo 1)
                Data = DateTime.Now
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(simulacao, out results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, v => v.ErrorMessage.Contains("mínimo 1"));
        }

        [Fact]
        public void Simulacao_DeveSerInvalida_QuandoProdutoNomeVazio()
        {
            // Arrange
            var simulacao = new Simulacao
            {
                ClienteId = 1,
                ProdutoNome = "", // Inválido (Required e StringLength)
                ValorInvestido = 100m,
                ValorFinal = 110m,
                PrazoMeses = 12,
                Data = DateTime.Now
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(simulacao, out results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, v => v.ErrorMessage.Contains("O nome do produto é obrigatório"));
        }
    }
}
