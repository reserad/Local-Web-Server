using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Web.Mvc;
using Local_Web_Server.Models;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Enums;
using SpotifyAPI.Local.Models;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using Track = SpotifyAPI.Local.Models.Track;

namespace Local_Web_Server.Controllers
{
    public class TestController : Controller
    {
        private SpotifyLocalAPI _spotifyLocal;
        private ImplicitGrantAuth _auth;
        private SpotifyAPI.Web.SpotifyWebAPI _spotify;
        private Track _currentTrack;
        private PrivateProfile _profile;
        private List<FullTrack> _savedTracks;
        private List<SimplePlaylist> _playlists;
        private bool check = true;

        // GET: Test
        public ActionResult Index()
        {
            _auth = new ImplicitGrantAuth
            {
                RedirectUri = "http://localhost:8000",
                ClientId = "26d287105e31491889f3cd293d85bfea",
                Scope = Scope.UserReadPrivate | Scope.UserReadEmail | Scope.PlaylistReadPrivate | Scope.UserLibrarayRead | Scope.UserReadPrivate | Scope.UserFollowRead | Scope.UserReadBirthdate,
                State = "XSS"
            };
            _auth.OnResponseReceivedEvent += _auth_OnResponseReceivedEvent;

            _auth.StartHttpServer(8000);
            _auth.DoAuth();

            SpotifyModel sm = new SpotifyModel();
            _spotifyLocal = new SpotifyLocalAPI();

            if (!SpotifyLocalAPI.IsSpotifyRunning())
            {
                return Json(false);
            }
            if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
            {
                return Json(false);
            }

            bool successful = _spotifyLocal.Connect();
            if (successful)
            {
                UpdateInfos();
                _spotifyLocal.ListenForEvents = true;
                sm.IsPlaying = true;
                sm.SongTitle = _currentTrack.TrackResource.Name;
                sm.SongArtist = _currentTrack.ArtistResource.Name;

                Bitmap derp = new Bitmap(_currentTrack.GetAlbumArt(AlbumArtSize.Size320));
                Byte[] imgFile;
                var stream = new MemoryStream();
                derp.Save(stream, ImageFormat.Png);
                imgFile = stream.ToArray();
                sm.SongAlbumArt = imgFile;
                sm.IsPlaying = true;
                stream.Close();
                sm.Tracks = GetPlaylists();
                _auth = null;
                _spotifyLocal = null;
                _spotify = null;
            }
            return View(sm);
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
                Thread.Sleep(250); // pause for 1/4 second;
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
                    listedTracks.Add(td);
                }
            }
            return listedTracks;
        }

        [HttpPost]
        public JsonResult VoteSkip(SpotifyLocalAPI spotify)
        {
            spotify.Skip();
            return Json(true);
        }

        public void UpdateInfos()
        {
            StatusResponse status = _spotifyLocal.GetStatus();
            if (status == null)
                return;
            //Basic Spotify Infos
            //UpdatePlayingStatus(status.Playing);

            if (status.Track != null && status.Track.Length != 0) //Update track infos
                UpdateTrack(status.Track);
        }

        public void UpdateTrack(Track track)
        {
            _currentTrack = track;
        }

        //public void UpdatePlayingStatus(bool playing)
        //{
        //    isPlayingLabel.Text = playing.ToString();
        //}

        //void _spotifyLocal_OnVolumeChange(VolumeChangeEventArgs e)
        //{
        //    volumeLabel.Text = (e.NewVolume*100).ToString(CultureInfo.InvariantCulture);
        //}

        //void _spotifyLocal_OnTrackTimeChange(TrackTimeChangeEventArgs e)
        //{
        //    timeLabel.Text = FormatTime(e.TrackTime) + "/" + FormatTime(_currentTrack.Length);
        //    timeProgressBar.Value = (int) e.TrackTime;
        //}

        //void _spotifyLocal_OnTrackChange(TrackChangeEventArgs e)
        //{
        //    UpdateTrack(e.NewTrack);
        //}

        //void _spotifyLocal_OnPlayStateChange(PlayStateEventArgs e)
        //{
        //    UpdatePlayingStatus(e.Playing);
        //}

        //private void connectBtn_Click(object sender, EventArgs e)
        //{
        //    Connect();
        //}

        //private void playUrlBtn_Click(object sender, EventArgs e)
        //{
        //    _spotifyLocal.PlayURL(playTextBox.Text, contextTextBox.Text);
        //}

        //private void playBtn_Click(object sender, EventArgs e)
        //{
        //    _spotifyLocal.Play();
        //}

        //private void pauseBtn_Click(object sender, EventArgs e)
        //{
        //    _spotifyLocal.Pause();
        //}

        //private void prevBtn_Click(object sender, EventArgs e)
        //{
        //    _spotifyLocal.Previous();
        //}

        //private void skipBtn_Click(object sender, EventArgs e)
        //{
        //    _spotifyLocal.Skip();
        //}

        private static String FormatTime(double sec)
        {
            TimeSpan span = TimeSpan.FromSeconds(sec);
            String secs = span.Seconds.ToString(), mins = span.Minutes.ToString();
            if (secs.Length < 2)
                secs = "0" + secs;
            return mins + ":" + secs;
        }
    }
}