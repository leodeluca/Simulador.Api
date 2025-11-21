using Simulador.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Simulador.Api.Tests.Models.Tests
{
    public class ProdutoTests
    {
        private bool TryValidate(object model, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            return Validator.TryValidateObject(model, context, results, true);
        }

        [Fact]
        public void Produto_DeveSerValido_QuandoTodosOsCamposCorretos()
        {
            // Arrange
            var produto = new Produto
            {
                Id = 1,
                Nome = "CDB 100% DI",
                Tipo = "Renda Fixa",
                Rentabilidade = 0.10m,
                Risco = "Baixo"
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(produto, out results);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Produto_DeveSerInvalido_QuandoNomeEhNuloOuVazio()
        {
            // Arrange
            var produto = new Produto
            {
                Nome = "", // Inválido (Required e StringLength)
                Tipo = "RF",
                Rentabilidade = 0.05m,
                Risco = "Baixo"
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(produto, out results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, v => v.ErrorMessage.Contains("O nome do produto é obrigatório"));
        }

        [Fact]
        public void Produto_DeveSerInvalido_QuandoRentabilidadeNegativa()
        {
            // Arrange
            var produto = new Produto
            {
                Nome = "Tesouro",
                Tipo = "RF",
                Rentabilidade = -0.01m, // Inválido (Range > 0)
                Risco = "Baixo"
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(produto, out results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, v => v.ErrorMessage.Contains("deve ser um valor positivo"));
        }

        [Fact]
        public void Produto_DeveSerInvalido_QuandoRiscoCurto()
        {
            // Arrange
            var produto = new Produto
            {
                Nome = "Ação X",
                Tipo = "RV",
                Rentabilidade = 0.15m,
                Risco = "R" // Inválido (MinLength 3)
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(produto, out results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, v => v.ErrorMessage.Contains("O risco deve ter entre 3 e 20 caracteres"));
        }
    }
}
