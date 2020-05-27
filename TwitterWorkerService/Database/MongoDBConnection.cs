using MongoDB.Driver;
using System;

namespace TwitterWorkerService
{
    class MongoDBConnection<T> : IDatabaseConnection<T>
    {
        MongoClient dbClient = new MongoClient(Credentials.mongoDbConnectingString);
        IMongoCollection<T> collection;
        public MongoDBConnection(string databaseName, string collectionName)
        {
            var database = dbClient.GetDatabase(databaseName);
            collection = database.GetCollection<T>(collectionName);
        }

        public void Delete(T value)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string fieldDefinition, string value)
        {
            var filter = Builders<T>.Filter.Eq(fieldDefinition, value);
            return collection.Find(filter).Any();
        }

        public void Insert(T value)
        {
            collection.InsertOne(value);
        }

        public void Update(T value)
        {
            throw new NotImplementedException();
        }
    }
}
