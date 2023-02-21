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

        public async Task<IActionResult> Delete(string isbn)
        {
            try
            {
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
