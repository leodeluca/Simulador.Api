using System.ComponentModel.DataAnnotations;

namespace Simulador.Api.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O tipo do produto é obrigatório.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O tipo deve ter entre 2 e 50 caracteres.")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "A rentabilidade é obrigatória.")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "A rentabilidade deve ser um valor positivo.")]
        public decimal Rentabilidade { get; set; }

        [Required(ErrorMessage = "O risco do produto é obrigatório.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O risco deve ter entre 3 e 20 caracteres.")]
        public string Risco { get; set; }
    }
}
