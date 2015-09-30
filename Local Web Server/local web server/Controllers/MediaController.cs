using Local_Web_Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Local_Web_Server.Controllers
{
    public class MediaController : Controller
    {
        // GET: Plex
        public ActionResult Plex()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Login", "Account");
        }

            [HttpPost]
        public ActionResult partialFileZone(string folder) 
        {
            DirectoryInfo di;
            List<Item> files = new List<Item>();
            if (folder == "" || folder == null)
            {
                di = new DirectoryInfo(Server.MapPath(@"\") + "FileZone");
                folder = Server.MapPath(@"\") + "FileZone";
            }
            else 
            {
                di = new DirectoryInfo(folder);
            }
            
            foreach (var fi in di.GetFiles())
            {
                Item item = new Item();
                var name = fi.Name.ToString();
                item.Title = name;
                item.Size = fi.Length;
                item.Format = fi.Extension;
                files.Add(item);
            }
            List<System.IO.DirectoryInfo> dirs = new List<System.IO.DirectoryInfo>();
            foreach (System.IO.DirectoryInfo dir in di.GetDirectories())
            {
                dirs.Add(dir);
            }
            FileZone fileClass = new FileZone();
            fileClass.Files = files;
            fileClass.CurrentDirectory = folder;
            fileClass.Folders = dirs;
            return PartialView(fileClass);
        }

        public ActionResult FileZone()
        {
            if (User.Identity.IsAuthenticated)
            {
                FileZone s = new FileZone();
                s.CurrentDirectory = Server.MapPath(@"\") + "FileZone";
                return View(s);
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public ActionResult FileZone(FileZone s)
        {
            var location = s.CurrentDirectory;
            bool isSavedSuccessfully = true;
            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {
                        string pathString;
                        if (location != null)
                        {
                            pathString = new DirectoryInfo(location).ToString();
                        }
                        else 
                        {
                            pathString = new DirectoryInfo(Server.MapPath(@"\") + "FileZone").ToString();
                        }

                        var fileName1 = Path.GetFileName(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, file.FileName);
                        file.SaveAs(path);
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }

            if (isSavedSuccessfully)
            {
                return View("FileZone", s);
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }

        [HttpPost]
        public JsonResult Delete(string location, string FileName)
        {
            if (User.Identity.IsAuthenticated)
            {
                DirectoryInfo di;
                if (location != "") 
                {
                    di = new DirectoryInfo(location); 
                } 
                else 
                { 
                    di = new DirectoryInfo(Server.MapPath(@"\") + "FileZone"); 
                }
                                
                foreach (var fi in di.GetFiles())
                { 
                    if(fi.Name == FileName)
                    {
                        string derp = fi.DirectoryName + "\\" + FileName;
                        System.IO.File.Delete(derp);
                    }
                }
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult DeleteFolder(string path)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    System.IO.Directory.Delete(path);
                }
                catch (Exception e) 
                {
                    foreach (var file in System.IO.Directory.GetFiles(path)) 
                    {
                        System.IO.File.Delete(file);
                    }
                    System.IO.Directory.Delete(path);
                }
            }
            return Json(true);
        }


        [HttpPost]
        public JsonResult CreateFolder(string location)
        {
            if (User.Identity.IsAuthenticated)
            {
                string pathString = "";
                if (location == "")
                {
                    pathString = new DirectoryInfo(Server.MapPath(@"\") + "FileZone\\New Folder").ToString();
                }
                else 
                {
                    pathString = new DirectoryInfo(location + "\\New Folder").ToString();
                }
                
                bool isExists = System.IO.Directory.Exists(pathString);

                if (!isExists)
                    System.IO.Directory.CreateDirectory(pathString);
                else 
                {
                    for (int i = 1; i < 100; i++)
                    {
                        if (location == "")
                        {
                            pathString = new DirectoryInfo(Server.MapPath(@"\") + "FileZone\\New Folder " + i).ToString();
                        }
                        else
                        {
                            pathString = new DirectoryInfo(location + "\\New Folder " + i).ToString();
                        }

                        isExists = System.IO.Directory.Exists(pathString);
                        if (!isExists)
                        {
                            System.IO.Directory.CreateDirectory(pathString);
                            break;
                        }
                    }
                }
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult RenameFolder(string path, string to)
        {
            if (User.Identity.IsAuthenticated)
            {
                int cut = 0;
                bool isExists = System.IO.Directory.Exists(path);
                if (isExists) 
                {
                    for (int i = path.Length - 1; i > 0; i--) 
                    {
                        if (path[i] == '\\') 
                        {
                            cut = i;
                            break;
                        }
                    }
                    string temp = path.Substring(0, 1+cut);
                    temp = temp + to;
                    Directory.Move(path, temp);
                }
            }
            return Json(true);
        }
    }
}