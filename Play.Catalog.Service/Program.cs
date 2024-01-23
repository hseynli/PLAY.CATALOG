using Play.Catalog.Service.Entities;
using Play.Common.MongoDb;
using Play.Common.Settings;

var builder = WebApplication.CreateBuilder(args);

ServiceSettings serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

builder.Services.AddMongo().AddMongoRepository<Item>("items");


builder.Services.AddControllers(options => 
{
    options.SuppressAsyncSuffixInActionNames = false;
});
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
