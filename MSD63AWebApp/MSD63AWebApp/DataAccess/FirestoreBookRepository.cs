using Google.Cloud.Firestore;
using MSD63AWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSD63AWebApp.DataAccess
{
    public class FirestoreBookRepository
    {
        FirestoreDb db;
        public FirestoreBookRepository(string project)
        {
            db = FirestoreDb.Create(project);
        }

        public void AddBook(Book b)
        {
            DocumentReference docRef = db.Collection("books").Document();
            docRef.SetAsync(b);
        }

        public async  Task<List<Book>>  GetBooks()
        {
            List<Book> books = new List<Book>();
            Query allBooksQuery = db.Collection("books");
            QuerySnapshot allBooksQuerySnapshot = await allBooksQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in allBooksQuerySnapshot.Documents)
            {
                Book b = documentSnapshot.ConvertTo<Book>();
                books.Add(b);
            }
            return books;
        }
    }
}
