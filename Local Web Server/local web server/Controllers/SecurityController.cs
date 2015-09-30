using Local_Web_Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Local_Web_Server.Controllers
{
    public class SecurityController : Controller
    {
        public ActionResult SecurityCam() 
        {
            if (User.Identity.IsAuthenticated) 
            { 
                return View();
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public JsonResult addCamera(string _address, string _location) 
        {
            if (User.Identity.IsAuthenticated)
            {
                if (_address.Length > 0 && _location.Length > 0) 
                {
                    List<Security> cams = new List<Security>();
                    Security s = new Security();
                    cams = s.getCameras();
                    bool check = true;
                    foreach (var item in cams) 
                    {
                        if (item.Address == _address) 
                        {
                            check = false;
                        }
                    }
                    if (check) 
                    {
                        s.Address = _address;
                        s.Location = _location;
                        s.Owner = User.Identity.Name;
                        s.CreateCamera(s);
                    }
                    return Json(true);
                }
            }
            return Json(false);
        }
        [HttpPost]
        public JsonResult deleteCamera(string id) 
        {
            if (User.Identity.IsAuthenticated)
            {
                Security s = new Security();
                s.deleteCameraById(Convert.ToInt32(id));
                return Json(true);
            }
            return Json(false);
        }

        //public ActionResult Locks() 
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return View();
        //    }
        //    return RedirectToAction("Login", "Account");
        //}

        //[HttpPost]
        //public ActionResult sendDoorCommand(string command) 
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        //closest to street
        //        if (command == "right door locked") {  }
        //        else if (command == "center door locked") { }
        //        //closest to garage
        //        else if (command == "left door locked") { }
        //    }

        //    return Json(true);
        //}

        //public ActionResult test()
        //{
        //    string text = System.IO.File.ReadAllText(Server.MapPath(@"~/Scripts/DecoderWorker.js"));
        //    return JavaScript(text);
        //}
    }
}