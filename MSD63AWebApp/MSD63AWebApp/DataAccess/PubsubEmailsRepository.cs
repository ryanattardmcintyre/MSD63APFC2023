using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using MSD63AWebApp.Models;
using Newtonsoft.Json;

namespace MSD63AWebApp.DataAccess
{
    public class PubsubEmailsRepository
    {
        TopicName topicName;
        public PubsubEmailsRepository(string project)
        {
            //get the queue (so we can add messages to it)
             topicName = TopicName.FromProjectTopic(project, "messages");

            if (topicName == null)
            {
                var p = PublisherServiceApiClient.Create();
                var t = p.CreateTopic("messages");
                topicName = t.TopicName;
            }
        }

        public async void PushMessage(Reservation r)
        {
            PublisherClient publisher = await PublisherClient.CreateAsync(topicName);
            var reservation = JsonConvert.SerializeObject(r);
            var pubsubMessage = new PubsubMessage
            {
                Data = ByteString.CopyFromUtf8(reservation),
                 
                Attributes =
                {
                    { "priorty", "low" }
                }
            };
            string message = await publisher.PublishAsync(pubsubMessage);

        }
    }
}
