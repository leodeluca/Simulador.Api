using Simulador.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Simulador.Api.Tests.Models.Tests
{
    public class PerfilTests
    {
        // Método auxiliar para facilitar a validação em todos os testes
        private bool TryValidate(object model, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            return Validator.TryValidateObject(model, context, results, true);
        }

        [Fact]
        public void Perfil_DeveSerValido_QuandoTodosOsCamposCorretos()
        {
            // Arrange
            var perfil = new Perfil
            {
                Id = 1,
                Nome = "Conservador",
                Descricao = "Descrição para clientes de baixo risco."
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(perfil, out results);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Perfil_DeveSerInvalido_QuandoNomeEhNulo()
        {
            // Arrange
            var perfil = new Perfil
            {
                Id = 1,
                Nome = null, // Inválido (Required)
                Descricao = "Uma descrição válida."
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(perfil, out results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, v => v.ErrorMessage.Contains("O nome do perfil é obrigatório"));
        }

        [Fact]
        public void Perfil_DeveSerInvalido_QuandoDescricaoCurta()
        {
            // Arrange
            var perfil = new Perfil
            {
                Id = 1,
                Nome = "Agressivo",
                Descricao = "DEV" // Inválido (MinLength 5)
            };
            ICollection<ValidationResult> results;

            // Act
            var isValid = TryValidate(perfil, out results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, v => v.ErrorMessage.Contains("entre 5 e 255 caracteres"));
        }
    }
}
