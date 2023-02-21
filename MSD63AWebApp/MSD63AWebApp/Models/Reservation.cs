using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSD63AWebApp.Models
{
    [FirestoreData]
    public class Reservation
    {
        [FirestoreProperty]
        public string User { get; set; }

        [FirestoreProperty]
        public DateTime From { get; set; }

        [FirestoreProperty]
        public string FromStr { get; set; }

        [FirestoreProperty]
        public DateTime To { get; set; }

        [FirestoreProperty]
        public string ToStr { get; set; }

        [FirestoreProperty]
        public string Isbn { get; set; }
    }
}
