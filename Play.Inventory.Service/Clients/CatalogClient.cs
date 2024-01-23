using Play.Inventory.Service.DTOs;
using System.Text.Json;

namespace Play.Inventory.Service.Clients
{
    public class CatalogClient
    {
        private readonly HttpClient httpClient;
        private readonly string _remoteServiceBaseUrl;

        public CatalogClient(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            _remoteServiceBaseUrl = $"{configuration["CatalogUrl"]}/items/";
        }

        public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync() => await httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");        
    }
}
