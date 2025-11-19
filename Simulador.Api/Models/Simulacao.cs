using System.ComponentModel.DataAnnotations;

namespace Simulador.Api.Models
{
    public class Simulacao
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID do cliente é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID do cliente deve ser positivo.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
        public string ProdutoNome { get; set; }

        [Required(ErrorMessage = "O valor investido é obrigatório.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor investido deve ser maior que zero.")]
        public decimal ValorInvestido { get; set; }

        [Required(ErrorMessage = "O valor final é obrigatório.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor final deve ser maior que zero.")]
        public decimal ValorFinal { get; set; }

        [Required(ErrorMessage = "O prazo em meses é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O prazo em meses deve ser no mínimo 1.")]
        public int PrazoMeses { get; set; }

        [Required(ErrorMessage = "A data da simulação é obrigatória.")]
        public DateTime Data { get; set; }
    }
}
