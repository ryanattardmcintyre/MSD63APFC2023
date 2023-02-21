using Google.Cloud.Firestore;
using MSD63AWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSD63AWebApp.DataAccess
{
    public class FirestoreReservationsRepository
    {
        FirestoreDb db;
        public FirestoreReservationsRepository(string project)
        {
            db = FirestoreDb.Create(project);
        }

        public async Task AddReservation(Reservation r, string isbn)
        {
            DocumentReference docRef = db.Collection("books/"+isbn+"/reservations").Document();
            await docRef.SetAsync(r);
        }
    }
}
