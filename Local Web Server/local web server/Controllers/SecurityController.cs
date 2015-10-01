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
    }
}