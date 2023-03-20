using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using System.Threading;

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
            bool acknowledge = false; //true - message will be pulled permanently from the queue
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
