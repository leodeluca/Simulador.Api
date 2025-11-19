using System.ComponentModel.DataAnnotations;

namespace Simulador.Api.Models
{
    public class Investimento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID do cliente é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID do cliente deve ser positivo.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "O tipo de investimento é obrigatório.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O tipo deve ter entre 2 e 50 caracteres.")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "O valor investido é obrigatório.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "A rentabilidade é obrigatória.")]
        [Range(0, 1.0, ErrorMessage = "A rentabilidade deve estar entre 0 e 1 (0% a 100%).")]
        public decimal Rentabilidade { get; set; }

        [Required(ErrorMessage = "A data é obrigatória.")]
        public DateTime Data { get; set; }
    }
}
