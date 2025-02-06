using Azure;
using Azure.Data.Tables;
using SolarPanel_Api.Controllers.SolarPanels.Dtos;
using SolarPanel_Api.Controllers.SolarPanels.Models;
using SolarPanel_Api.Entities;
using System.Linq.Expressions;

namespace SolarPanel_Api.Services.AzureTable
{
    public class SolarPanelTableService : ISolarPanelTableService
    {
        private readonly TableClient _tableClient;
        private const string partionKey = "solarpanel";

        public SolarPanelTableService(TableServiceClient tableServiceClient)
        { 
            _tableClient = tableServiceClient.GetTableClient("solarpaneldemo");
        }

        public async Task<List<SolarPanelDto>> GetAll()
        {
            List<SolarPanelDto> solarPanels = new List<SolarPanelDto>();
            var tableResult = _tableClient.QueryAsync<SolarPanel>(filter: "");

            await foreach (var row in tableResult)
            {
                solarPanels.Add(new SolarPanelDto
                {
                    Id = row.RowKey,
                    Width = row.Width,
                    Length = row.Length,
                    Power = row.Power,
                    Stock = row.Stock,

                });
            }
            return solarPanels;
        }
        public async Task<SolarPanelDto> GetById(string Id)
        {
            var entity = await _tableClient.GetEntityAsync<SolarPanel>(partionKey, Id);

            SolarPanelDto solarPanelDto = new SolarPanelDto
            {
                Id = entity.Value.RowKey,
                Width = entity.Value.Width,
                Length = entity.Value.Length,
                Power = entity.Value.Power,
                Stock = entity.Value.Stock,
            };
            return solarPanelDto;
        }
        public async Task<InfoPanelResponse> Add(SolarPanelDto solarPanelDto)
        {
            var entity = new SolarPanel
            {
                RowKey = Guid.NewGuid().ToString(),

                Width = solarPanelDto.Width,
                Length = solarPanelDto.Length,
                Power = solarPanelDto.Power,
                Stock = solarPanelDto.Stock,

                PartitionKey = partionKey
            };
            InfoPanelResponse result = new InfoPanelResponse();
            try
            {
                await _tableClient.CreateIfNotExistsAsync();
                var response = await _tableClient.AddEntityAsync<SolarPanel>(entity);
            }
            catch (RequestFailedException ex)
            {
                result.Error = ex.Message;
            }
           
            return result;
        }

        public async Task<InfoPanelResponse> Delete(string Id)
        {
            InfoPanelResponse result = new InfoPanelResponse();
            try
            {
                var entity = await _tableClient.GetEntityAsync<SolarPanel>(partionKey, Id);
                await _tableClient.DeleteEntityAsync(entity.Value.PartitionKey, entity.Value.RowKey);
                
            }
            catch (RequestFailedException ex)
            {
                result.Error = ex.Message;
            }
           
            return result;
        }       

        public async Task<InfoPanelResponse> Update(string Id, SolarPanelDto solarPanel)
        {
            InfoPanelResponse result = new InfoPanelResponse();
            try
            {
                var entity = await _tableClient.GetEntityAsync<SolarPanel>(partionKey, Id);

                await _tableClient.DeleteEntityAsync(entity.Value.PartitionKey, entity.Value.RowKey);
                var currentSolarPanel = await _tableClient.GetEntityAsync<SolarPanel>(partionKey, Id);
                currentSolarPanel.Value.Width = solarPanel.Width;
                currentSolarPanel.Value.Length = solarPanel.Length;
                currentSolarPanel.Value.Power = solarPanel.Power;
                currentSolarPanel.Value.Stock = solarPanel.Stock;

                await _tableClient.UpdateEntityAsync<SolarPanel>(currentSolarPanel, currentSolarPanel.Value.ETag);

            }
            catch (RequestFailedException ex)
            {
                result.Error = ex.Message;
            }
           
            return result;

        }

        public async Task<ConfiguratorResponse> Configurator(int WidthRoof, int LengthRoof)
        {
            ConfiguratorResponse result = new ConfiguratorResponse();
            return result;
        }
    }
}
