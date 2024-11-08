using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Berrevoets.Play.Economy.Common.MongoDB;

public static class Extensions
{
    public static IHostApplicationBuilder AddMongo<T>(this IHostApplicationBuilder builder, string databaseName)
        where T : IEntity
    {
        // Register Bson serializers for Guid and DateTimeOffset
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        builder.AddMongoDBClient(databaseName);

        return builder;
    }

    public static IHostApplicationBuilder AddMongoRepository<T>(this IHostApplicationBuilder builder,
                                                                string collectionName) where T : IEntity
    {
        builder.Services.AddSingleton<IRepository<T>>(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            return new MongoRepository<T>(database!, collectionName);
        });

        return builder;
    }
}