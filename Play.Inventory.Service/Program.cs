using Play.Common.MassTransit;
using Play.Common.MongoDb;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongo().AddMongoRepository<InventoryItem>("inventoryItems")
                           .AddMongoRepository<CatalogItem>("catalogitems")
                           .AddMassTransitWithRabbitMq();

AddCatalogClient(builder);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void AddCatalogClient(WebApplicationBuilder builder)
{
    builder.Services.AddHttpClient<CatalogClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["CatalogUrl"]);
    })
    .AddTransientHttpErrorPolicy(configurePolicy => configurePolicy.Or<TimeoutRejectedException>().WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryAttempt) =>
            {
                builder.Services.BuildServiceProvider().GetService<ILogger<CatalogClient>>()
                            .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}");

            }))
    .AddTransientHttpErrorPolicy(configurePolicy => configurePolicy.Or<TimeoutRejectedException>().CircuitBreakerAsync(3, TimeSpan.FromSeconds(15),
            onBreak: (outcome, timespan) =>
            {
                builder.Services.BuildServiceProvider().GetService<ILogger<CatalogClient>>().LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds...");

            },
            onReset: () =>
            {
                builder.Services.BuildServiceProvider().GetService<ILogger<CatalogClient>>().LogWarning($"Closing the circuit...");
            }))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
}