namespace Simulador.Api.Models
{
    public class Simulacao
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }

        public string ProdutoNome { get; set; }

        public decimal ValorInvestido { get; set; }

        public decimal ValorFinal { get; set; }

        public int PrazoMeses { get; set; }

        public DateTime Data { get; set; }
    }
}
