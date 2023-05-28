using MAuth.AspNetCore.Models.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MAuth.AspNetCore.Mongo
{
    public class MAuthMongoDbContext
    {
        private readonly IOptions<MongoOptions> options;
        private string databaseName;
        private MongoClient mongoClient;
        public IMongoDatabase Database => mongoClient.GetDatabase(databaseName);
        public MAuthMongoDbContext(
            MongoClient mongoClient,
            IOptions<MongoOptions> options
            )
        {
            this.mongoClient = mongoClient;
            this.options = options;
            MongoUrl url = new MongoUrl(options.Value.Connection);
            databaseName = url.DatabaseName;
        }
        public async Task EnsureDatabaseAndConfiguration()
        {
            //await Database.GetCollection<QuestionNaire>(GetCollectionName(CollectionNames.QuestionNaire))
            //.Indexes
            //.CreateManyAsync(new[] {
            //new CreateIndexModel<QuestionNaire>(Builders<QuestionNaire>.IndexKeys.Ascending(p => p.Id))
            //});
            //await Database.GetCollection<QuestionNaire>(CollectionNames.QuestionNaire)
            //.Indexes
            //.CreateManyAsync(new[] {
            //new CreateIndexModel<QuestionNaire>(Builders<QuestionNaire>.IndexKeys.Ascending(p => p.Name),new CreateIndexOptions{ Unique=true})
            //});
        }
        public IMongoCollection<T> GetCollection<T>(string name) where T : class => Database.GetCollection<T>(GetCollectionName(name));

        public string GetCollectionName(string name)
        {
            if (string.IsNullOrWhiteSpace(options.Value.Prefix))
            {
                return name;
            }
            else
            {
                return options.Value.Prefix + name;
            }
        }
    }
}
