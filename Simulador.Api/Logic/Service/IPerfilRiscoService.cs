using static Simulador.Api.Controllers.PerfilRiscoController;

namespace Simulador.Api.Logic.Service
{
    public interface IPerfilRiscoService
    {
        Task<PerfilRiscoDto> ObterPerfilRiscoAsync(int clienteId);
    }
}
