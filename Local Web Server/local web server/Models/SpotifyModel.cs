using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using SpotifyAPI.Web.Models;

namespace Local_Web_Server.Models
{
    public class TrackDetails
    {
        public string title;
        public string artist;
        public string album;
        public string albumArt;
        public int duration;
    }

    public class StoredSpotifyData
    {
        public string SongRecommendation;
        public DateTime Time;
        public string SongThatWasPlaying;
        public string IP;
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
        public bool IsValidIP { get; set; }
        public Dictionary<string, int> VotedItemsDictionary { get; set; } 

        public bool Empty
        {
            get
            {
                return (
                    string.IsNullOrWhiteSpace(SongTitle) &&
                    string.IsNullOrWhiteSpace(SongArtist));
            }
        }
        public void CreateVoteEntry(string SongRecommendation, DateTime Time, string SongThatWasPlaying, string IP)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
            "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _insertSql = @"INSERT INTO [dbo].[Spotify] 
                                    ([SongRecommendation], [Time], [SongThatWasPlaying], [IP])
                                    VALUES (@sr, @t, @sp, @ip)";
                var insertCmd = new SqlCommand(_insertSql, cn);
                insertCmd.Parameters
                    .Add(new SqlParameter("@sr", SqlDbType.VarChar))
                    .Value = SongRecommendation;
                insertCmd.Parameters
                    .Add(new SqlParameter("@t", SqlDbType.DateTime))
                    .Value = Time;
                insertCmd.Parameters
                    .Add(new SqlParameter("@sp", SqlDbType.VarChar))
                    .Value = SongThatWasPlaying;
                insertCmd.Parameters
                    .Add(new SqlParameter("@ip", SqlDbType.VarChar))
                    .Value = IP;
                cn.Open();
                insertCmd.ExecuteNonQuery();
            }
        }

        public List<StoredSpotifyData> GetVotes()
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"SELECT [SongRecommendation], [Time], [SongThatWasPlaying], [IP] FROM [dbo].[Spotify]";
                var cmd = new SqlCommand(_sql, cn);
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    List<StoredSpotifyData> data = new List<StoredSpotifyData>();
                    while (reader.Read())
                    {
                        StoredSpotifyData d = new StoredSpotifyData();
                        d.SongRecommendation = reader["SongRecommendation"].ToString();
                        d.Time = Convert.ToDateTime(reader["Time"].ToString());
                        d.SongThatWasPlaying = reader["SongThatWasPlaying"].ToString();
                        d.IP = reader["IP"].ToString();
                        data.Add(d);
                    }
                    reader.Dispose();
                    cmd.Dispose();
                    return data;
                }
                else
                {
                    return new List<StoredSpotifyData>();
                }
            }
        }

        public bool IsValidVote(string IP)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"SELECT [IP] FROM [dbo].[Spotify] WHERE [IP] = @ip";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@ip", SqlDbType.NVarChar))
                    .Value = IP;
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    cmd.Dispose();
                    return true;
                }
                cmd.Dispose();
                return false;
            }
        }

        public void Truncate()
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename" +
             "=|DataDirectory|\\Database.mdf; Integrated Security=True"))
            {
                string _sql = @"TRUNCATE TABLE Spotify";
                var cmd = new SqlCommand(_sql, cn);
                cn.Open();
                var reader = cmd.ExecuteReader();
                reader.Dispose();
                cmd.Dispose();
            }
        }
    }
}
