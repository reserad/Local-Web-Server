using System;
using System.Collections.Generic;
using System.Linq;
using Local_Web_Server.Models;
using Quartz;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Models;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace Local_Web_Server.Controllers
{
    public class TestController : IJob
    {
        private ImplicitGrantAuth _auth;
        private SpotifyAPI.Web.SpotifyWebAPI _spotify;
        private SpotifyLocalAPI _spotifyLocal;
        private Track _currentTrack;
        private bool Winner = false;

        public void Execute(IJobExecutionContext context)
        {
            SpotifyModel sm = new SpotifyModel();
            _auth = new ImplicitGrantAuth
            {
                RedirectUri = "http://localhost:8000",
                ClientId = "26d287105e31491889f3cd293d85bfea",
                Scope =
                    Scope.UserReadPrivate | Scope.UserReadEmail | Scope.PlaylistReadPrivate | Scope.UserLibrarayRead |
                    Scope.UserReadPrivate | Scope.UserFollowRead | Scope.UserReadBirthdate,
                State = "XSS"
            };
            _auth.OnResponseReceivedEvent += _auth_OnResponseReceivedEvent;

            _auth.StartHttpServer(8000);
            _auth.DoAuth(false);

            _spotifyLocal = new SpotifyLocalAPI();

            if (!SpotifyLocalAPI.IsSpotifyRunning())
            {
                return;
            }
            if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
            {
                return;
            }

            bool successful = _spotifyLocal.Connect();
            if (successful)
            {
                UpdateInfos();
                SpotifyModel spotifyModel = new SpotifyModel();
                var dictDetails = GetDictionaryDetails(_currentTrack.TrackResource.Name).Keys.ToList();
                if (Winner)
                {
                    dictDetails.Sort();
                    var winner = dictDetails[0];
                    spotifyModel.Truncate();
                    while (!_currentTrack.TrackResource.Name.Equals(winner))
                    {
                        _spotifyLocal.Skip();
                    }
                    Winner = false;
                }
            }
        }

        public void UpdateTrack(Track track)
        {
            _currentTrack = track;
        }

        public void UpdateInfos()
        {
            StatusResponse status = _spotifyLocal.GetStatus();
            if (status == null)
                return;

            if (status.Track != null && status.Track.Length != 0) //Update track infos
                UpdateTrack(status.Track);
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


        private void _auth_OnResponseReceivedEvent(Token token, string state)
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
        }
    }
}