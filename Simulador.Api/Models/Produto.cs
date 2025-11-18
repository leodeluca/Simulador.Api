namespace Simulador.Api.Models
{
    public class Produto
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Tipo { get; set; }

        public decimal Rentabilidade { get; set; }

        public string Risco { get; set; }
    }
}
