using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSD63AWebApp.DataAccess;
using MSD63AWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSD63AWebApp.Controllers
{
    public class BooksController : Controller
    {
        FirestoreBookRepository fbr;
        ILogger<BooksController> _logger;
        public BooksController(FirestoreBookRepository _fbr, ILogger<BooksController> logger)
        {
            _logger = logger;
            fbr = _fbr;
        }


        //Part 1 - is called to show the user a blank page where to input the book details
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        //Part 2 - is called when the user clicked on submit and the details are submitted to the web server
        [HttpPost][Authorize]
        public IActionResult Create(Book b, IFormFile file)
        {
            try
            {
                _logger.LogInformation($"User {User.Identity.Name} is uploading a file with filename {file.FileName}");
                throw new Exception("Error on purpose");
                string newFilename;
                //1) ebook is going to be stored in the cloud storage i.e. in the bucket with name msd63a2023ra
                if (file != null)
                {
                    var storage = StorageClient.Create();
                    using var fileStream = file.OpenReadStream(); //reads the uploaded file from the server's memory
                    newFilename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                  
                    _logger.LogInformation($"File {file.FileName} has been renamed to {newFilename}");
                    
                    storage.UploadObject("msd63a2023ra", newFilename, null, fileStream);   
                    //https://storage.googleapis.com/msd63a2023ra/security.jpg //will work only if it is a uniform bucket with allUsers enabled
                    b.Link = $"https://storage.googleapis.com/{"msd63a2023ra"}/{newFilename}";

                    _logger.LogInformation($"File {newFilename} has been uploaded. Link {b.Link}");
                }

                //2) will store the book details/info in the NoSql database (Firestore)
                _logger.LogInformation($"File {file.FileName} will be saved to db");
                fbr.AddBook(b);
                _logger.LogInformation($"File {file.FileName} with isbn {b.Isbn} has been saved in the database");
                TempData["success"] = "Book was added successfully";
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"File with filename {file.FileName} was either not saved or not saved in db");
                //log the exceptions
                TempData["error"] = "Book was not added";
            }
            return View();
        }

        public IActionResult Index()
        {
            Task<List<Book>> t = fbr.GetBooks();
            
            var list = t.Result;
            return View(list);
        }

        public async Task<IActionResult> Delete(string isbn)
        {
            try
            {
                var book = await fbr.GetBook(isbn);
                string link = book.Link; //http://xxxxxxxxx/bucketname/nameOffile.pdf

                var storage = StorageClient.Create();
                string objectName = System.IO.Path.GetFileName(link);
                storage.DeleteObject("msd63a2023ra", objectName);


                await fbr.DeleteBook(isbn);
                TempData["success"] = "Book was deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Book was not deleted";
            }

            return RedirectToAction("Index");
        
        }
       
        //part 1 - this method will be called when the link Edit is clicked and a page will be
        //shown to the user with the original data in the textboxes
        public async Task<IActionResult> Edit(string isbn)
        {
            var book = await fbr.GetBook(isbn);
            return View(book);
            
        }

        //Part 2- this method will be called after the user filled in the data in the textboxes and
        //clicked submit
        [HttpPost]
        public async Task<IActionResult> Edit(Book b)
        {
            try
            {
                await fbr.UpdateBook(b);
                TempData["success"] = "Book was deleted successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Book was not edited";
                return View(b);
            }

            
        }


        public async Task<IActionResult> Details(string isbn)
        {
            var b = await fbr.GetBook(isbn);
            return View(b);
        }

      
    }
}
