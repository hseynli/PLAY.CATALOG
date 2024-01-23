using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Play.Common.Settings;
using Play.Common.Interfaces;

namespace Play.Common.MongoDb
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.DateTime));

            services.AddSingleton(serviceProvider =>
            {
                IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
                ServiceSettings serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                MongoDbSettings mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                MongoClient mongoClient = new(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });

            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T: IEntity
        {
            services.AddSingleton<IRepository<T>>(serviceProvider =>
            {
                IMongoDatabase database = serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName);
            });

            return services;
        }
    }
}