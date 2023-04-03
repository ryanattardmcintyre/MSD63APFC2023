using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace httpFunction
{
    [FirestoreData] //so that we dont need to store this data formatted as json in Firestore
    public class Book
    {
        [FirestoreProperty]
        public string Isbn { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public int Year { get; set; }
        [FirestoreProperty]
        public string Author { get; set; }
        [FirestoreProperty]
        public string Category { get; set; }

        [FirestoreProperty]
        public string Link { get; set; }

        
    }
}