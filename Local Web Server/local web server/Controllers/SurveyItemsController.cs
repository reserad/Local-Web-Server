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
    public class SurveyItemsController : Controller
    {
        public JsonResult Vote(int ID, string choice) 
        {
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name.ToString();
                SurveyItems Survey = new SurveyItems();
                Survey.Vote(userName, ID, choice);
            }
            return Json(true);
        }
        // GET: SurveyItems
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name.ToString();
                SurveyItems Survey = new SurveyItems();
                List<SurveyItems> returnedList = new List<SurveyItems>();
                var list = Survey.getSurveys();

                foreach (var items in list) 
                {
                    returnedList.Add(items);
                }

                return View(returnedList);
            }
            return RedirectToAction("Login", "Account");
        }
        public ActionResult PastSurveys() 
        {
            if (User.Identity.IsAuthenticated)
            {
                SurveyItems Survey = new SurveyItems();
                List<SurveyItems> returnedList = new List<SurveyItems>();

                var list = Survey.getSurveys();

                foreach (var items in list)
                {
                    returnedList.Add(items);
                }

                return View(returnedList);
            }
            return RedirectToAction("Login", "Account");
        }

        // GET: SurveyItems/Create
        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Option1,Option2,Option3,Option4,EndDate")] SurveyItems SurveyItems)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    SurveyItems.Creator = User.Identity.Name;
                    SurveyItems.CreateSurvey(SurveyItems);
                    return RedirectToAction("Index");
                }

                return View(SurveyItems);
            }
            return RedirectToAction("Login", "Account");
        }

        // POST: SurveyItems/Delete/5
        [HttpPost]
        public JsonResult Delete(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                Regex regex = new Regex(@"\d+");
	            Match match = regex.Match(id.ToString());
	            if (match.Success)
	            {
                    SurveyItems SurveyItems = new SurveyItems();
                    SurveyItems.deleteSurveysByID(Convert.ToInt32(id));
                }
            }
            return Json(true);
        }
    }
}
