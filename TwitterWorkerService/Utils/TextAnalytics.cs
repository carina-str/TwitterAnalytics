using Azure;
using Azure.AI.TextAnalytics;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using System;

namespace TwitterWorkerService
{
    class TextAnalytics
    {
        private static readonly AzureKeyCredential credentials = new AzureKeyCredential(Credentials.azureCredentials);
        private static readonly Uri endpoint = new Uri("https://textanalysetwitter.cognitiveservices.azure.com/");

        public static DocumentSentiment DocumentSentiment(string text)
        {
            var client = new TextAnalyticsClient(endpoint, credentials);
            return client.AnalyzeSentiment(text); //analyze Sentiment and returning the result as a DocumentSentiment object
        }
    }

    class SentimentAnalysis
    {
        //SentimentAnalysis class to use as BsonDocuments for saving in MongoDB and having the opportunity to filter them
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        //Using the new ComplexTypeSerializer instead of the default one
        [BsonElement("DocumentSentiment")]
        [BsonSerializer(typeof(ComplexTypeSerializer))]
        public DocumentSentiment documentSentiment { get; set; }

        [BsonElement("Text")]
        public string tweetText { get; set; }

        [BsonConstructor]
        public SentimentAnalysis(string tweetText)
        {
            this.tweetText = tweetText;
            documentSentiment = TextAnalytics.DocumentSentiment(tweetText);
        }
    }

    //complex type serializer to serialize the DocumentSentiment object
    public class ComplexTypeSerializer : SerializerBase<DocumentSentiment>
    {
        public override DocumentSentiment Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));
            var document = serializer.Deserialize(context, args);

            var bsonDocument = document.ToBsonDocument();

            var result = BsonExtensionMethods.ToJson(bsonDocument);
            try //default constructor in class DocumentSentiment not available
            {
                return JsonConvert.DeserializeObject<DocumentSentiment>(result);
            }
            catch
            {
                return null;
            }

        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DocumentSentiment value)
        {
            var jsonDocument = JsonConvert.SerializeObject(value);
            var bsonDocument = BsonSerializer.Deserialize<BsonDocument>(jsonDocument);

            var serializer = BsonSerializer.LookupSerializer(typeof(BsonDocument));
            serializer.Serialize(context, bsonDocument.AsBsonValue);
        }
    }
}


