using Simulador.Api.Logic.Repositories;
using static Simulador.Api.Controllers.PerfilRiscoController;

namespace Simulador.Api.Logic.Service
{
    public class PerfilRiscoService : IPerfilRiscoService
    {
        private readonly IClienteRepository repository;
        public PerfilRiscoService(IClienteRepository repository) => this.repository = repository;
        public async Task<PerfilRiscoDto> ObterPerfilRiscoAsync(int clienteId)
        {
            // Usa o repositório para buscar o cliente e o perfil
            var cliente = await repository.GetClienteComPerfilAsync(clienteId);

            if (cliente == null || cliente.Perfil == null)
            {
                return null; // Cliente não encontrado ou sem perfil associado
            }

            // Mapeia os dados da entidade para o DTO de resposta
            return new PerfilRiscoDto(
                cliente.Id, // clienteId: 123
                cliente.Perfil.Nome, // perfil: "Moderado" (do objeto Perfil)
                cliente.PontuacaoRisco, // pontuacao: 65 (da entidade Cliente)
                cliente.Perfil.Descricao // descricao: "Perfil equilibrado..." (do objeto Perfil)
            );
        }
    }
}
