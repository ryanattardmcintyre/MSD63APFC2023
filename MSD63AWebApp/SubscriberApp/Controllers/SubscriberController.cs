using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using System.Threading;
using Common.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace SubscriberApp.Controllers
{
    public class SubscriberController : Controller
    {

        //this method when called will connect with the topic "messages" on the cloud
        //subscribes
        //pull messages
        //send them out as emails
        public async Task<IActionResult> Index()
        {
            bool acknowledge = true; //true - message will be pulled permanently from the queue
                                     //false - message will be restored back into the queue once the deadline of the acknowledgement exceeds
            string projectId = "msd63a2023";
            string subscriptionId = "messages-sub";


            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);
            // SubscriberClient runs your message handle function on multiple
            // threads to maximize throughput.
            int messageCount = 0;
            string messageOutput = "";
            Task startTask = subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
            {
                string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());

                //code that sends out the email
                Reservation r = JsonConvert.DeserializeObject<Reservation>(text);

                //you have to use MailGun/Sendgrid/....

              /* RestClient client = new RestClient("https://api.mailgun.net/v3");
                client. = new HttpBasicAuthenticator("api", "");
              
                RestRequest request = new RestRequest();
                request.AddParameter("domain", "", ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                request.AddParameter("from", "ryanattarddemo@gmail.com");
                request.AddParameter("to", "ryanattard@gmail.com");
                request.AddParameter("subject", "Testing Mail Feature");
                request.AddParameter("text", $"This is to confirm that book with isbn {r.Isbn} was reserved {r.FromDt.ToLongDateString()} till {r.ToDt.ToLongTimeString()}");
                request.Method = Method.Post;
                client.ExecuteAsync(request).Wait();
                */

                messageOutput += $"Message {message.MessageId}: {text}";
                Console.WriteLine($"Message {message.MessageId}: {text}");
                Interlocked.Increment(ref messageCount);
                return Task.FromResult(acknowledge ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack);
            });
            // Run for 5 seconds.
            await Task.Delay(5000);
            await subscriber.StopAsync(CancellationToken.None);
            // Lets make sure that the start task finished successfully after the call to stop.
            await startTask;
            return Content(messageCount.ToString());
 
        }
    }
}
