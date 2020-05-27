using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace TwitterWorkerService
{
    public class Worker : BackgroundService
    {
        public readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IDatabaseConnection<SentimentAnalysis> database = new MongoDBConnection<SentimentAnalysis>("twitterAnalysis", "tweets"); //using a mongodb database

            while (!stoppingToken.IsCancellationRequested)
            {
                var tweets = TwitterAccess.GetTweets(); //get the 20 newest tweets of twitter with #startup
                foreach (string tweet in tweets)
                {
                    var analysis = new SentimentAnalysis(tweet); //analyce the tweet and save the result to save it in the next steps

                    if (database.Exists("text", analysis.tweetText) == false) //if the tweet has not been analyced yet
                        database.Insert(analysis); //save the tweet into the database
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
