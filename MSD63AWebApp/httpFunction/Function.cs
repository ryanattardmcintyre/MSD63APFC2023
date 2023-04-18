using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.Extensions.Logging;

namespace httpFunction;

public class Function : IHttpFunction
{

ILogger<Function> _logger;
public Function(ILogger<Function> logger) =>
    _logger = logger;

    /// <summary>
    /// Logic for your function goes here.
    /// </summary>
    /// <param name="context">The HTTP context, containing the request and the response.</param>
    /// <returns>A task representing the asynchronous operation.</returns>

//dotnet build //builds/compiles the function
//dotnet run //runs the application on a given url
//dotnet add package Google.Cloud.Firestore
//dotnet add package Google.Cloud.Firestore.V1

    public async Task HandleAsync(HttpContext context)
    {
 //   System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
    //          "msd63a2023-19b02b290325.json");

        _logger.LogInformation("Function has been triggered");

        HttpRequest request = context.Request;
            // Check URL parameters for "message" field
            string isbn = request.Query["isbn"];
 
            Book b = await GetBook(isbn);

            await context.Response.WriteAsync($"Book requested has these details: Name: {b.Name}, Author: {b.Author}");
    }


      public async Task<Book> GetBook(string isbn)
        {
           var db = FirestoreDb.Create("msd63a2023");

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
}
