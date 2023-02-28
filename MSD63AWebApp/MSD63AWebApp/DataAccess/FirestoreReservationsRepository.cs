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
        FirestoreBookRepository _fbr;
        public FirestoreReservationsRepository(string project, FirestoreBookRepository fbr)
        {
            db = FirestoreDb.Create(project);
            _fbr = fbr;
        }

        public async Task AddReservation(Reservation r)
        {
            var docId = await _fbr.GetBookDocumentId(r.Isbn);

            DocumentReference docRef = 
                db.Collection("books/"+ docId + "/reservations").Document();
            await docRef.SetAsync(r);
        }

        public async Task<List<Reservation>> GetReservations(string isbn)
        {
            var docId = await _fbr.GetBookDocumentId(isbn);

            List<Reservation> reservations = new List<Reservation>();
            Query allReservationsQuery = db.Collection("books/" + docId + "/reservations");
            QuerySnapshot allReservationsQuerySnapshot = await allReservationsQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in allReservationsQuerySnapshot.Documents)
            {
                Reservation r = documentSnapshot.ConvertTo<Reservation>();
                reservations.Add(r);
            }
            return reservations;

        }

    }
}
