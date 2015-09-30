using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Local_Web_Server.Models
{
    public class MessagesModel
    {
        [Required]
        [Display(Name = "To")]
        public string To { get; set; }

        [Required]
        [Display(Name = "From")]
        public string From { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool HasSeen { get; set; }

        public void SendMessage(string to, string from, string message, DateTime when)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFileName=|DataDirectory|Database.mdf;Integrated Security=True"))
            {
                string _insertSql = @"INSERT INTO [dbo].[Messages] ([To], [From], [Message], [TimeStamp], [HasSeen]) VALUES (@to, @from, @message, @date, @seen)";
                var insertCmd = new SqlCommand(_insertSql, cn);
                insertCmd.Parameters
                .Add(new SqlParameter("@to", SqlDbType.NVarChar))
                .Value = to;
                insertCmd.Parameters
                .Add(new SqlParameter("@from", SqlDbType.NVarChar))
                .Value = from;
                insertCmd.Parameters
                .Add(new SqlParameter("@message", SqlDbType.NVarChar))
                .Value = message;
                insertCmd.Parameters
                .Add(new SqlParameter("@date", SqlDbType.NVarChar))
                .Value = System.DateTime.Now;
                insertCmd.Parameters
                .Add(new SqlParameter("@seen", SqlDbType.NVarChar))
                .Value = false;
                cn.Open();
                insertCmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public List<MessagesModel> RecieveMessages(string username)
        {
            List<MessagesModel> allMessages = new List<MessagesModel>();
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFileName=|DataDirectory|Database.mdf;Integrated Security=True"))
            {
                string _sql = @"SELECT * FROM [dbo].[Messages] WHERE [To] = @u OR [From] = @u";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = username;
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        MessagesModel message = new MessagesModel();
                        message.To = reader["To"].ToString();
                        message.From = reader["From"].ToString();
                        message.Message = reader["Message"].ToString();
                        message.TimeStamp = Convert.ToDateTime(reader["TimeStamp"].ToString());
                        message.HasSeen = Convert.ToBoolean(reader["HasSeen"].ToString());
                        allMessages.Add(message);
                    }
                }
            }
            return allMessages;
        }
        public void MarkAsRead(string to)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFileName=|DataDirectory|Database.mdf;Integrated Security=True"))
            {
                string _sql = @"UPDATE [dbo].[Messages] SET HasSeen = @HasSeen WHERE [To] = @to";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@HasSeen", SqlDbType.Bit))
                    .Value = true;
                cmd.Parameters
                    .Add(new SqlParameter("@to", SqlDbType.NVarChar))
                    .Value = to;
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public int NumberUnreadMessages(string username)
        {
            using (var cn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFileName=|DataDirectory|Database.mdf;Integrated Security=True"))
            {
                string _sql = @"SELECT COUNT(*) FROM [dbo].[Messages] WHERE [To] = @user AND [HasSeen] = @seen";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@user", SqlDbType.NVarChar))
                    .Value = username;
                cmd.Parameters
                    .Add(new SqlParameter("@seen", SqlDbType.Bit))
                    .Value = false;
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int count = reader.GetInt32(0);
                    return count;
                }
                return 0;
            }
        }
    }
}