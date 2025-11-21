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
            var cliente = await repository.GetClienteComPerfilAsync(clienteId);

            if (cliente == null || cliente.Perfil == null)
            {
                return null;
            }

            return new PerfilRiscoDto(
                cliente.Id,
                cliente.Perfil.Nome,
                cliente.PontuacaoRisco,
                cliente.Perfil.Descricao
            );
        }
    }
}
