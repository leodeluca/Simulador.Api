using System.ComponentModel.DataAnnotations;

namespace Simulador.Api.Models
{
    public class Perfil
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do perfil é obrigatório.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 50 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição do perfil é obrigatória.")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "A descrição deve ter entre 5 e 255 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

    }
}
