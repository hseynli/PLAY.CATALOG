using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.DTOs;
using Play.Catalog.Service.Entities;
using Play.Common;
using Play.Common.Interfaces;

namespace Play.Catalog.Service.Controllers
{
    [Route("items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> repository;

        public ItemsController(IRepository<Item> repository)
        {
            this.repository = repository;
        }

        private static readonly List<ItemDto> items = new()
        {
            new ItemDto(Guid.NewGuid(), "Potion", "Restore a small amount of HP", 5, DateTime.UtcNow),
            new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTime.UtcNow),
            new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, DateTime.UtcNow),
        };

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync() => (await repository.GetAllAsync()).Select(p => p.AsDto());

        [HttpGet("{id}")]
        public async Task<ItemDto> GetAsync(Guid id) => (await repository.GetAsync(id)).AsDto();

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            Item item = new Item(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTime.Now);
            await repository.CreateAsync(item);

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

            return NoContent();
        }
    }
}
