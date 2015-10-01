using System.Collections.Generic;

namespace Local_Web_Server.Models
{
    public class TrackDetails
    {
        public string title;
        public string artist;
        public string album;
        public string albumArt;
    }
    public class SpotifyModel
    {
        public bool IsPlaying { get; set; }
        public string SongTitle { get; set; }
        public string SongArtist { get; set; }
        public string SongAlbumArt { get; set; }
        public List<TrackDetails> Tracks { get; set; }
        public TrackDetails ChosenSong { get; set; }
        public string PreviousSongTitle { get; set; }
        public string PreviousSongArtist { get; set; }
        public string PreviousSongAlbumArt { get; set; }
        public string NextSongTitle { get; set; }
        public string NextSongArtist { get; set; }
        public string NextSongAlbumArt { get; set; }

        public bool Empty
    {
        get
        {
            return (
                string.IsNullOrWhiteSpace(SongTitle) &&
                string.IsNullOrWhiteSpace(SongArtist));
        }
    }
    }
}
