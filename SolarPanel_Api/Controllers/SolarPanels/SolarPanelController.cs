using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using SolarPanel_Api.Controllers.SolarPanels.Dtos;
using SolarPanel_Api.Dtos;
using SolarPanel_Api.Entities;
using SolarPanel_Api.Extentions;
using SolarPanel_Api.Services.AzureTable;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SolarPanel_Api.Controllers.SolarPanels
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolarPanelController : ControllerBase
    {
        private readonly ISolarPanelTableService _azureStorageTableService;
        public SolarPanelController(ISolarPanelTableService azureStorageTableService)
        {
            _azureStorageTableService = azureStorageTableService;
        }

        [HttpGet("getAll")]
        public async Task<SolarPanelResponse> GetAll([FromQuery] DataInput input)
        {
            var solarPanels = await _azureStorageTableService.GetAll(input);
            SolarPanelResponse solarPanelResponse = new SolarPanelResponse()
            {
                SolarPanels = solarPanels,
                Total = solarPanels.ToList().Count,
            };
            return solarPanelResponse;
        }

        [HttpGet("{Id}")]
        public async Task<SolarPanelDto> GetById(string Id)
        {
            SolarPanelDto response = await _azureStorageTableService.GetById(Id);

            return response;
        }

        [HttpPost("update-create")]
        public async Task<InfoPanelResponse> CreateOrUbdate([FromBody] SolarPanelDto solarPanel)
        {
            InfoPanelResponse response = new InfoPanelResponse();

            if (solarPanel.Id == null)
            {
                response = await _azureStorageTableService.Add(solarPanel);
            }
            else
            {
                response = await _azureStorageTableService.Update(solarPanel.Id, solarPanel);
            }
            return response;

        }

        [HttpDelete("{Id}")]
        public async Task<InfoPanelResponse> Delete(string Id)
        {
            return await _azureStorageTableService.Delete(Id);
        }

        [HttpGet("configurator")]
        public async Task<ConfiguratorResponse> Configurator([FromQuery] DataConfigurator input)
        {
            ConfiguratorResponse resultCalculateNeededPanels = await _azureStorageTableService.Configurator(input);
               return resultCalculateNeededPanels;
        }
        [HttpGet("test")]
        public async Task<string> test()
        {
            return "TEST";
        }

    }
}
