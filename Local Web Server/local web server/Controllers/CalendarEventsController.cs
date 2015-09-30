using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Local_Web_Server.Models;
using System.Text.RegularExpressions;

namespace Local_Web_Server.Controllers
{
    
    public class CalendarEventsController : Controller
    {

        // GET: CalendarEvents
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name.ToString();
                CalendarEvents Events = new CalendarEvents();
                List<CalendarEvents> returnedList = new List<CalendarEvents>();
                var list = Events.getEvents();

                foreach (var items in list)
                {
                    returnedList.Add(items);
                }

                return View(returnedList);
            }
            return RedirectToAction("Login", "Account");
        }

        // GET: CalendarEvents/Create
        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
                return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public ActionResult Create(CalendarEvents calendar)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    calendar.Creator = User.Identity.Name;
                    calendar.CreateEvent(calendar);
                    return RedirectToAction("Index");
                }
                return View(calendar);
            }
            return RedirectToAction("Login", "Account");
        }

        // GET: CalendarEvents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CalendarEvents Events = new CalendarEvents();

                Events = Events.getEventByID(Convert.ToInt32(id));

                return View(Events);
            }
            return RedirectToAction("Login", "Account");
        }

        // POST: CalendarEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(CalendarEvents Events)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    Events.UpdateEvent(Events);
                    return RedirectToAction("Index");
                }
                return View(Events);
            }
            return RedirectToAction("Login", "Account");
        }
        // POST: CalendarEvents/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                CalendarEvents Events = new CalendarEvents();
                Events.deleteEventByID(id);
                return Json(true);
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
