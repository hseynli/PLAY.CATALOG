﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Play.Common.Interfaces;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.DTOs;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [Route("items")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> inventoryItemsRepository;
        private readonly CatalogClient catalogClient;
        private readonly IRepository<CatalogItem> catalogItemsRepository;

        public ItemsController(IRepository<InventoryItem> itemsRepository, IRepository<CatalogItem> catalogItemsRepository, CatalogClient catalogClient)
        {
            this.inventoryItemsRepository = itemsRepository;
            this.catalogClient = catalogClient;      
            this.catalogItemsRepository = catalogItemsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            //IReadOnlyCollection<CatalogItemDto> catalogItems = await catalogClient.GetCatalogItemsAsync();
            IReadOnlyCollection<InventoryItem> inventoryItemEntities = await inventoryItemsRepository.GetAllAsync(item => item.UserId == userId);
            IEnumerable<Guid> itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);
            IReadOnlyCollection<CatalogItem> catalogItemsEntities = await catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

            var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
            {
                CatalogItem item = catalogItemsEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);

                return inventoryItem.AsDto(item.Name, item.Description);
            });

            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await inventoryItemsRepository.GetAsync(item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    UserId = grantItemsDto.UserId,
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTime.UtcNow
                };

                await inventoryItemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;

                await inventoryItemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}