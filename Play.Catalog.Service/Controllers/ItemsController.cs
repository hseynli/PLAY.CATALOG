using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service.DTOs;
using Play.Catalog.Service.Entities;
using Play.Common;
using Play.Common.Interfaces;

namespace Play.Catalog.Service.Controllers
{
    [Route("items")]
    [ApiController]
    [Authorize(Roles = AdminRole)]
    public class ItemsController : ControllerBase
    {
        private const string AdminRole = "Admin";
        private readonly IRepository<Item> repository;
        private readonly IPublishEndpoint publishEndpoint;

        public ItemsController(IRepository<Item> repository, IPublishEndpoint publishEndpoint)
        {
            this.repository = repository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync() => Ok((await repository.GetAllAsync()).Select(p => p.AsDto()));

        [HttpGet("{id}")]
        public async Task<ItemDto> GetAsync(Guid id) => (await repository.GetAsync(id)).AsDto();

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            Item item = new Item(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTime.Now);
            await repository.CreateAsync(item);

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(GetAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            Item existingItem = await repository.GetAsync(id);

            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await repository.UpdateAsync(existingItem);

            await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            Item item = await repository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await repository.DeleteAsync(item.Id);

            await publishEndpoint.Publish(new CatalogItemDeleted(id));

            return NoContent();
        }
    }
}
