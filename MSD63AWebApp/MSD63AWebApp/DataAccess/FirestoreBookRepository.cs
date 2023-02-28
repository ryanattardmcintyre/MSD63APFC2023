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

        public async Task<string> GetBookDocumentId(string isbn)
        {
            Query allBooksQuery = db.Collection("books").WhereEqualTo("Isbn", isbn); ;
            QuerySnapshot allBooksQuerySnapshot = await allBooksQuery.GetSnapshotAsync();
            var document = allBooksQuerySnapshot.FirstOrDefault();
            if (document != null)
                return document.Id;
            else
                return null;

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

        public async Task<Book> GetBook(string isbn)
        {
            List<Book> books = new List<Book>();
            Query allBooksQuery = db.Collection("books").WhereEqualTo("Isbn", isbn); ;
            QuerySnapshot allBooksQuerySnapshot = await allBooksQuery.GetSnapshotAsync();
  
            foreach (DocumentSnapshot documentSnapshot in allBooksQuerySnapshot.Documents)
            {
                Book b = documentSnapshot.ConvertTo<Book>();
                books.Add(b);
            }

           return books.FirstOrDefault();
        }

        public async Task UpdateBook(Book b)
        {
            Query allBooksQuery = db.Collection("books").WhereEqualTo("Isbn", b.Isbn); ;
            QuerySnapshot allBooksQuerySnapshot = await allBooksQuery.GetSnapshotAsync();
            if(allBooksQuerySnapshot.Documents.Count>0)
            {
                string id = allBooksQuerySnapshot.Documents[0].Id;
                DocumentReference booksRef = db.Collection("books").Document(id);
                await booksRef.SetAsync(b);
            }
            else
            {
                throw new Exception("No books with isbn " + b.Isbn);
            }

           
        }

        public async Task DeleteBook(string isbn)
        {
            List<Book> books = new List<Book>();
            Query allBooksQuery = db.Collection("books").WhereEqualTo("Isbn", isbn); ;
            QuerySnapshot allBooksQuerySnapshot = await allBooksQuery.GetSnapshotAsync();

            string id = allBooksQuerySnapshot.Documents[0].Id;
            DocumentReference booksRef = db.Collection("books").Document(id);
            await booksRef.DeleteAsync();
        }


    }
}
