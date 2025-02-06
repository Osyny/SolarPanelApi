using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolarPanel_Api.Controllers.SolarPanels.Dtos;
using SolarPanel_Api.Controllers.SolarPanels.Models;
using SolarPanel_Api.Entities;
using SolarPanel_Api.Services.AzureTable;

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

        [HttpGet]
        public async Task<SolarPanelResponse> GetAll()
        {
            List<SolarPanelDto> solarPanels = await _azureStorageTableService.GetAll();
            SolarPanelResponse solarPanelResponse = new SolarPanelResponse()
            {
                SolarPanels = solarPanels,
                Total = solarPanels.Count,
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
                await _azureStorageTableService.Update(solarPanel.Id, solarPanel);
            }
            return response;

        }

        [HttpDelete("{Id}")]
        public async Task<InfoPanelResponse> Delete(string Id)
        {
            return await _azureStorageTableService.Delete(Id);
        }

    }
}
