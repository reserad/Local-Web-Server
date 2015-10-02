using Local_Web_Server.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Enums;
using SpotifyAPI.Local.Models;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace Local_Web_Server.Controllers
{
    public class MediaController : Controller
    {
        private SpotifyLocalAPI _spotifyLocal;
        private ImplicitGrantAuth _auth;
        private SpotifyAPI.Web.SpotifyWebAPI _spotify;
        private Track _currentTrack;
        private PrivateProfile _profile;
        private List<FullTrack> _savedTracks;
        private List<SimplePlaylist> _playlists;
        private bool check = true;
        private bool Winner = false;

        // GET: Test
        public ActionResult Music()
        {
            _auth = new ImplicitGrantAuth
            {
                RedirectUri = "http://localhost:8000",
                ClientId = "26d287105e31491889f3cd293d85bfea",
                Scope = Scope.UserReadPrivate | Scope.UserReadEmail | Scope.PlaylistReadPrivate | Scope.UserLibrarayRead | Scope.UserReadPrivate | Scope.UserFollowRead | Scope.UserReadBirthdate,
                State = "XSS"
            };
            _auth.OnResponseReceivedEvent += _auth_OnResponseReceivedEvent;

            //_auth.StartHttpServer(8000);
            _auth.DoAuth(true);

            SpotifyModel sm = new SpotifyModel();
            _spotifyLocal = new SpotifyLocalAPI();

            if (!SpotifyLocalAPI.IsSpotifyRunning())
            {
                return View(sm);
            }
            if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
            {
                return View(sm);
            }

            bool successful = _spotifyLocal.Connect();
            if (successful)
            {
                UpdateInfos();
                _spotifyLocal.ListenForEvents = true;
                sm.IsPlaying = true;
                try
                {
                    sm.VotedItemsDictionary = GetDictionaryDetails(sm.SongTitle);

                    if (Winner)
                    {
                        SpotifyModel spotifyModel = new SpotifyModel();
                        Winner = false;
                        spotifyModel.Truncate();
                    }

                    sm.SongTitle = _currentTrack.TrackResource.Name;
                    sm.SongArtist = _currentTrack.ArtistResource.Name;

                    Bitmap derp = new Bitmap(_currentTrack.GetAlbumArt(AlbumArtSize.Size320));
                    Byte[] imgFile;
                    var stream = new MemoryStream();
                    derp.Save(stream, ImageFormat.Png);
                    imgFile = stream.ToArray();
                    sm.SongAlbumArt = _currentTrack.GetAlbumArtUrl(AlbumArtSize.Size320);
                    sm.IsPlaying = true;
                    stream.Close();
                    sm.Tracks = GetPlaylists();
                    int index = 0;
                    foreach (var track in sm.Tracks)
                    {
                        if (track.title.Equals(_currentTrack.TrackResource.Name))
                        {
                            break;
                        }
                        index++;
                    }
                    if (index == 0)
                        index++;
                    sm.PreviousSongAlbumArt = sm.Tracks[index - 1].albumArt;
                    sm.PreviousSongTitle = sm.Tracks[index - 1].title;
                    sm.PreviousSongArtist = sm.Tracks[index - 1].artist;
                    if (index + 1 >= sm.Tracks.Count)
                        index = 0;
                    else
                        index++;
                    sm.NextSongAlbumArt = sm.Tracks[index].albumArt;
                    sm.NextSongTitle = sm.Tracks[index].title;
                    sm.NextSongArtist = sm.Tracks[index].artist;

                    _auth = null;
                    _spotifyLocal = null;
                    _spotify = null;
                }
                catch (Exception e)
                {
                }
            }
            return View(sm);
        }
        public Dictionary<string, int> GetDictionaryDetails(string SongTitle)
        {
            SpotifyModel spotifyModel = new SpotifyModel();
            List<StoredSpotifyData> votes = spotifyModel.GetVotes();

            HashSet<string> _items = new HashSet<string>();
            foreach (var vote in votes)
            {
                _items.Add(vote.SongRecommendation);
            }
            Dictionary<string, int> uniqueCountOfVotes = new Dictionary<string, int>();
            foreach (var _item in _items)
            {
                var count = votes.Count(item => item.SongRecommendation == _item);
                uniqueCountOfVotes.Add(_item, count);
            }
            if (votes.Count > 0 && (DateTime.Now.Millisecond - votes[0].Time.Millisecond > 600000 || !votes[0].SongThatWasPlaying.Equals(SongTitle)))
            {
                Winner = true;
            }
            return uniqueCountOfVotes;
        }

        void _auth_OnResponseReceivedEvent(Token token, string state)
        {
            _auth.StopHttpServer();

            if (state != "XSS")
            {
                return;
            }
            if (token.Error != null)
            {
                return;
            }

            _spotify = new SpotifyAPI.Web.SpotifyWebAPI
            {
                UseAuth = true,
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };
            check = false;
        }

        private List<TrackDetails> GetPlaylists()
        {
            while (check)
            {
                Thread.Sleep(250);
            };
            _profile = _spotify.GetPrivateProfile();
            Paging<SimplePlaylist> playlists = _spotify.GetUserPlaylists(_profile.Id);

            List<TrackDetails> listedTracks = new List<TrackDetails>();
            foreach (var ID in playlists.Items)
            {
                var details = _spotify.GetPlaylistTracks(_profile.Id, ID.Id);
                for (int i = 0; i < details.Total; i++)
                {
                    TrackDetails td = new TrackDetails();
                    td.title = details.Items[i].Track.Name;
                    td.artist = details.Items[i].Track.Artists[0].Name;
                    td.album = details.Items[i].Track.Album.Name;
                    td.albumArt = details.Items[i].Track.Album.Images[1].Url;
                    td.duration = details.Items[i].Track.DurationMs;
                    listedTracks.Add(td);
                }
            }
            return listedTracks.OrderBy(details => details.title).ToList();
        }

        [HttpPost]
        public JsonResult VoteSkip(string SongRecommendation, string SongThatWasPlaying)
        {
            _spotifyLocal = new SpotifyLocalAPI();
            bool successful = _spotifyLocal.Connect();
            string ip = Request.UserHostAddress;
            SpotifyModel spotifyModel = new SpotifyModel();
            if (successful && spotifyModel.IsValidVote(ip))
            {
                spotifyModel.CreateVoteEntry(SongRecommendation, DateTime.Now, SongThatWasPlaying, ip);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public void UpdateInfos()
        {
            StatusResponse status = _spotifyLocal.GetStatus();
            if (status == null)
                return;

            if (status.Track != null && status.Track.Length != 0) //Update track infos
                UpdateTrack(status.Track);
        }

        public void UpdateTrack(Track track)
        {
            _currentTrack = track;
        }

        private static String FormatTime(double sec)
        {
            TimeSpan span = TimeSpan.FromSeconds(sec);
            String secs = span.Seconds.ToString(), mins = span.Minutes.ToString();
            if (secs.Length < 2)
                secs = "0" + secs;
            return mins + ":" + secs;
        }
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