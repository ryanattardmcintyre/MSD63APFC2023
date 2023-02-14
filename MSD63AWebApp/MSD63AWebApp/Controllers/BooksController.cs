using Microsoft.AspNetCore.Mvc;
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
        public BooksController(FirestoreBookRepository _fbr)
        {
            fbr = _fbr;
        }


        //Part 1 - is called to show the user a blank page where to input the book details
        public IActionResult Create()
        {
            return View();
        }

        //Part 2 - is called when the user clicked on submit and the details are submitted to the web server
        [HttpPost]
        public IActionResult Create(Book b)
        {
            try
            {
                fbr.AddBook(b);
                TempData["success"] = "Book was added successfully";
            }
            catch (Exception e)
            {
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

       

      
    }
}
