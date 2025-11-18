namespace Simulador.Api.Models
{
    public class Investimento
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Tipo { get; set; }
        public decimal Valor { get; set; }
        public decimal Rentabilidade { get; set; }
        public DateTime Data { get; set; }
    }
}
