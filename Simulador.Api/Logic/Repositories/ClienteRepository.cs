using Microsoft.EntityFrameworkCore;
using Simulador.Api.Data;
using Simulador.Api.Models;

namespace Simulador.Api.Logic.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext dbContext;
        public ClienteRepository(AppDbContext dbContext) => this.dbContext = dbContext;

        public async Task<Cliente> GetClienteComPerfilAsync(int clienteId)
        {
            return await dbContext.Clientes
                .Include(c => c.Perfil)
                .FirstOrDefaultAsync(c => c.Id == clienteId);
        }

        public async Task<Cliente> GetClienteById(int id)
        {
            return await dbContext.Clientes
                                 .Include(c => c.Perfil)
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
