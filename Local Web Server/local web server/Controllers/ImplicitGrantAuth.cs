using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Local_Web_Server.Models;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace Local_Web_Server.Controllers
{
    public class ImplicitGrantAuth
    {
        public delegate void OnResponseReceived(Token token, String error);

        private SimpleHttpServer _httpServer;
        private Thread _httpThread;
        public String ClientId { get; set; }
        public String RedirectUri { get; set; }
        public String State { get; set; }
        public Scope Scope { get; set; }
        public Boolean ShowDialog { get; set; }
        public event OnResponseReceived OnResponseReceivedEvent;

        /// <summary>
        ///     Start the auth process (Make sure the internal HTTP-Server ist started)
        /// </summary>
        public void DoAuth(bool Client)
        {
            String uri = GetUri();
            if(!Client)
                Process.Start(uri);
            else
            {
                _httpServer = new SimpleHttpServer(80, AuthType.Implicit);
                _httpServer.OnAuth += HttpServerOnOnAuthClient;

                _httpThread = new Thread(_httpServer.Listen);
                _httpThread.Start();
            }
        }

        private String GetUri()
        {
            StringBuilder builder = new StringBuilder("https://accounts.spotify.com/authorize/?");
            builder.Append("client_id=" + ClientId);
            builder.Append("&response_type=token");
            builder.Append("&redirect_uri=" + RedirectUri);
            builder.Append("&state=" + State);
            builder.Append("&scope=" + Scope.GetStringAttribute(" "));
            builder.Append("&show_dialog=" + ShowDialog);
            return builder.ToString();
        }

        /// <summary>
        ///     Start the internal HTTP-Server
        /// </summary>
        public void StartHttpServer(int port = 80)
        {
            _httpServer = new SimpleHttpServer(port, AuthType.Implicit);
            _httpServer.OnAuth += HttpServerOnOnAuth;

            _httpThread = new Thread(_httpServer.Listen);
            _httpThread.Start();
        }

        private void HttpServerOnOnAuth(AuthEventArgs e)
        {
            Token t = new Token()
            {
                AccessToken = e.Code,
                TokenType = e.TokenType,
                ExpiresIn = e.ExpiresIn,
                Error = e.Error
            };
            if (OnResponseReceivedEvent != null)
                OnResponseReceivedEvent(t, e.State);
            SpotifyModel sm = new SpotifyModel();
            sm.Truncate();
            sm.WriteToken(e.Code, e.TokenType, e.ExpiresIn, e.Error, t.CreateDate);
        }

        private void HttpServerOnOnAuthClient(AuthEventArgs e)
        {
            SpotifyModel sm = new SpotifyModel();
            if (OnResponseReceivedEvent != null)
                OnResponseReceivedEvent(sm.GetToken(), "XSS");
        }

        /// <summary>
        ///     This will stop the internal HTTP-Server (Should be called after you got the Token)
        /// </summary>
        public void StopHttpServer()
        {
            _httpServer.Dispose();
            _httpServer = null;
        }
    }
}