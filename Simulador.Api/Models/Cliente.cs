using System.ComponentModel.DataAnnotations;

namespace Simulador.Api.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Range(0, 100, ErrorMessage = "A pontuação de risco deve estar entre 0 e 100.")]
        public int PontuacaoRisco { get; set; }

        // Chave estrangeira e propriedade de navegação
        [Required(ErrorMessage = "O ID do perfil é obrigatório.")]
        public int PerfilId { get; set; }
        public Perfil Perfil { get; set; }

    }
}
