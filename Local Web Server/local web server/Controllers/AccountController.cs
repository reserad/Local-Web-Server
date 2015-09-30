using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Local_Web_Server.Models;
namespace Local_Web_Server.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "CalendarEvents");
            }
            return View();
        }

        public ActionResult Settings()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    SettingsModel settings = new SettingsModel();
                    settings = settings.getServiceSetting(User.Identity.Name);
                    return View(settings);
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Settings(SettingsModel settings)
        {
            if (User.Identity.IsAuthenticated)
            {
                settings.SaveSettings(User.Identity.Name, settings.Service);
                return View(settings);
            }

            return RedirectToAction("Login", "Account");
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Models.User user)
        {
            user.CreateAccount(user.UserName, user.Password, user.Email);
            if (user.IsValid(user.UserName, user.Password))
            {
                FormsAuthentication.SetAuthCookie(user.UserName, true);
                return RedirectToAction("Index", "CalendarEvents");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.User user)
        {

            if (ModelState.IsValid)
            {
                if (user.IsValid(user.UserName, user.Password))
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, true);
                    return RedirectToAction("Index", "CalendarEvents");
                }
                else
                {
                    ModelState.AddModelError("", "Username and or password invalid");
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
        public ActionResult Messages()
        {
            if (User.Identity.IsAuthenticated)
            {
                User userlist = new User();
                var listUsers = userlist.AllUsers();
                List<SelectListItem> obj = new List<SelectListItem>();
                int count = 0;
                foreach (var item in listUsers)
                {
                    if (User.Identity.Name != item.UserName)
                    {
                        obj.Add(new SelectListItem { Text = item.UserName, Value = count.ToString() });
                        count++;
                    }
                }
                ViewBag.data = obj;
                return View();
            }
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public JsonResult MarkRead()
        {
            if (User.Identity.IsAuthenticated)
            {
                MessagesModel message = new MessagesModel();
                message.MarkAsRead(User.Identity.Name);
            }
            return Json(true);
        }
        [HttpPost]
        public JsonResult SendMessage(string message, string to)
        {
            MessagesModel _message = new MessagesModel();
            _message.SendMessage(to, User.Identity.Name, message, System.DateTime.Now);
            return Json(true);
        }
        [HttpGet]
        public ActionResult getOrganizedMessages()
        {
            MessagesModel message = new MessagesModel();
            var messages = message.RecieveMessages(User.Identity.Name);
            List<string> Recipients = new List<string>();
            List<List<MessagesModel>> organizedMessages = new List<List<MessagesModel>>();
            for (int i = 0; i < messages.Count(); i++)
            {
                if (messages[i].From != User.Identity.Name && !Recipients.Contains(messages[i].From))
                {
                    Recipients.Add(messages[i].From);
                }
                if (messages[i].To != User.Identity.Name && !Recipients.Contains(messages[i].To))
                {
                    Recipients.Add(messages[i].To);
                }
            }
            foreach (var recipient in Recipients)
            {
                List<MessagesModel> item = new List<MessagesModel>();
                for (int i = 0; i < messages.Count(); i++)
                {
                    if (messages[i].From.Contains(recipient) || messages[i].To.Contains(recipient))
                    {
                        item.Add(messages[i]);
                    }
                }
                organizedMessages.Add(item);
            }
            User userlist = new User();
            var listUsers = userlist.AllUsers();
            List<SelectListItem> obj = new List<SelectListItem>();
            int count = 0;
            foreach (var item in listUsers)
            {
                if (User.Identity.Name != item.UserName)
                {
                    obj.Add(new SelectListItem { Text = item.UserName, Value = count.ToString() });
                    count++;
                }
            }
            ViewBag.data = obj;
            MessagePageModel m = new MessagePageModel();
            m.organizedMessages = organizedMessages;

            return PartialView(m);
        }
    }
}