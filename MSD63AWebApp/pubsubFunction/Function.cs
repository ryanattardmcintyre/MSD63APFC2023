using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace HelloPubSub
{
    public class Function : ICloudEventFunction<MessagePublishedData>
    {
        ILogger<Function> _logger;
        public Function(ILogger<Function> logger)
        {
            _logger=logger;
        }

        public Task HandleAsync(CloudEvent cloudEvent, MessagePublishedData data, CancellationToken cancellationToken)
        {
            var documentId = data.Message?.TextData;
            _logger.LogInformation($"document id: {documentId}");
            
            FirestoreDb firestoreDb = FirestoreDb.Create("msd63a2023");
            _logger.LogInformation($"connected with firestore");
            DocumentReference docRef = firestoreDb.Collection("books").Document(documentId);
            _logger.LogInformation($"Retrieved document: {docRef!=null}");

            Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { "status", "not available" }
                };

            _logger.LogInformation($"Updating document");
            Task t = docRef.UpdateAsync(updates);
            _logger.LogInformation($"Updated document with status not available");

            t.Wait();

            //docRef.SetAsync(updates, SetOptions.MergeAll);
            return Task.CompletedTask;
        }
    }
}