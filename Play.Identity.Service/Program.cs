using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Play.Common.Settings;
using Play.Identity.Service.Entities;
using Play.Identity.Service.HostedServices;
using Play.Identity.Service.Settings;
var builder = WebApplication.CreateBuilder(args);

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
ServiceSettings serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
MongoDbSettings mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
IdentityServerSettings identityServerSettings = builder.Configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();

builder.Services.Configure<IdentitySettings>(builder.Configuration.GetSection(nameof(IdentitySettings)))
                .AddDefaultIdentity<ApplicationUser>(options => 
                {
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                })
                .AddRoles<ApplicationRole>()
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(mongoDbSettings.ConnectionString, serviceSettings.ServiceName);

builder.Services.AddIdentityServer(options => 
                {
                    options.Events.RaiseSuccessEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;                    
                })
                .AddAspNetIdentity<ApplicationUser>()   
                .AddInMemoryApiScopes(identityServerSettings.ApiScopes)
                .AddInMemoryApiResources(identityServerSettings.ApiResources)
                .AddInMemoryClients(identityServerSettings.Clients)
                .AddInMemoryIdentityResources(identityServerSettings.IdentityResources)
                .AddDeveloperSigningCredential();

builder.Services.AddLocalApiAuthentication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<IdentitySeedHostedService>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

//https://localhost:5003/Identity/Account/Register
//https://localhost:5003/.well-known/openid-configuration

app.UseIdentityServer();

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
