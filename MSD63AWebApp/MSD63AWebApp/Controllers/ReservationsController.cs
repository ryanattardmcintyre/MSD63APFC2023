using Common.Models;
using Microsoft.AspNetCore.Mvc;
using MSD63AWebApp.DataAccess;
using MSD63AWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSD63AWebApp.Controllers
{
    public class ReservationsController : Controller
    {
        FirestoreBookRepository _fbr;
        FirestoreReservationsRepository _frr;
        PubsubEmailsRepository _pser;
        public ReservationsController(FirestoreBookRepository fbr, FirestoreReservationsRepository frr,
            PubsubEmailsRepository pser)
        {
            _fbr = fbr;
            _frr = frr;
            _pser = pser;
        }


        public async Task<IActionResult> List(string isbn)
        {
            var list = await _frr.GetReservations(isbn);
            return View(list);
        }

        //ViewBag is used to pass data to page when there is no redirection
        //Tempdata[] is used to pass data to page when there is even a redirection
        public IActionResult Create(string isbn)
        {
            //Book myBook = await _fbr.GetBook(isbn);
            ViewBag.Isbn = isbn;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Reservation r, DateTime from, DateTime to)
        {
            try
            {
                //convert from DateTime to Google.cloud.firestore.timestamp
                r.From = Google.Cloud.Firestore.Timestamp.FromDateTime(from.ToUniversalTime());
                r.To = Google.Cloud.Firestore.Timestamp.FromDateTime(to.ToUniversalTime());

                await _frr.AddReservation(r);
                TempData["success"] = "Reservation added successfully";

                _pser.PushMessage(r); //as soon as the reservation is created, we push its details onto the queue to be sent as an email

                //redirect to Books/Index
                return RedirectToAction("Index", "Books");
            }
            catch (Exception ex)
            {
                //logging the exception
                TempData["error"] = "Reservation was not saved";
                return View(r);
            }
        
        }
    }
}
