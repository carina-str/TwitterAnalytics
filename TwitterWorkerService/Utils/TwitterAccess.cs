using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace TwitterWorkerService
{
    class TwitterAccess
    {
        private static readonly string credentials = Credentials.twitterCredentials;
        private static string Authorization()
        {
            string access_token = "";
            var post = WebRequest.Create("https://api.twitter.com/oauth2/token") as HttpWebRequest;
            post.Method = "POST";
            post.ContentType = "application/x-www-form-urlencoded";
            post.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;
            var reqbody = Encoding.UTF8.GetBytes("grant_type=client_credentials");
            post.ContentLength = reqbody.Length;
            using (var req = post.GetRequestStream()) //getting the encoded url
            {
                req.Write(reqbody, 0, reqbody.Length);
            }
            try
            {
                string respbody = null;
                using (var resp = post.GetResponse().GetResponseStream())
                {
                    var respR = new StreamReader(resp);
                    respbody = respR.ReadToEnd();
                }
                access_token = respbody.Substring(respbody.IndexOf("access_token\":\"") + "access_token\":\"".Length, respbody.IndexOf("\"}") - (respbody.IndexOf("access_token\":\"") + "access_token\":\"".Length));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in TwitterAccess.authorization(): " + e.Message);
            }
            return access_token;
        }

        public static List<string> GetTweets()
        {
            int tweetCountLimit = 30;
            List<string> list = new List<string>();
            string access_token = Authorization(); //getting the access_token
                                                   //getting the tweets as a search request limited to 30 tweets in extended mode
            var tweets = WebRequest.Create("https://api.twitter.com/1.1/search/tweets.json?q=%23startup&count=" + tweetCountLimit + "&include_entities=false&tweet_mode=extended") as HttpWebRequest;
            tweets.Method = "GET";
            tweets.Headers[HttpRequestHeader.Authorization] = "Bearer " + access_token;
            try
            {
                string respbody = null;
                using (var resp = tweets.GetResponse().GetResponseStream())//there request sends
                {
                    var respR = new StreamReader(resp);
                    respbody = respR.ReadToEnd();
                }

                var jo = JObject.Parse(respbody); //saving the json into a jobject to use as an json array

                for (int i = 0; i < jo.Count; i++)
                    list.Add(jo["statuses"][i]["full_text"].Value<string>()); //getting the full_text of every tweet as a string; saving in a list object
            }
            catch (Exception e) //401 (access token invalid or expired)
            {
                Console.WriteLine("Error in TwitterAccess.getTweets(): " + e.Message);
            }
            return list;
        }
    }
}
