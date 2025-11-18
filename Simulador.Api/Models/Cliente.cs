namespace Simulador.Api.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        public int PontuacaoRisco { get; set; }

        // Chave estrangeira e propriedade de navegação
        public int PerfilId { get; set; }
        public Perfil Perfil { get; set; }

    }
}
