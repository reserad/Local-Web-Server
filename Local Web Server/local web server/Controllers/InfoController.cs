using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Local_Web_Server.Models;

namespace Local_Web_Server.Controllers
{
    public class InfoController : Controller
    {
        // GET: Info
        public ActionResult WhoIsAtHome()
        {
            if (User.Identity.IsAuthenticated)
            {
                List<SelectListItem> obj = new List<SelectListItem>();
                obj.Add(new SelectListItem { Text = "Phone", Value = "1" });
                obj.Add(new SelectListItem { Text = "PC", Value = "2" });
                obj.Add(new SelectListItem { Text = "Other", Value = "3" });
                ViewBag.data = obj;
                return View();
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public JsonResult addAddress(string _address, string _deviceType)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (_address.Length > 0 && _deviceType.Length > 0)
                {
                    List<WhoIsOnlineModel> addresses = new List<WhoIsOnlineModel>();
                    WhoIsOnlineModel s = new WhoIsOnlineModel();
                    addresses = s.getAddresses();
                    bool check = true;
                    foreach (var item in addresses)
                    {
                        if (item.Address == _address)
                        {
                            check = false;
                        }
                    }
                    if (check)
                    {
                        s.Address = _address;
                        s.DeviceType = _deviceType;
                        s.Owner = User.Identity.Name;
                        s.CreateEntry(s);
                    }
                    return Json(true);
                }
            }
            return Json(false);
        }
        [HttpPost]
        public JsonResult deleteAddress(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                WhoIsOnlineModel s = new WhoIsOnlineModel();
                s.deleteAddressById(Convert.ToInt32(id));
                return Json(true);
            }
            return Json(false);
        }

        [HttpPost]
        public JsonResult HostName(string ip) 
        {
            return Json(MacAddressData.GetHostName(ip));
        }
    }
}