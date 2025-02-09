using SolarPanel_Api.Controllers.SolarPanels.Dtos;
using SolarPanel_Api.Dtos;

namespace SolarPanel_Api.Services.AzureTable
{
    public interface ISolarPanelTableService
    {
        Task<List<SolarPanelDto>> GetAll(DataInput input);
        Task<SolarPanelDto> GetById(string Id);
        Task<InfoPanelResponse> Add(SolarPanelDto solarPanelDto);
        Task<InfoPanelResponse> Delete(string Id);
        Task<InfoPanelResponse> Update(string Id, SolarPanelDto solarPanel);
        Task<ConfiguratorResponse> Configurator(DataConfigurator input);
    }
}
