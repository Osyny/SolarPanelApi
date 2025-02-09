using Azure;
using Azure.Data.Tables;
using SolarPanel_Api.Controllers.SolarPanels.Dtos;
using SolarPanel_Api.Dtos;
using SolarPanel_Api.Entities;
using SolarPanel_Api.Extentions;
using System.Collections.Generic;
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

        public async Task<List<SolarPanelDto>> GetAll(DataInput input = null)
        {
            List<SolarPanelDto> solarPanels = new List<SolarPanelDto>();

            var tableResult = _tableClient.QueryAsync<SolarPanel>(filter: "");

            await foreach (var row in tableResult)
            {
                if(solarPanels != null)
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
            }
            if(input != null)
            {
                solarPanels = await GetSortQuery(input, solarPanels.AsQueryable());
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

        public async Task<ConfiguratorResponse> Configurator(DataConfigurator input)
        {
            ConfiguratorResponse output = new ConfiguratorResponse();

            var allPannels = await GetAll();
            foreach (var pannel in allPannels) 
            {
                //int columns = input.WidthRoof / pannel.Width;
                //int rows = input.LengthRoof / pannel.Length;
                //int pannels = columns * rows;
                int pannels = (int)(input.WidthRoof / pannel.Width) * (int)(input.LengthRoof / pannel.Length);
                pannels = pannels > pannel.Stock ? pannel.Stock : pannels;
                int power = pannels * pannel.Power;
                if (power > output.TotalPower)
                {
                    output.Panel = pannel;
                    output.Count = pannels;
                    output.TotalPower = power;
                }
            }

            Console.WriteLine("----------- Best configuration for this roof -----------");
            Console.WriteLine($"Pannel parameters: width - {output.Panel.Width}, lenght - {output.Panel.Length}, power - {output.Panel.Power}");
            Console.WriteLine($"Needed pannels: {output.Panel.Stock}");
            Console.WriteLine($"Total power: {output.TotalPower}\n--------------------------------------------");

            return output;
        }

        private async Task<List<SolarPanelDto>> GetSortQuery(DataInput input, IQueryable<SolarPanelDto> panels)
        {
            var parse = input?.Sorting?.Split(" ");

            IQueryable<SolarPanelDto>? sortPanels = null;

            if (parse != null && parse.Count() > 1)
            {
                var type = parse[0].First().ToString().ToUpper() + parse[0].Substring(1);
                var propertyInfo = typeof(SolarPanel).GetProperty(type);

                switch (parse[1])
                {
                    case "asc":
                        sortPanels = panels.OrderByField(type, true).Skip((int)(input?.Skip)).
                              Take((int)(input?.Rows));

                        return sortPanels.ToList();
                    case "desc":
                        sortPanels = panels.OrderByField(type, false).Skip((int)(input?.Skip)).
                            Take((int)(input?.Rows));
                        return sortPanels.ToList();
                }
            }
            else
            {
                sortPanels = panels.OrderBy(p => p.Stock).Skip((int)(input?.Skip)).
                    Take((int)(input?.Rows));
            }
            return sortPanels.ToList();
        }
    }
}
