using Simulador.Api.Models;

namespace Simulador.Api.Logic.Repositories
{
    public interface IClienteRepository
    {
        Task<Cliente> GetClienteComPerfilAsync(int clienteId);

        Task<Cliente> GetClienteById(int id);
    }
}
