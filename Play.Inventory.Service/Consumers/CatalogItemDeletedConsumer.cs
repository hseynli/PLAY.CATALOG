﻿using MassTransit;
using Play.Catalog.Contracts;
using Play.Common.Interfaces;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemDeletedConsumer : IConsumer<CatalogItemDeleted>
    {
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemDeletedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
        {
            CatalogItemDeleted message = context.Message;

            CatalogItem item = await repository.GetAsync(message.ItemId);

            if (item is null)
            {
                return;
            }
            else
            {
                await repository.DeleteAsync(message.ItemId);
            }
        }
    }
}